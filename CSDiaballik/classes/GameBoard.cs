using System;
using System.Collections.Generic;
using System.Linq;

namespace CSDiaballik
{
    public class GameBoard
    {
        private Piece[,] _pieces;
        private int _size;


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
        public List<Position2D> GetValidMoves(Piece pie)
        {
            var pos = pie.Position;
            return new List<Position2D>
                {
                    new Position2D(pos.X - 1, pos.Y),
                    new Position2D(pos.X + 1, pos.Y),
                    new Position2D(pos.X, pos.Y - 1),
                    new Position2D(pos.X, pos.Y + 1)
                }.Where(p => p.X >= 0 && p.X < _size
                             && p.Y >= 0 && p.Y < _size
                             && _pieces[p.X, p.Y] == null)
                .ToList();
        }

        /// <summary>
        /// Moves a piece to a new location.
        /// </summary>
        /// <param name="p">Piece to move</param>
        /// <param name="dst">New position</param>
        public void MovePiece(Piece p, Position2D dst)
        {
            throw new NotImplementedException();
        }
    }
}