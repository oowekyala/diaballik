using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
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


        private static IEnumerable<GameMemento> TestCasesProvider() {
            // base test cases, generated randomly
            var games = Enumerable.Range(5, 16)
                                  .Where(x => x % 2 != 0)
                                  .SelectMany(x => Generate(() => AnyGame(x)).Take(5))
                                  .Select(g => g.Memento).ToList();

            return games;
        }

        private static IEnumerable<GameMemento> UndoTestCasesProvider() {
            return Generate(AnySimpleGame).Where(g => g.CanUndo).Select(g => g.Memento).Take(20);
        }

        [Test, TestCaseSource(sourceName: nameof(UndoTestCasesProvider))]
        public void TestOneUndo(GameMemento memento) {
            Assert.AreEqual(memento.Undo().ToState(), memento.Parent.ToState());
        }


        [Test]
        public void TestChainedUndoRedos1([Range(5, 17, 2)] int size) {
            var mem = Generate(() => AnyGame(size, 5)).First(g => g.CanUndo).Memento;

            // nodes:   a <- b <- c <- u1 <- r1 <- u2 <- u3
            // toState: a    b    c    b     c     b     a

            var a = mem.Parent.Parent.ToState();
            var b = mem.Parent.ToState();
            var c = mem.ToState();
            var u1 = mem.Undo();
            var r = u1.Redo();
            var u2 = r.Undo();
            var u3 = u2.Undo();

            Console.WriteLine(u3.FullAncestryString());

            Assert.AreEqual(b, u1.ToState());
            Assert.AreEqual(c, r.ToState());
            Assert.AreEqual(b, u2.ToState());
            Assert.AreEqual(a, u3.ToState());
        }


        [Test]
        public void TestChainedRedos() {
            var mem = Generate(() => AnySimpleGame(7, 5)).First(g => g.CanUndo).Memento;

            // nodes:   a <- b <- c <- d <- undo <- undo <- undo <- redo <- redo 
            // toState: a    b    c    d    c       b       a       b       c

            var a = mem.GetNthParent(2).ToState();
            var b = mem.GetNthParent(1).ToState();
            var c = mem.GetNthParent(0).ToState();
            var d = mem.ToState();
            var u1 = mem.Undo();
            var u2 = u1.Undo();
            var u3 = u2.Undo();
            var r1 = u3.Redo();
            var r2 = r1.Redo();

            Assert.AreEqual(c, u1.ToState());
            Assert.AreEqual(b, u2.ToState());
            Assert.AreEqual(a, u3.ToState());
            Assert.AreEqual(b, r1.ToState());
            Assert.AreEqual(c, r2.ToState());
        }


        [Test]
        public void TestChainedUndoRedos2() {
            var mem = Generate(() => AnySimpleGame(17, 5)).First(g => g.CanUndo).Memento;

            // nodes:   a <- b <- c <- u1 <- r1 <- u2 <- r2 <- u3 <- u4
            // toState: a    b    c    b     c     b     c     b     a

            var a = mem.Parent.Parent.ToState();
            var b = mem.Parent.ToState();
            var c = mem.ToState();
            var u1 = mem.Undo();
            var r1 = u1.Redo();
            var u2 = r1.Undo();
            var r2 = u2.Redo();
            var u3 = r2.Undo();
            var u4 = u3.Undo();

            Console.WriteLine(u4.FullAncestryString());

            Assert.AreEqual(b, u1.ToState());
            Assert.AreEqual(c, r1.ToState());
            Assert.AreEqual(b, u2.ToState());
            Assert.AreEqual(c, r2.ToState());
            Assert.AreEqual(b, u3.ToState());
            Assert.AreEqual(a, u4.ToState());
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