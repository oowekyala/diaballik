using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using NUnit.Framework;

namespace CSDiaballik.Tests
{
    [TestFixture]
    public class GameBoardTests
    {
     


        [Test]
        public void TestPositionHasPiece()
        {
            const int size = 7;
            var dummy = new NoobAiPlayer(Color.Blue, "toto", new List<Position2D>());
            var randomPieces = TestUtil.RandomPositions(15, size).Select(p => new Piece(dummy, p)).Distinct().ToList();

            var gb = new GameBoard(size, randomPieces, Enumerable.Empty<Piece>());

            foreach (var p in randomPieces)
            {
                Assert.IsTrue(gb.PositionHasPiece(p.Position));
            }
        }

        public void TestStackPieces()
        {
            
            
            
        }
        
        
    }
}
