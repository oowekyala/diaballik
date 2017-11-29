using Microsoft.VisualStudio.TestTools.UnitTesting;
using CSDiaballik;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace CSDiaballik.Tests
{
    [TestClass()]
    public class GameBoardTests
    {
        [TestMethod()]
        public void InitializationTest()
        {
            int size = 7;
            Position2D piecePos = new Position2D(size, 0);
            List<Piece> p2Pieces = new List<Piece>();
            NoobAiPlayer ai = new NoobAiPlayer(Color.Blue, "toto", new List<Position2D>());
            Piece p1 = new Piece(ai);
            p1.Position = piecePos;
            List<Piece> p1Pieces = new List<Piece> { p1 };
            GameBoard gb = new GameBoard(size, p1Pieces, p2Pieces);
            Assert.IsTrue(gb.PositionHasPiece(piecePos));
            
        }
    }
}