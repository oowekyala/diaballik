using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Text;


namespace Diaballik.Core {
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
    ///     This class is immutable.
    /// </summary>
    public class GameBoard {
        // we could also delegate everything to the previous board
        // taking care to avoid delegation chains.

        private readonly ImmutableDictionary<Position2D, IPlayer> _boardLookup;

        private readonly ImmutableHashSet<Position2D> _player1Positions;
        private readonly ImmutableHashSet<Position2D> _player2Positions;

        public IPlayer Player1 { get; }
        public IPlayer Player2 { get; }

        public Position2D BallBearer1 { get; }
        public Position2D BallBearer2 { get; }

        public IEnumerable<Position2D> Player1Positions => _player1Positions;
        public IEnumerable<Position2D> Player2Positions => _player2Positions;


        public int Size { get; }

        // Performs full consistency checks, only the first time
        private GameBoard(int size, (FullPlayerBoardSpec, FullPlayerBoardSpec) specs) {
            Size = size;
            (Player1, Player2) = specs.Map(x => x.Player);

            var positions = specs.Map(x => x.Positions.ToList());

#if DEBUG
            CheckPieces(size, positions);
#endif

            (BallBearer1, BallBearer2) = positions.Zip(specs, (l, spec) => l[spec.BallIndex]);
            (_player1Positions, _player2Positions) = positions.Map(ImmutableHashSet.CreateRange);

            var lookupBuilder = ImmutableDictionary.CreateBuilder<Position2D, IPlayer>();
            specs.Map(spec => spec.Positions.Select(p => new KeyValuePair<Position2D, IPlayer>(p, spec.Player)))
                 .Foreach(lookupBuilder.AddRange);
            _boardLookup = lookupBuilder.ToImmutable();
        }


        // used internally, for MovePiece updates
        private GameBoard(GameBoard previous, ImmutableDictionary<Position2D, IPlayer> lookup,
            (ImmutableHashSet<Position2D>, ImmutableHashSet<Position2D>) positions) {
            Size = previous.Size;
            Player1 = previous.Player1;
            Player2 = previous.Player2;
            _boardLookup = lookup;
            (BallBearer1, BallBearer2) = previous.BallBearerPair();
            (_player1Positions, _player2Positions) = positions;
        }


        // used internally, for MoveBall updates
        private GameBoard(GameBoard previous, (Position2D, Position2D) ballBearers) {
            Size = previous.Size;
            Player1 = previous.Player1;
            Player2 = previous.Player2;
            _boardLookup = previous._boardLookup;
            (BallBearer1, BallBearer2) = ballBearers;
            _player1Positions = previous._player1Positions;
            _player2Positions = previous._player2Positions;
        }


        public bool IsVictoriousPlayer(IPlayer player) {
            return PositionsForPlayer(player)
                .Select(p => p.X)
                .Any(i => Size - 1 - GetRowIndexOfInitialLine(player) == i);
        }


        // Gets the index of the starting row of a player. Used to check for victory
        private int GetRowIndexOfInitialLine(IPlayer player) {
            return player == Player1
                ? Size - 1
                : player == Player2
                    ? 0
                    : throw new ArgumentException("Unknown player");
        }


        /// <summary>
        ///     Creates a new gameBoard.
        /// </summary>
        /// <param name="size">Size of the board</param>
        /// <param name="p1Spec">Spec of player 2</param>
        /// <param name="p2Spec">Spec of player 2</param>
        /// <returns>A new gameboard</returns>
        /// <exception cref="ArgumentException">
        ///     If the players have an incorrect number of pieces,
        ///     or there are duplicate pieces
        /// </exception>
        public static GameBoard Create(int size, FullPlayerBoardSpec p1Spec, FullPlayerBoardSpec p2Spec) {
            return new GameBoard(size, (p1Spec, p2Spec));
        }


