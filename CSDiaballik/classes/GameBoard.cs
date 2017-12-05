using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Runtime.InteropServices;

using System.Linq;

namespace CSDiaballik {
    /// <summary>
    ///     Represents the game board and manages the moving of pieces.
    ///     By convention, if n is the value of Size, the four corners
    ///     of the board have the positions:
    ///     <pre>
    ///         (0, 0) --- (0, n-1)
    ///         |            |
    ///         |            |
    ///         (n-1, 0) --- (n-1, n-1)
    ///     </pre>
    ///     By convention, the first player owns the row n-1 (the bottom
    ///     one), and the second player owns the row 0 (the top one).
    /// 
    ///     This class is immutable.
    /// </summary>
    public class GameBoard {

        // we could also delegate everything to the previous board
        // taking care to avoid delegation chains.

        private readonly ImmutableDictionary<Position2D, IPlayer> _boardLookup;

        public IPlayer Player1 { get; }
        public IPlayer Player2 { get; }

        private readonly ImmutableHashSet<Position2D> _player1Positions;
        private readonly ImmutableHashSet<Position2D> _player2Positions;

        public Position2D BallBearer1 { get; }
        public Position2D BallBearer2 { get; }

        public IEnumerable<Position2D> Player1Positions => _player1Positions;
        public IEnumerable<Position2D> Player2Positions => _player2Positions;


        // Performs full consistency checks, only the first time
        private GameBoard(int size, (FullPlayerBoardSpec, FullPlayerBoardSpec) specs) {
            Size = size;

            (Player1, Player2) = specs.Map(x => x.Player);
            var (p1List, p2List) = specs.Map(x => x.Positions.ToList());


            CheckPieces(size, p1List, p2List);

            (BallBearer1, BallBearer2) = (p1List, p2List).Zip(specs, (l, spec) => l[spec.BallIndex]);

            var lookupBuilder = ImmutableDictionary.CreateBuilder<Position2D, IPlayer>();
            specs.Map(spec => spec.Positions.Select(p => new KeyValuePair<Position2D, IPlayer>(p, spec.Player)))
                 .Foreach(lookupBuilder.AddRange);
            _boardLookup = lookupBuilder.ToImmutable();

            (_player1Positions, _player2Positions) = (p1List, p2List).Map(ImmutableHashSet.CreateRange);
        }


        public int Size { get; }


        // for moving pieces
        private GameBoard(GameBoard previous, ImmutableDictionary<Position2D, IPlayer> lookup,
                          (ImmutableHashSet<Position2D>, ImmutableHashSet<Position2D>) positions) {
            Size = previous.Size;
            Player1 = previous.Player1;
            Player2 = previous.Player2;
            _boardLookup = lookup;
            (BallBearer1, BallBearer2) = previous.BallBearerPair();
            (_player1Positions, _player2Positions) = positions;
        }


        // For moving the ball
        private GameBoard(GameBoard previous,
                          (Position2D, Position2D) ballBearers) {
            Size = previous.Size;
            Player1 = previous.Player1;
            Player2 = previous.Player2;
            _boardLookup = previous._boardLookup;
            (BallBearer1, BallBearer2) = ballBearers;
            _player1Positions = previous._player1Positions;
            _player2Positions = previous._player2Positions;
        }


        public bool IsVictoriousPlayer(IPlayer player)
            => PositionsForPlayer(player).Select(p => p.X).Any(i => Size - 1 - GetRowIndexOfInitialLine(player) == i);


        // Gets the index of the starting row of a player. Used to check for victory
        private int GetRowIndexOfInitialLine(IPlayer player)
            => player == Player1 ? Size - 1
               : player == Player2 ? 0
               : throw new ArgumentException("Unknown player");


        /// <summary>
        ///     Creates a new gameboard.
        /// </summary>
        /// <param name="size">Size of the board</param>
        /// <param name="p1Spec">Spec of player 2</param>
        /// <param name="p2Spec">Spec of player 2</param>
        /// <returns>A new gameboard</returns>
        /// <exception cref="ArgumentException">
        ///     If the players have an incorrect number of pieces,
        ///     or there are duplicate pieces
        /// </exception>
        public static GameBoard New(int size, FullPlayerBoardSpec p1Spec, FullPlayerBoardSpec p2Spec)
            => new GameBoard(size, (p1Spec, p2Spec));


