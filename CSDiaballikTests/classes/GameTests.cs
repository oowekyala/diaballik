using NUnit.Framework;

namespace CSDiaballik.Tests {
    [TestFixture]
    public class GameTests {

        [Test]
        public void TestPlayerIdentity() {
            const int size = 7;
            var (spec1, spec2) = TestUtil.DummyPlayerSpecPair(size);
            var gb = GameBoard.Create(7, spec1, spec2);

            var g = GameState.InitialState(gb);
            Assert.AreSame(spec1.Player, g.Player1);
            Assert.AreSame(spec2.Player, g.Player2);
        }

    }
}
