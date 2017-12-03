using NUnit.Framework;

namespace CSDiaballik.Tests
{
    [TestFixture]
    public class GameTests
    {
        [Test]
        public void TestPlayerIdentity()
        {
            const int size = 7;
            var specs = TestUtil.DummyPlayerSpecPair(size);
            var gb = GameBoard.New(7, specs.Item1, specs.Item2);

            var g = new Game(gb, specs.Item1.Player, specs.Item1.Player);
            Assert.AreSame(specs.Item1.Player, g.Player1);
            Assert.AreSame(specs.Item1.Player, g.Player2);
        }
    }
}
