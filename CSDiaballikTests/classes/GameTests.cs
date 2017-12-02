using NUnit.Framework;

namespace CSDiaballik.Tests
{
    [TestFixture]
    public class GameTests
    {
        [Test]
        public void GameTest()
        {
            const int size = 7;
            var pos = TestUtil.RandomPositionsPair(size, size);
            var p1 = TestUtil.DummyPlayer(pos.Item1);
            var p2 = TestUtil.DummyPlayer(pos.Item2);

            var gb = new GameBoard(7, p1.Pieces, p2.Pieces);

            var g = new Game(gb, p1, p2);
            Assert.AreSame(p1, g.Player1);
            Assert.AreSame(p2, g.Player2);
        }
    }
}
