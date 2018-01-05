using System.Collections.Generic;
using System.Linq;
using Diaballik.Core;
using Diaballik.Mock;
using NUnit.Framework;
using static Diaballik.Core.Util.SerializationUtil;
using static Diaballik.Mock.MockUtil;

namespace Diaballik.Tests {
    [TestFixture]
    public class GameMementoTest {
        [TearDown]
        public void PrintStats() {
            MockUtil.PrintStats(nameof(GameMementoTest));
        }


        [Test]
        public void TestRootIdentity([Range(5, 15, 2)] int size) {
            var specs = DummyPlayerSpecPair(size);
            var game = Game.Init(size, specs, true);

            Assert.AreEqual(game.State, game.Memento.State);
            Assert.AreEqual(game.State, new RootMemento(specs, size, true).State);
        }


        private static IEnumerable<GameMemento> SerializationTestCasesProvider() {
            return Generate(AnyGame).Select(g => g.Memento).Take(100);
        }


        [Test, TestCaseSource(sourceName: nameof(SerializationTestCasesProvider))]
        public void TestReadWriteRoundTrip(GameMemento memento) {
            Assert.AreEqual(memento, Deserializer.MementoFromElement(Serializer.MementoToElement(memento)));
        }
    }
}