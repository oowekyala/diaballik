using System;
using System.Collections.Generic;
using System.Linq;

namespace CSDiaballik
{
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
    public class GameBoard
    {
        private readonly IPlayer[,] _board;

        private readonly IPlayer _p1;
        private readonly IPlayer _p2;

        private readonly HashSet<Position2D> _player1;
        private readonly HashSet<Position2D> _player2;

        private Position2D _ballBearer1;
        private Position2D _ballBearer2;


        // Performs full consistency checks
        private GameBoard(int size, FullPlayerBoardSpec p1Spec, FullPlayerBoardSpec p2Spec)
        {
            Size = size;

            _p1 = p1Spec.Player;
            _p2 = p2Spec.Player;

            var p1List = p1Spec.Positions.ToList();
            var p2List = p2Spec.Positions.ToList();

            CheckPieces(size, p1List, p2List);

            _ballBearer1 = p1List[p1Spec.Ball];
            _ballBearer2 = p2List[p2Spec.Ball];

            _board = new IPlayer[Size, Size];

            p1List.ForEach(p => _board[p.X, p.Y] = _p1);
            p2List.ForEach(p => _board[p.X, p.Y] = _p2);

            _player1 = new HashSet<Position2D>(p1List);
            _player2 = new HashSet<Position2D>(p2List);
        }


        public int Size { get; }


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
        {
            return new GameBoard(size, p1Spec, p2Spec);
        }


        private static void CheckPieces(int size, List<Position2D> p1List, List<Position2D> p2List)
        {
            if (p1List.Count != p2List.Count || p1List.Count != size)
            {
                throw new ArgumentException("One or more players have an incorrect number of pieces");
            }


            if (p1List.Distinct().Count() != p1List.Count
                || p2List.Distinct().Count() != p2List.Count
                || p1List.Intersect(p2List).Count() != 0)
            {
                throw new ArgumentException("One or more players have duplicate pieces");
            }
        }


        public IEnumerable<Position2D> getPlayer1Pieces()
        {
            return _player1;
        }


        public IEnumerable<Position2D> getPlayer2Pieces()
        {
            return _player2;
        }


        /// <summary>
        ///     Gets the positions to which this piece can legally be moved.
        /// </summary>
        /// <param name="pos">The position of the piece</param>
        /// <exception cref="ArgumentException">If the piece is invalid</exception>
        public IEnumerable<Position2D> GetValidMoves(Position2D pos)
        {
            if (!IsPositionOnBoard(pos) || IsFree(pos))
            {
                throw new ArgumentException("Illegal: no piece to move");
            }

            return pos.Neighbours().Where(p => IsPositionOnBoard(p) && IsFree(p));
        }


        public bool IsFree(Position2D pos)
        {
            return _board[pos.X, pos.Y] == null;
        }


        /// <summary>
        ///     Moves a piece to a new location. Does not check whether the move satisfies the rules of the game.
        /// </summary>
        /// <param name="src">Piece to move</param>
        /// <param name="dst">New position</param>
        /// <exception cref="ArgumentException">If the piece or destination position is invalid</exception>
        public void MovePiece(Position2D src, Position2D dst)
        {
            CheckPositionIsValid(dst);
            if (IsFree(src))
            {
                throw new ArgumentException("Illegal: no piece to move");
            }

            CheckPositionIsValid(src);
            if (!IsFree(dst))
            {
                throw new ArgumentException("Illegal: destination is not free");
            }

            var player = GetPiece(src);
            SetPiece(src, null);
            SetPiece(dst, player);
            var positions = _PositionsForPlayer(player);
            positions.Remove(src);
            positions.Add(dst);
        }


        private HashSet<Position2D> _PositionsForPlayer(IPlayer player)
        {
            return player == _p1 ? _player1
                   : player == _p2 ? _player2 : null;
        }


        /// <summary>
        ///     Gets the positions of the pieces of a player, or null if the player is not recognised.
        /// </summary>
        /// <param name="player">The player</param>
        /// <returns>The positions</returns>
        public IEnumerable<Position2D> PositionsForPlayer(IPlayer player)
        {
            return _PositionsForPlayer(player);
        }


        private IPlayer GetPiece(Position2D pos)
        {
            return _board[pos.X, pos.Y];
        }


        private void SetPiece(Position2D pos, IPlayer player)
        {
            _board[pos.X, pos.Y] = player;
        }


        private bool IsPositionOnBoard(Position2D p)
        {
            return p.X >= 0 && p.X < Size
                   && p.Y >= 0 && p.Y < Size;
        }


        private void CheckPositionIsValid(Position2D p)
        {
            if (!IsPositionOnBoard(p))
            {
                throw new ArgumentException("Illegal: position is out of the board " + p);
            }
        }
    }
}
