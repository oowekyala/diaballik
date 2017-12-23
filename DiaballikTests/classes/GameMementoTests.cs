using Diaballik.Core;
using NUnit.Framework;

namespace Diaballik.Tests {
    public class GameMementoTest {

        [Test]
        public void TestRootIdentity([Range(5, 15, 2)] int size) {
            var specs = TestUtil.DummyPlayerSpecPair(size);
            var game = Game.Init(size, specs, true);

            Assert.AreEqual(game.State, game.Memento.ToGame());
            Assert.AreEqual(game.State, new RootMemento(specs, size, true).ToGame());
        }



    }
}