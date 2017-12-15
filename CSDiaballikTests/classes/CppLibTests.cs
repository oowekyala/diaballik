using System;
using System.Collections.Generic;
using System.Linq;
using CppDiaballik;
using NUnit.Framework;
using static CSDiaballik.Tests.TestUtil;

namespace CSDiaballik.Tests {
    [TestFixture]
    public class CppLibTests {
       
        [Test]
        public void TestGetPossibleMoves([Range(3, 13)] int size) {
            var specs = (size - 1, 0).Map(row => Enumerable.Range(0, size).Select(y => new Position2D(row, y)))
                .Map(ps => DummyPlayerSpec(size, ps));

            var board = GameBoard.Create(size, specs);
            var ba = board.Analyser();

            var possibleMoves = ba.MovesForPiece(new Position2D(0, 0)).ToList();
            var possibleMovesBall = ba.MovesForBall(board.BallBearer1);

            Assert.AreEqual(2, possibleMoves.Count); // direct front and front right
            Assert.AreEqual(2, possibleMovesBall.Count()); // direct left and right
        }
    }
}