        /// <summary>
        ///     Creates a new gameBoard.
        /// </summary>
        /// <param name="size">Size of the board</param>
        /// <param name="specs">Pair of the specs of each players</param>
        /// <returns>A new gameboard</returns>
        /// <exception cref="ArgumentException">
        ///     If the players have an incorrect number of pieces,
        ///     or there are duplicate pieces
        /// </exception>
        public static GameBoard Create(int size, (FullPlayerBoardSpec, FullPlayerBoardSpec) specs) {
            return new GameBoard(size, specs);
        }

#if DEBUG
        private static void CheckPieces(int size, (List<Position2D>, List<Position2D>) positions) {
            Debug.Assert(positions.Map(l => l.Count).Forall(i => i == size),
                         "One or more players have an incorrect number of pieces");

            var (p1List, p2List) = positions;

            Debug.Assert(p1List.Distinct().Count() == p1List.Count
                         && p2List.Distinct().Count() == p2List.Count
                         && !p1List.Intersect(p2List).Any(),
                         "One or more players have duplicate pieces");
        }
#endif

        /// <summary>
        ///     Returns true if the position has no piece on it.
        ///     Doesn't check for validity of the position.
        /// </summary>
        /// <param name="pos">Position to check</param>
        /// <returns>True if the position is free</returns>
        public bool IsFree(Position2D pos) {
            return !_boardLookup.ContainsKey(pos);
        }

        public bool HasBall(Position2D p) {
            return p == BallBearer1 || p == BallBearer2;
        }


