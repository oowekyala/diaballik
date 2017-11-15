using System;
using System.Collections.Generic;
using System.Linq;

namespace CSDiaballik
{
    public class GameBoard
    {
        private readonly Piece[,] _pieces;
        private readonly int _size;


        private GameBoard(int size, IEnumerable<Piece> p1Pieces, IEnumerable<Piece> p2Pieces)
        {
            _size = size;
            _pieces = new Piece[_size, _size];

            PutPiecesOnBoard(p1Pieces);
            PutPiecesOnBoard(p2Pieces);
        }

        private void PutPiecesOnBoard(IEnumerable<Piece> ps)
        {
            foreach (var piece in ps)
            {
                var pos = piece.Position;
                _pieces[pos.X, pos.Y] = piece;
            }
        }


        public GameBoardMemento GetMemento()
        {
            throw new NotImplementedException();
        }


        /// <summary>
        /// Gets the positions to which this piece can legally be moved.
        /// </summary>
        /// <param name="pie">The piece</param>
        /// <exception cref="ArgumentException">If the piece is invalid</exception>
        public List<Position2D> GetValidMoves(Piece pie)
        {
            CheckPieceIsOnBoard(pie);

            var pos = pie.Position;
            return new List<Position2D>
                {
                    new Position2D(pos.X - 1, pos.Y),
                    new Position2D(pos.X + 1, pos.Y),
                    new Position2D(pos.X, pos.Y - 1),
                    new Position2D(pos.X, pos.Y + 1)
                }.Where(p => IsPositionOnBoard(p)
                             && _pieces[p.X, p.Y] == null)
                .ToList();
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
            return p.X >= 0 && p.X < _size
                   && p.Y >= 0 && p.Y < _size;
        }

        private void CheckPositionIsValid(Position2D p)
        {
            if (!IsPositionOnBoard(p))
            {
                throw new ArgumentException("Illegal: position is out of the board " + p);
            }
        }

        /// <summary>
        /// Moves a piece to a new location. Does not check whether the move satisfies the rules of the game.
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
        }
    }
}