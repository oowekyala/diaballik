using System;
using System.Collections.Generic;
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
    /// </summary>
    public class GameBoard {

        private readonly IPlayer[,] _board;

        public IPlayer Player1 { get; }
        public IPlayer Player2 { get; }

        private readonly HashSet<Position2D> _player1Positions;
        private readonly HashSet<Position2D> _player2Positions;

        public Position2D BallBearer1 { get; private set; }
        public Position2D BallBearer2 { get; private set; }

        public IEnumerable<Position2D> Player1Positions => _player1Positions;
        public IEnumerable<Position2D> Player2Positions => _player2Positions;


        // Performs full consistency checks
        private GameBoard(int size, (FullPlayerBoardSpec, FullPlayerBoardSpec) specs) {
            Size = size;

            (Player1, Player2) = specs.Map(x => x.Player);
            var (p1List, p2List) = specs.Map(x => x.Positions.ToList());


            CheckPieces(size, p1List, p2List);

            (BallBearer1, BallBearer2) = (p1List, p2List).Merge(specs, (l, spec) => l[spec.BallIndex]);

            _board = new IPlayer[Size, Size];
            p1List.ForEach(p => _board[p.X, p.Y] = Player1);
            p2List.ForEach(p => _board[p.X, p.Y] = Player2);

            (_player1Positions, _player2Positions) = (p1List, p2List).Map(l => new HashSet<Position2D>(l));
        }


        public int Size { get; }


        public bool IsVictoriousPlayer(IPlayer player)
            => PositionsForPlayer(player).Select(p => p.X).Any(i => Size - 1 - GetRowIndexOfInitialLine(player) == i);


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

            return pos.Neighbours().Where(p => IsPositionOnBoard(p) && IsFree(p));
        }


        public bool IsFree(Position2D pos) {
            return _board[pos.X, pos.Y] == null;
        }


        /// <summary>
        ///     Moves a piece to a new location. 
        ///     This method cannot move the piece which carries the ball.
        /// </summary>
        /// <param name="src">Piece to move</param>
        /// <param name="dst">New position</param>
        /// <exception cref="ArgumentException">If the piece or destination position is invalid</exception>
        public void MovePiece(Position2D src, Position2D dst) {
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
            SetPiece(src, null);
            SetPiece(dst, player);
            var positions = _PositionsForPlayer(player);
            positions.Remove(src);
            positions.Add(dst);
        }


        /// <summary>
        ///     Moves the piece which carries the ball to a new location.
        /// </summary>
        /// <param name="src">Piece to move</param>
        /// <param name="dst">New position</param>
        /// <exception cref="ArgumentException">If the piece or destination position is invalid</exception>
        public void MoveBall(Position2D src, Position2D dst) {
            CheckPositionIsValid(src);
            if (IsFree(src) || !Equals(src, BallBearer1) && !Equals(src, BallBearer2)) {
                throw new ArgumentException("Illegal: no ball to move on position " + src);
            }

            CheckPositionIsValid(dst);
            if (IsFree(dst) || PlayerOn(dst) != PlayerOn(src)) {
                throw new ArgumentException("Illegal: no friendly piece on position " + dst);
            }

            if (PlayerOn(src) == Player1) {
                BallBearer1 = dst;
            }
            else {
                BallBearer2 = dst;
            }
        }


        private HashSet<Position2D> _PositionsForPlayer(IPlayer player)
            => player == Player1 ? _player1Positions
               : player == Player2 ? _player2Positions
               : throw new ArgumentException("Unknown player");


        // Gets the index of the starting row of a player. Used to check for victory
        private int GetRowIndexOfInitialLine(IPlayer player)
            => player == Player1 ? 0
               : player == Player2 ? Size - 1
               : throw new ArgumentException("Unknown player");


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
            return player == Player1 ? BallBearer1 :
                   player == Player2 ? BallBearer2 : throw new ArgumentException("Unknown player");
        }


        private IPlayer PlayerOn(Position2D pos) {
            return _board[pos.X, pos.Y];
        }


        private void SetPiece(Position2D pos, IPlayer player) {
            _board[pos.X, pos.Y] = player;
        }


        private bool IsPositionOnBoard(Position2D p) {
            return p.X >= 0 && p.X < Size
                   && p.Y >= 0 && p.Y < Size;
        }


        private void CheckPositionIsValid(Position2D p) {
            if (!IsPositionOnBoard(p)) {
                throw new ArgumentException("Illegal: position is out of the board " + p);
            }
        }

    }
}
