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
    public class GameTests
    {
        [TestMethod()]
        public void GameTest()
        {
            List<Piece> p1Pieces = new List<Piece>();
            List<Piece> p2Pieces = new List<Piece>();

            GameBoard gb = new GameBoard(7, p1Pieces, p2Pieces);
            AiPlayer p1 = new NoobAiPlayer(Color.Blue, "toto", new List<Position2D>());
            AiPlayer p2 = new NoobAiPlayer(Color.Blue, "titi", new List<Position2D>()); 
            Game g = new Game(gb, p1, p2);
            Assert.AreEqual(p1, g.Player1);
            Assert.AreEqual(p2, g.Player2);
        }

        [TestMethod()]
        public void GameTest1()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void UpdateTest()
        {
            Assert.Fail();
        }
    }
}