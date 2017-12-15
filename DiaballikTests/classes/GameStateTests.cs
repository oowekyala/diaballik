using Diaballik.Core;
using NUnit.Framework;

namespace Diaballik.Tests {
    [TestFixture]
    public class GameStateTests {
        [Test]
        public void TestPlayerIdentity() {
            const int size = 7;
            var (spec1, spec2) = TestUtil.DummyPlayerSpecPair(size);

            var g = GameState.InitialState(7, (spec1, spec2), true);
            Assert.AreSame(spec1.Player, g.Player1);
            Assert.AreSame(spec2.Player, g.Player2);
        }

    }
}
