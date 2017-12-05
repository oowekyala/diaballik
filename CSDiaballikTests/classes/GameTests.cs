using NUnit.Framework;

namespace CSDiaballik.Tests {
    [TestFixture]
    public class GameTests {

        [Test]
        public void TestPlayerIdentity() {
            const int size = 7;
            var (spec1, spec2) = TestUtil.DummyPlayerSpecPair(size);
            var gb = GameBoard.New(7, spec1, spec2);

            var g = GameState.New(gb);
            Assert.AreSame(spec1.Player, g.Player1);
            Assert.AreSame(spec2.Player, g.Player2);
        }

    }
}