        /// <summary>
        ///     Moves a piece to a new location. This method cannot move the piece which carries the ball.
        /// </summary>
        /// <param name="src">Piece to move</param>
        /// <param name="dst">New position</param>
        /// <returns>The updated gameboard</returns>
        /// <exception cref="ArgumentException">If the piece or destination position is invalid</exception>
        public GameBoard MovePiece(Position2D src, Position2D dst) {
#if DEBUG
            Debug.Assert(!HasBall(src), "Illegal: cannot move the piece which carries the ball");

            CheckPositionIsValid(src);
            CheckPositionIsValid(dst);

            Debug.Assert(!IsFree(src), "Illegal: no piece to move");
            Debug.Assert(IsFree(dst), "Illegal: destination is not free");
#endif

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
        ///     Moves the ball from one piece to another friendly piece.
        /// </summary>
        /// <param name="src">Piece to move</param>
        /// <param name="dst">New position</param>
        /// <returns>The updated game</returns>
        /// <exception cref="ArgumentException">If the piece or destination position is invalid</exception>
        public GameBoard MoveBall(Position2D src, Position2D dst) {
#if DEBUG
            CheckPositionIsValid(src);
            Debug.Assert(!IsFree(src) && HasBall(src),
                         "Illegal: no ball to move on position " + src);
            CheckPositionIsValid(dst);
            Debug.Assert(!IsFree(dst) && PlayerOn(dst) == PlayerOn(src),
                         "Illegal: no friendly piece on position " + dst);
#endif

            return PlayerOn(src) == Player1
                ? new GameBoard(this, (dst, BallBearer2))
                : new GameBoard(this, (BallBearer1, dst));
        }


        private ImmutableHashSet<Position2D> _PositionsForPlayer(IPlayer player) {
            return player == Player1
                ? _player1Positions
                : player == Player2
                    ? _player2Positions
                    : throw new ArgumentException("Unknown player");
        }


        /// <summary>
        ///     Gets the positions of the pieces of a player.
        /// </summary>
        /// <param name="player">The player</param>
        /// <returns>The positions</returns>
        /// <exception cref="ArgumentException">If the player is not recognised</exception>
        public IEnumerable<Position2D> PositionsForPlayer(IPlayer player) {
            return _PositionsForPlayer(player);
        }


        /// <summary>
        ///     Gets the position of the piece bearing the ball of a player.
        /// </summary>
        /// <param name="player">The player</param>
        /// <returns>The position</returns>
        /// <exception cref="ArgumentException">If the player is not recognised</exception>
        public Position2D BallBearerForPlayer(IPlayer player) {
            return player == Player1
                ? BallBearer1
                : player == Player2
                    ? BallBearer2
                    : throw new ArgumentException("Unknown player");
        }


        public IPlayer GetOtherPlayer(IPlayer player) {
            return player == Player1
                ? Player2
                : player == Player2
                    ? Player1
                    : throw new ArgumentException("Unknown player");
        }

        /// <summary>
        ///     Returns the player at the specified position.
        ///     Returns null if the tile is empty or out of bounds.
        /// </summary>
        /// <param name="pos">The position to test</param>
        /// <returns>The player, or null</returns>
        public IPlayer PlayerOn(Position2D pos) {
            var ok = _boardLookup.TryGetValue(pos, out var player);
            return ok ? player : null;
        }


        public bool IsOnBoard(Position2D p) {
            return p.X >= 0 && p.X < Size
                   && p.Y >= 0 && p.Y < Size;
        }

#if DEBUG
        private void CheckPositionIsValid(Position2D p) {
            if (!IsOnBoard(p)) {
                throw new ArgumentException("Illegal: position is out of the board " + p);
            }
        }
#endif

        // TODO Move this into the C++ lib
        /// <summary>
        ///     Returns true if there is a piece-free vertical, horizontal, or diagonal line
        ///     between the two positions on the Board.
        /// </summary>
        /// <param name="p1">Position</param>
        /// <param name="p2">Position</param>
        /// <returns>True if there is a free line between the specified positions</returns>
        /// <exception cref="ArgumentException">If the positions are identical</exception>
        public bool IsLineFreeBetween(Position2D p1, Position2D p2) {
            var dX = p2.X - p1.X;
            var dY = p2.Y - p1.Y;
            var deltaX = Math.Abs(dX);
            var deltaY = Math.Abs(dY);

            Debug.Assert(deltaX != 0 || deltaY != 0, "Illegal: cannot move to the same piece");


            if (deltaX <= 1 && deltaY <= 1) {
                return true; // pieces are side by side
            }

            // neither straight line nor diagonal
            if (deltaX != 0 && deltaY != 0 && deltaX != deltaY) {
                return false;
            }

            // compute the range only if count > 0. Otherwise we won't use the value anyway
            var ys = deltaY > 0 ? Enumerable.Range(Math.Min(p1.Y, p2.Y) + 1, deltaY - 1) : null;
            var xs = deltaX > 0 ? Enumerable.Range(Math.Min(p1.X, p2.X) + 1, deltaX - 1) : null;

            if (deltaX == 0) {
                // same row
                return ys.Select(y => new Position2D(p1.X, y)).All(IsFree);
            }

            if (deltaY == 0) {
                // same column
                return xs.Select(x => new Position2D(x, p1.Y)).All(IsFree);
            }

            // we have to reverse one list in two quadrants, otherwise the zipped positions cross the diagonal
            return deltaX == deltaY && xs.Zip(dX * dY > 0 ? ys : ys.Reverse(), Position2D.New).All(IsFree); // diagonal
        }


        public (IEnumerable<Position2D>, IEnumerable<Position2D>) PositionsPair() {
            return (Player1Positions, Player2Positions);
        }


        public (Position2D, Position2D) BallBearerPair() {
            return (BallBearer1, BallBearer2);
        }


        public override string ToString() {
            var sb = new StringBuilder();
            sb.Append("   ");

            void AppendItem(object o) {
                sb.AppendFormat("{0,3}", o);
            }

            foreach (var i in Enumerable.Range(0, Size)) {
                AppendItem(i);
            }
            sb.AppendLine();


            for (var x = 0; x < Size; x++) {
                AppendItem(x);
                for (var y = 0; y < Size; y++) {
                    var pos = new Position2D(x, y);
                    if (pos == BallBearer1) {
                        AppendItem('X');
                        continue;
                    }
                    if (pos == BallBearer2) {
                        AppendItem('O');
                        continue;
                    }
                    var p = PlayerOn(pos);
                    if (p == Player1) {
                        AppendItem('x');
                    } else if (p == Player2) {
                        AppendItem('o');
                    } else {
                        AppendItem(' ');
                    }
                }
                sb.AppendLine();
            }
            return sb.ToString();
        }
    }
}