        /// <summary>
        ///     Creates a new gameboard.
        /// </summary>
        /// <param name="size">Size of the board</param>
        /// <param name="specs">Pair of the specs of each players</param>
        /// <returns>A new gameboard</returns>
        /// <exception cref="ArgumentException">
        ///     If the players have an incorrect number of pieces,
        ///     or there are duplicate pieces
        /// </exception>
        public static GameBoard New(int size, (FullPlayerBoardSpec, FullPlayerBoardSpec) specs)
            => new GameBoard(size, specs);


        private static void CheckPieces(int size, List<Position2D> p1List, List<Position2D> p2List) {
            if (p1List.Count != p2List.Count || p1List.Count != size) {
                throw new ArgumentException("One or more players have an incorrect number of pieces");
            }


            if (p1List.Distinct().Count() != p1List.Count
                || p2List.Distinct().Count() != p2List.Count
                || p1List.Intersect(p2List).Count() != 0) {
                throw new ArgumentException("One or more players have duplicate pieces");
            }
        }


        /// <summary>
        ///     Gets the positions to which this piece can legally be moved.
        /// </summary>
        /// <param name="pos">The position of the piece</param>
        /// <exception cref="ArgumentException">If the piece is invalid</exception>
        public IEnumerable<Position2D> GetValidMoves(Position2D pos) {
            
            if (!IsPositionOnBoard(pos) || IsFree(pos)) {
                throw new ArgumentException("Illegal: no piece to move");
            }
            /*IntPtr nativeAlgo;
            nativeAlgo = AlgoBoard_new();
            AlgoBoard_fillMap(nativeAlgo, 5);*/

            return pos.Neighbours().Where(p => IsPositionOnBoard(p) && IsFree(p));
        }


        public bool IsFree(Position2D pos) => !_boardLookup.ContainsKey(pos);


        /// <summary>
        ///     Moves a piece to a new location. This method cannot move the piece which carries the ball.
        /// </summary>
        /// <param name="src">Piece to move</param>
        /// <param name="dst">New position</param>
        /// <returns>The updated gameboard</returns>
        /// <exception cref="ArgumentException">If the piece or destination position is invalid</exception>
        public GameBoard MovePiece(Position2D src, Position2D dst) {
            if (Equals(src, BallBearer1) || Equals(src, BallBearer2)) {
                throw new ArgumentException("Illegal: cannot move the piece which carries the ball");
            }

            CheckPositionIsValid(src);
            if (IsFree(src)) {
                throw new ArgumentException("Illegal: no piece to move");
            }

            CheckPositionIsValid(dst);
            if (!IsFree(dst)) {
                throw new ArgumentException("Illegal: destination is not free");
            }

            var player = PlayerOn(src);
            var lookup = _boardLookup.ToBuilder();
            lookup.Remove(src);
            lookup.Add(dst, player);

            var positions = _PositionsForPlayer(player).ToBuilder();
            positions.Remove(src);
            positions.Add(dst);

            return player == Player1
                       ? new GameBoard(this, lookup.ToImmutable(), (positions.ToImmutable(), _player2Positions))
                       : new GameBoard(this, lookup.ToImmutable(), (_player1Positions, positions.ToImmutable()));
        }


        /// <summary>
        ///     Moves the piece which carries the ball to a new location.
        /// </summary>
        /// <param name="src">Piece to move</param>
        /// <param name="dst">New position</param>
        /// <returns>The updated game</returns>
        /// <exception cref="ArgumentException">If the piece or destination position is invalid</exception>
        public GameBoard MoveBall(Position2D src, Position2D dst) {
            CheckPositionIsValid(src);
            if (IsFree(src) || !Equals(src, BallBearer1) && !Equals(src, BallBearer2)) {
                throw new ArgumentException("Illegal: no ball to move on position " + src);
            }

            CheckPositionIsValid(dst);
            if (IsFree(dst) || PlayerOn(dst) != PlayerOn(src)) {
                throw new ArgumentException("Illegal: no friendly piece on position " + dst);
            }

            return PlayerOn(src) == Player1
                       ? new GameBoard(this, (dst, BallBearer2))
                       : new GameBoard(this, (BallBearer1, dst));
        }


