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
        public void TestNewAnalyser([Range(3, 13)] int size) {
            var specs = (size - 1, 0).Map(row => Enumerable.Range(0, size).Select(y => new Position2D(row, y)))
                .Map(ps => DummyPlayerSpec(size, ps));

            var board = GameBoard.Create(size, specs);
            var ba = BoardAnalyser.New(board);
        }

        [Test]
        public void TestGetPossibleMoves([Range(3, 13)] int size) {
            var specs = (size - 1, 0).Map(row => Enumerable.Range(0, size).Select(y => new Position2D(row, y)))
                .Map(ps => DummyPlayerSpec(size, ps));

            var board = GameBoard.Create(size, specs);
            var ba = BoardAnalyser.New(board);

            var possibleMoves = ba.GetPossibleMoves(new Position2D(0, 0));
      /*      var possibleMovesBall = ba.GetPossibleMoves(board.BallBearer1);


            Assert.AreEqual(1, possibleMoves.Count);
            Assert.AreEqual(size - 1, possibleMovesBall.Count);
            Assert.AreEqual(new Position2D(1, 0), possibleMoves[0]);
        */}
    }
}