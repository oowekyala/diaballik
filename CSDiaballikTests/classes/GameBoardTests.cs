using System.Collections.Generic;
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
            var positions = RandomPositionsPair(size + 1, size).Select(e => e.ToList());
            var empty = positions.Item1[0];
            positions.Foreach(e => e.RemoveAt(0));
            var specs = positions.Select(p => DummyPlayerSpec(size, p));
            
            var board = GameBoard.New(size, specs.Item1, specs.Item2);

            Assert.IsTrue(board.IsFree(empty));
            board.MovePiece(positions.Item1[0], empty);
            Assert.IsFalse(board.IsFree(empty));
        }


        [Test]
        public void TestPlayersHaveNotSizePieces()
        {
            const int size = 7;
            var specs = DummyPlayerSpecPair(size - 1);

            Assert.That(() => GameBoard.New(size, specs.Item1, specs.Item2), Throws.ArgumentException);
        }


        [Test]
        public void TestPositionHasPiece()
        {
            const int size = 7;
            var specs = DummyPlayerSpecPair(size);

            var gb = GameBoard.New(size, specs.Item1, specs.Item2);

            foreach (var p in specs.Item1.Positions)
            {
                Assert.IsFalse(gb.IsFree(p));
            }

            foreach (var p in specs.Item1.Positions)
            {
                Assert.IsFalse(gb.IsFree(p));
            }
        }


        [Test]
        public void TestStackPieces()
        {
            const int size = 7;
            var position = RandomPositions(size).ToList();
            var dummy = DummyPlayerSpec(size, position);
            var dummy2 = DummyPlayerSpec(size, position);

            Assert.AreNotSame(dummy, dummy2);
            Assert.That(() => GameBoard.New(size, dummy, dummy2), Throws.ArgumentException);
        }


        [Test]
        public void TestUnequalNumPieces()
        {
            const int size = 7;
            var dummy = DummyPlayerSpec(size, RandomPositions(size - 1, size));
            var dummy2 = DummyPlayerSpec(size, RandomPositions(size).ToList());

            Assert.AreNotSame(dummy, dummy2);
            Assert.That(() => GameBoard.New(size, dummy, dummy2), Throws.ArgumentException);
        }
    }
}
