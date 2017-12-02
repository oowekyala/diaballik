using System.Linq;
using NUnit.Framework;
using static CSDiaballik.Tests.TestUtil;

namespace CSDiaballik.Tests
{
    [TestFixture]
    public class GameBoardTests
    {
        [Test]
        public void TestMovePiece()
        {
            const int size = 7;
            var positions = RandomPositionsPair(size + 1, size).ToList().Select(e => e.ToList()).ToList();
            var empty = positions[0][0];
            positions.ForEach(e => e.RemoveAt(0));
            var pieces = positions.Select(DummyPlayer).Select(p => p.Pieces).ToList();

            var board = new GameBoard(size, pieces[0], pieces[1]);

            Assert.IsFalse(board.PositionHasPiece(empty));
            board.MovePiece(pieces[0][0], empty);
            Assert.IsTrue(board.PositionHasPiece(empty));
        }


        [Test]
        public void TestPlayersHaveNotSizePieces()
        {
            const int size = 7;
            var positions = RandomPositionsPair(size - 1);
            var dummy = DummyPlayer(positions.Item1);
            var dummy2 = DummyPlayer(positions.Item2);

            Assert.AreNotSame(dummy, dummy2);
            Assert.That(() => new GameBoard(size, dummy.Pieces, dummy2.Pieces), Throws.ArgumentException);
        }


        [Test]
        public void TestPositionHasPiece()
        {
            const int size = 7;
            var positions = RandomPositionsPair(size, size);

            var dummy = DummyPlayer(positions.Item1);
            var dummy2 = DummyPlayer(positions.Item2);
            var gb = new GameBoard(size, dummy.Pieces, dummy2.Pieces);

            foreach (var p in dummy.Pieces)
            {
                Assert.IsTrue(gb.PositionHasPiece(p.Position));
            }

            foreach (var p in dummy2.Pieces)
            {
                Assert.IsTrue(gb.PositionHasPiece(p.Position));
            }
        }


        [Test]
        public void TestStackPieces()
        {
            const int size = 7;
            var position = RandomPositions(size).ToList();
            var dummy = DummyPlayer(position);
            var dummy2 = DummyPlayer(position);

            Assert.AreNotSame(dummy, dummy2);
            Assert.That(() => new GameBoard(size, dummy.Pieces, dummy2.Pieces), Throws.ArgumentException);
        }


        [Test]
        public void TestUnequalNumPieces()
        {
            const int size = 7;
            var dummy = DummyPlayer(RandomPositions(size - 1, size));
            var dummy2 = DummyPlayer(RandomPositions(size).ToList());

            Assert.AreNotSame(dummy, dummy2);
            Assert.That(() => new GameBoard(size, dummy.Pieces, dummy2.Pieces), Throws.ArgumentException);
        }
    }
}
