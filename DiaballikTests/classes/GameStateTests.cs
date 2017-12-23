using System.Collections.Generic;
using System.Linq;
using Diaballik.AlgoLib;
using Diaballik.Core;
using Diaballik.Core.Util;
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

        private static IEnumerable<UpdateAction> GetMoves(GameState b) {
            return b.PositionsForPlayer(b.CurrentPlayer)
                    .SelectMany(b.AvailableMoves)
                    .Cast<UpdateAction>()
                    .Union(new[] {new PassAction()});
        }

        [Test]
        public void TestChangePlayerTransition([Range(7, 15, 2)] int size) {
            var g0 = TestUtil.WhateverState(size, 2);
            var move = TestUtil.GetAMove(g0);
            var g1 = move.UpdateState(g0);
            AssertThatPlayerHasChanged(g0, g1);
        }

        [Test]
        public void TestGameStateTransition([Range(5, 17, 2)] int size) {
            // get a state with a random history
            var g0 = TestUtil.WhateverState(size);

            foreach (var move in GetMoves(g0)) {
                switch (move) {
                    case MoveBallAction mba:
                        AssertMoveBallSucceeded(g0, g0.MoveBall(mba.Src, mba.Dst), mba);
                        Assert.AreEqual(g0.MoveBall(mba.Src, mba.Dst), mba.UpdateState(g0));
                        break;
                    case MovePieceAction mpa:
                        AssertMovePieceSucceeded(g0, g0.MovePiece(mpa.Src, mpa.Dst), mpa);
                        Assert.AreEqual(g0.MovePiece(mpa.Src, mpa.Dst), mpa.UpdateState(g0));
                        break;
                    case PassAction pa:
                        AssertPassSucceeded(g0, g0.Pass());
                        Assert.AreEqual(g0.Pass(), pa.UpdateState(g0));
                        break;
                }
            }
        }


        // [R21_14_GAME_TURN_AUTO]
        // When his three actions are performed, the game shall automatically allow the next player to play.
        private static void AssertThatPlayerHasChanged(GameState before, GameState after) {
            // asserts that the player has effectively changed
            Assert.AreEqual(before.GetOtherPlayer(before.CurrentPlayer), after.CurrentPlayer);
            Assert.AreEqual(3, after.NumMovesLeft);
        }

        private static void AssertWhetherPlayerHasChanged(GameState before, GameState after) {
            if (before.NumMovesLeft == 1) {
                // player has changed
                AssertThatPlayerHasChanged(before, after);
            } else {
                Assert.AreEqual(before.CurrentPlayer, after.CurrentPlayer);
                Assert.AreEqual(before.NumMovesLeft - 1, after.NumMovesLeft);
            }
        }

        private static void AssertMovePieceSucceeded(GameState before, GameState after, MovePieceAction action) {
            CollectionAssert.DoesNotContain(after.PositionsForPlayer(before.CurrentPlayer), action.Src);
            CollectionAssert.Contains(after.PositionsForPlayer(before.CurrentPlayer), action.Dst);

            AssertWhetherPlayerHasChanged(before, after);
        }

        private static void AssertMoveBallSucceeded(GameState before, GameState after, MoveBallAction action) {
            CollectionAssert.AreEquivalent(before.PositionsForPlayer(before.CurrentPlayer),
                                           after.PositionsForPlayer(before.CurrentPlayer));
            Assert.AreEqual(before.BallCarrierForPlayer(before.CurrentPlayer), action.Src);
            Assert.AreEqual(after.BallCarrierForPlayer(before.CurrentPlayer), action.Dst);

            AssertWhetherPlayerHasChanged(before, after);
        }

        private static void AssertPassSucceeded(GameState before, GameState after) {
            before.PositionsPair.Zip(after.PositionsPair)
                  .Foreach(t => CollectionAssert.AreEquivalent(t.Item1, t.Item2));
            Assert.AreEqual(before.BallCarrierPair, after.BallCarrierPair);

            AssertThatPlayerHasChanged(before, after);
        }
    }
}