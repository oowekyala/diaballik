using System.Linq;
using NUnit.Framework;
using static CSDiaballik.Tests.TestUtil;

namespace CSDiaballik.Tests
{
    [TestFixture]
    public class CppLibTests
    {
        [Test]
        public void TestLibrary([Range(3, 13)] int size)
        {
            var specs = (size - 1, 0).Map(row => Enumerable.Range(0, size).Select(y => new Position2D(row, y)))
                .Map(ps => DummyPlayerSpec(size, ps));

            var board = GameBoard.Create(size, specs);
            board.GetValidMoves(new Position2D(0, 0));
        }
    }
}