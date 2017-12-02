using System.Collections.Generic;
using System.Drawing;
using NUnit.Framework;

namespace CSDiaballik.Tests
{
    [TestFixture]
    public class GameTests
    {
        [Test]
        public void GameTest()
        {
            var p1Pieces = new List<Piece>();
            var p2Pieces = new List<Piece>();

            var gb = new GameBoard(7, p1Pieces, p2Pieces);
            var p1 = new NoobAiPlayer(Color.Blue, "toto", new List<Position2D>());
            var p2 = new NoobAiPlayer(Color.Blue, "titi", new List<Position2D>()); 
            var g = new Game(gb, p1, p2);
            Assert.AreEqual(p1, g.Player1);
            Assert.AreEqual(p2, g.Player2);
        }

      
    }
}