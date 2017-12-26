using System;
using System.Collections.Generic;
using System.Linq;
using Diaballik.Core;
using Diaballik.Players;
using NUnit.Framework;
using static Diaballik.Tests.MockUtil;

namespace Diaballik.Tests {
    public class GameMementoTest {
        [Test]
        public void TestRootIdentity([Range(5, 15, 2)] int size) {
            var specs = DummyPlayerSpecPair(size);
            var game = Game.Init(size, specs, true);

            Assert.AreEqual(game.State, game.Memento.ToState());
            Assert.AreEqual(game.State, new RootMemento(specs, size, true).ToState());
        }


        private static IEnumerable<GameMemento> TestCasesProvider() {
            // base test cases, generated randomly
            var gs = Enumerable.Range(5, 16)
                               .Where(x => x % 2 != 0)
                               .SelectMany(x => Generate(() => AnyGame(x)).Take(5))
                               .Select(g => g.Memento).ToList();
            // adding an undo action on some of them
            return gs.Concat(gs.Where(g => new UndoAction().IsValidOn(g.ToState()))
                               .Select(g => g.Append(new UndoAction())).Take(15));
        }


        [Test, TestCaseSource(sourceName: nameof(TestCasesProvider))]
        public void TestDeconstructReconstruct(GameMemento memento) {
            var (root, nodes) = memento.Deconstruct();

            var st = root.ToState(); // initial state
            var pred1 = st;
            var pred2 = st;

            foreach (var node in nodes) {
                switch (node) {
                    case ActionMementoNode action:
                        pred2 = pred1;
                        pred1 = st;
                        st = ((IUpdateAction) action.Action).UpdateState(st);
                        break;
                    case UndoMementoNode undo:
                        st = pred1;
                        pred1 = pred2;
                        break;
                }
                Assert.AreEqual(node.ToState(), st);
            }
        }

        [Test, TestCaseSource(sourceName: nameof(TestCasesProvider))]
        public void TestReadWriteRoundTrip(GameMemento memento) {
            var serializer = new MementoSerializationUtil.Serializer();
            var deserializer = new MementoSerializationUtil.Deserializer();

            Assert.AreEqual(memento, deserializer.FromDocument(serializer.ToXml(memento)));
        }

        [TearDown]
        public void PrintStats() {
            MockUtil.PrintStats();
        }
    }
}