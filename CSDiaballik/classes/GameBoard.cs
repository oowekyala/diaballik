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
        private readonly Piece[,] _pieces;
        

        
        

        public GameBoard(int size, IEnumerable<Piece> p1Pieces, IEnumerable<Piece> p2Pieces)
        {
            Size = size;
            _pieces = new Piece[Size, Size];

            var p2PiecesList = p2Pieces as IList<Piece> ?? p2Pieces.ToList();
            var p1PiecesList = p1Pieces as IList<Piece> ?? p1Pieces.ToList();

            if (p1PiecesList.Count != p2PiecesList.Count || p1PiecesList.Count != size)
            {
                throw new ArgumentException("One or more players have an incorrect number of pieces");
            }

            PutPiecesOnBoard(p1PiecesList);
            PutPiecesOnBoard(p2PiecesList);
        }


        public int Size { get; }


        private void PutPiecesOnBoard(IEnumerable<Piece> ps)
        {
            foreach (var piece in ps)
            {
                var pos = piece.Position;
                if (_pieces[pos.X, pos.Y] != null)
                {
                    throw new ArgumentException("Two pieces cannot be at the same position");
                }
                _pieces[pos.X, pos.Y] = piece;
            }
        }


        /// <summary>
        ///     Gets the positions to which this piece can legally be moved.
        /// </summary>
        /// <param name="piece">The piece</param>
        /// <exception cref="ArgumentException">If the piece is invalid</exception>
        public IEnumerable<Position2D> GetValidMoves(Piece piece)
        {
            CheckPieceIsOnBoard(piece);

            return piece.Position.Neighbours()
                        .Where(p => IsPositionOnBoard(p) && _pieces[p.X, p.Y] == null);
        }


        /// <summary>
        ///     Moves a piece to a new location. Does not check whether the move satisfies the rules of the game.
        /// </summary>
        /// <param name="p">Piece to move</param>
        /// <param name="dst">New position</param>
        /// <exception cref="ArgumentException">If the piece or destination position is invalid</exception>
        public void MovePiece(Piece p, Position2D dst)
        {
            CheckPieceIsOnBoard(p);
            CheckPositionIsValid(dst);
            _pieces[p.Position.X, p.Position.Y] = null;
            _pieces[dst.X, dst.Y] = p;
            p.Position = dst;
        }


        private void CheckPieceIsOnBoard(Piece p)
        {
            CheckPositionIsValid(p.Position);

            if (_pieces[p.Position.X, p.Position.Y] != p)
            {
                throw new ArgumentException("Illegal: piece not at its place " + p);
            }
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


        public bool PositionHasPiece(Position2D pos)
        {
            return _pieces[pos.X, pos.Y] != null;
        }
    }
}
