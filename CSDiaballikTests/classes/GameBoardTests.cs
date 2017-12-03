using System.Linq;
using NUnit.Framework;
using static CSDiaballik.Tests.TestUtil;
using System.Collections.Generic;

namespace CSDiaballik.Tests {
    [TestFixture]
    public class GameBoardTests {

        [Test]
        public void TestMovePiece() {
            const int size = 7;
            var positions = RandomPositionsPair(size + 1, size).Select(e => e.ToList());
            var empty = positions.Item1[0];
            positions.Foreach(e => e.RemoveAt(0));
            var specs = positions.Select(p => DummyPlayerSpec(size, p));

            var board = GameBoard.New(size, specs);

            Assert.IsTrue(board.IsFree(empty));
            board.MovePiece(positions.Item1[0], empty);
            Assert.IsFalse(board.IsFree(empty));
        }


        [Test]
        public void TestMovePieceWithBall() {
            const int size = 7;
            const int ballIndex = 1;
            var positions = RandomPositionsPair(size + 1, size).Select(e => e.ToList());
            var empty = positions.Item1[0];
            positions.Foreach(e => e.RemoveAt(0));
            var ballBearer = positions.Item1[ballIndex];
            var specs = positions.Select(p => DummyPlayerSpec(p, ballIndex));

            var board = GameBoard.New(size, specs);

            Assert.AreEqual(ballBearer, board.BallBearer1);
            Assert.IsTrue(board.IsFree(empty));
            Assert.That(() => board.MovePiece(ballBearer, empty), Throws.ArgumentException);
        }


        public void TestMoveBall() {
            
        }


        [Test]
        public void TestPlayersHaveNotSizePieces() {
            const int size = 7;
            var specs = DummyPlayerSpecPair(size - 1);

            Assert.That(() => GameBoard.New(size, specs), Throws.ArgumentException);
        }


        [Test]
        public void TestPositionHasPiece() {
            const int size = 7;
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
        public void TestStackPieces() {
            const int size = 7;
            var position = RandomPositions(size).ToList();
            var dummy = DummyPlayerSpec(size, position);
            var dummy2 = DummyPlayerSpec(size, position);

            Assert.AreNotSame(dummy, dummy2);
            Assert.That(() => GameBoard.New(size, dummy, dummy2), Throws.ArgumentException);
        }


        [Test]
        public void TestUnequalNumPieces() {
            const int size = 7;
            var dummy = DummyPlayerSpec(size, RandomPositions(size - 1, size));
            var dummy2 = DummyPlayerSpec(size, RandomPositions(size).ToList());

            Assert.AreNotSame(dummy, dummy2);
            Assert.That(() => GameBoard.New(size, dummy, dummy2), Throws.ArgumentException);
        }


        [Test]
        public void TestBallPosition() {
            const int size = 7;
            var specs = RandomPositionsPair(size).Select(p => DummyPlayerSpec(p, 0));
            var (ball1, ball2) = specs.Select(p => p.Positions.First());

            var gb = GameBoard.New(size, specs);

            Assert.AreEqual(ball1, gb.BallBearer1);
            Assert.AreEqual(ball2, gb.BallBearer2);
        }

        [Test]
        public void TestVictory()
        {
            const int size = 7;

            List<Position2D> p1Positions = new List<Position2D>();
            List<Position2D> p2Positions = new List<Position2D>();

            for (int i=0; i<size; i++)
            {
                p1Positions.Add(new Position2D(0, i));
                p2Positions.Add(new Position2D(size-1, i));
            }

            FullPlayerBoardSpec p1spec = DummyPlayerSpec(size, p1Positions);
            FullPlayerBoardSpec p2spec = DummyPlayerSpec(size, p2Positions);

            var board = GameBoard.New(size, p1spec, p2spec);
            
            board.MovePiece(new Position2D(0,0), new Position2D(1, 0));
            board.MovePiece(new Position2D(size-1, size-1), new Position2D(0, 0));
            board.MoveBall(board.BallBearer2, new Position2D(0, 0));
            Assert.IsTrue(board.IsVictoriousPlayer(board.Player2));
        }

    }
}
