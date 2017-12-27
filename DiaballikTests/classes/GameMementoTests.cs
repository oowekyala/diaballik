﻿using System.Collections.Generic;
using System.Linq;
using Diaballik.Core;
using Diaballik.Players;
using NUnit.Framework;
using static Diaballik.Tests.MockUtil;

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

            Assert.AreEqual(game.State, game.Memento.ToState());
            Assert.AreEqual(game.State, new RootMemento(specs, size, true).ToState());
        }


        private static IEnumerable<GameMemento> SerializationTestCasesProvider() {
            return Generate(AnyGame).Select(g => g.Memento).Take(100);
        }


        [Test, TestCaseSource(sourceName: nameof(SerializationTestCasesProvider))]
        public void TestReadWriteRoundTrip(GameMemento memento) {
            var serializer = new MementoSerializationUtil.Serializer();
            var deserializer = new MementoSerializationUtil.Deserializer();

            Assert.AreEqual(memento, deserializer.FromDocument(serializer.ToXml(memento)));
        }
    }
}