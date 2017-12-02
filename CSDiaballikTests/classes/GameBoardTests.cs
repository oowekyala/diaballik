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
            var dummy = TestUtil.DummyPlayer(TestUtil.RandomPositions(15, size).Distinct());

            var gb = new GameBoard(size, dummy.Pieces, Enumerable.Empty<Piece>());

            foreach (var p in dummy.Pieces)
            {
                Assert.IsTrue(gb.PositionHasPiece(p.Position));
            }
        }


        public void TestStackPieces()
        {
        }
    }
}
