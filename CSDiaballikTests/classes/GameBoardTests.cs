using System.Linq;
using NUnit.Framework;
using NUnit.Framework.Constraints;
using static CSDiaballik.Tests.TestUtil;

namespace CSDiaballik.Tests {
    [TestFixture]
    public class GameBoardTests {

        [Test]
        public void TestMovePiece([Range(3, 13)] int size) {
            var positions = RandomPositionsPair(size + 1, size).Map(e => e.ToList());
            var empty = positions.Item1[0];
            positions.Foreach(e => e.RemoveAt(0));
            var specs = positions.Map(p => DummyPlayerSpec(size, p));

            var board = GameBoard.New(size, specs);

            Assert.IsTrue(board.IsFree(empty));
            board.MovePiece(positions.Item1[0], empty);
            Assert.IsFalse(board.IsFree(empty));
        }


        [Test]
        public void TestMovePieceWithBall([Range(3, 13)] int size) {
            const int ballIndex = 1;
            var positions = RandomPositionsPair(size + 1, size).Map(e => e.ToList());
            var empty = positions.Item1[0];
            positions.Foreach(e => e.RemoveAt(0));
            var ballBearer = positions.Item1[ballIndex];
            var specs = positions.Map(p => DummyPlayerSpec(p, ballIndex));

            var board = GameBoard.New(size, specs);

            Assert.AreEqual(ballBearer, board.BallBearer1);
            Assert.IsTrue(board.IsFree(empty));
            Assert.That(() => board.MovePiece(ballBearer, empty), Throws.ArgumentException);
        }


        public void TestMoveBall([Range(3, 13)] int size) {
        }


        [Test]
        public void TestPlayersHaveNotSizePieces([Range(3, 13)] int size) {
            var specs = DummyPlayerSpecPair(size - 1);

            Assert.That(() => GameBoard.New(size, specs), Throws.ArgumentException);
        }


        [Test]
        public void TestPositionHasPiece([Range(3, 13)] int size) {
            var specs = DummyPlayerSpecPair(size);
            var gb = GameBoard.New(size, specs);

            foreach (var p in specs.Item1.Positions) {
                Assert.IsFalse(gb.IsFree(p));
            }

            foreach (var p in specs.Item1.Positions) {
                Assert.IsFalse(gb.IsFree(p));
            }
        }


        [Test]
        public void TestStackPieces([Range(3, 13)] int size) {
            var position = RandomPositions(size).ToList();
            var dummy = DummyPlayerSpec(size, position);
            var dummy2 = DummyPlayerSpec(size, position);

            Assert.AreNotSame(dummy, dummy2);
            Assert.That(() => GameBoard.New(size, dummy, dummy2), Throws.ArgumentException);
        }


        [Test]
        public void TestUnequalNumPieces([Range(3, 13)] int size) {
            var dummy = DummyPlayerSpec(size, RandomPositions(size - 1, size));
            var dummy2 = DummyPlayerSpec(size, RandomPositions(size).ToList());

            Assert.AreNotSame(dummy, dummy2);
            Assert.That(() => GameBoard.New(size, dummy, dummy2), Throws.ArgumentException);
        }


        [Test]
        public void TestBallPosition([Range(3, 13)] int size) {
            var specs = RandomPositionsPair(size).Map(p => DummyPlayerSpec(p, 0));
            var (ball1, ball2) = specs.Map(p => p.Positions.First());

            var gb = GameBoard.New(size, specs);

            Assert.AreEqual(ball1, gb.BallBearer1);
            Assert.AreEqual(ball2, gb.BallBearer2);
        }

    }
}