        private ImmutableHashSet<Position2D> _PositionsForPlayer(IPlayer player)
            => player == Player1 ? _player1Positions
               : player == Player2 ? _player2Positions
               : throw new ArgumentException("Unknown player");


        /// <summary>
        ///     Gets the positions of the pieces of a player.
        /// </summary>
        /// <param name="player">The player</param>
        /// <returns>The positions</returns>
        /// <exception cref="ArgumentException">If the player is not recognised</exception>
        public IEnumerable<Position2D> PositionsForPlayer(IPlayer player) => _PositionsForPlayer(player);


        /// <summary>
        ///     Gets the position of the piece bearing the ball of a player.
        /// </summary>
        /// <param name="player">The player</param>
        /// <returns>The position</returns>
        /// <exception cref="ArgumentException">If the player is not recognised</exception>
        public Position2D BallBearerForPlayer(IPlayer player) => player == Player1 ? BallBearer1 :
                                                                 player == Player2 ? BallBearer2 :
                                                                 throw new ArgumentException("Unknown player");


        public IPlayer PlayerOn(Position2D pos) => _boardLookup[pos];


        public bool IsPositionOnBoard(Position2D p) => p.X >= 0 && p.X < Size
                                                       && p.Y >= 0 && p.Y < Size;


        private void CheckPositionIsValid(Position2D p) {
            if (!IsPositionOnBoard(p)) {
                throw new ArgumentException("Illegal: position is out of the board " + p);
            }
        }


        /// <summary>
        /// Returns true if there is a piece-free vertical, horizontal, or diagonal line 
        /// between the two positions on the board.
        /// </summary>
        /// <param name="p1">Position</param>
        /// <param name="p2">Position</param>
        /// <returns>True if there is a free line between the specified positions</returns>
        /// <exception cref="ArgumentException">If the positions are identical</exception>
        public bool IsLineFreeBetween(Position2D p1, Position2D p2) {
            var deltaX = Math.Abs(p2.X - p1.X);
            var deltaY = Math.Abs(p2.Y - p1.Y);

            if (deltaX == 0 && deltaY == 0) {
                throw new ArgumentException("Illegal: cannot move to the same piece");
            }

            if (deltaX <= 1 && deltaY <= 1) {
                return true; // pieces are side by side
            }

            var ys = Enumerable.Range(Math.Min(p1.Y, p2.Y) + 1, deltaY - 2);
            var xs = Enumerable.Range(Math.Min(p1.X, p2.X) + 1, deltaX - 2);

            if (deltaX == 0) {
                // same row
                return ys.Select(y => new Position2D(p1.X, y)).All(IsFree);
            }

            if (deltaY == 0) {
                // same column
                return xs.Select(x => new Position2D(x, p1.Y)).All(IsFree);
            }

            return deltaX == deltaY && xs.Zip(ys, Position2D.New).All(IsFree); // diagonal
        }


        public (IEnumerable<Position2D>, IEnumerable<Position2D>) PositionsPair()
            => (Player1Positions, Player2Positions);


        public (Position2D, Position2D) BallBearerPair() => (BallBearer1, BallBearer2);

        [DllImport("CppLib.dll", CallingConvention = CallingConvention.Cdecl)]
        extern static void AlgoBoard_fillMap(IntPtr algo, int nbTiles);

        [DllImport("CppLib.dll", CallingConvention = CallingConvention.Cdecl)]
        extern static IntPtr AlgoBoard_new();

        [DllImport("CppLib.dll", CallingConvention = CallingConvention.Cdecl)]
        extern static IntPtr AlgoBoard_delete(IntPtr algo);
    }
}
