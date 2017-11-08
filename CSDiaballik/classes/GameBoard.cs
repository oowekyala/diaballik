using System;
using System.Collections.Generic;

namespace CSDiaballik
{
    public class GameBoard
    {
        private Piece[][] pieces;
        
        
        private int size;


        public GameBoardMemento GetMemento()
        {
            throw new NotImplementedException();
        }


        /// <summary>
        /// Gets the positions to which this piece can legally be moved.
        /// </summary>
        /// <param name="p">The piece</param>
        public List<Position2D> GetValidMoves(Piece p)
        {
            throw new NotImplementedException();
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