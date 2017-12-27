using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Diaballik.AlgoLib;
using Diaballik.Core;
using Diaballik.Core.Util;
using Diaballik.Players;

namespace Diaballik.Tests {
    /// <summary>
    ///     Utility methods to mock model objects.
    /// </summary>
    public static class MockUtil {
        #region Position mocking

        public static IEnumerable<Position2D> RandomPositions(int n, int boardSize) {
            var pos = OrderedPositionsPool(boardSize).ToList();
            if (pos.Count < n) {
                throw new InvalidOperationException("Not enough positions on the board "
                                                    + "(" + pos.Count + ", need " + n + ")");
            }

            return pos.Shuffle().Take(n);
        }


        // Implicitly uses the board size as the number of pieces
        public static IEnumerable<Position2D> RandomPositions(int n) => RandomPositions(n, n);


        internal static IEnumerable<Position2D> OrderedPositionsPool(int boardSize)
            => from col in Enumerable.Range(0, boardSize)
                from row in Enumerable.Range(0, boardSize)
                select new Position2D(col, row);


        public static (IEnumerable<Position2D>, IEnumerable<Position2D>) RandomPositionsPair(int n, int boardSize) {
            var pool = RandomPositions(2 * n, boardSize).ToList();
            return (pool.GetRange(0, n), pool.GetRange(n, n));
        }


        public static (IEnumerable<Position2D>, IEnumerable<Position2D>) RandomPositionsPair(int n)
            => RandomPositionsPair(n, n);


        private static IEnumerable<T> Shuffle<T>(this IList<T> list) {
            for (var i = 0; i < list.Count; i++) {
                var j = Rng.Next(i, list.Count);
                yield return list[j];

                list[j] = list[i];
            }
        }

        #endregion

        #region Player mocking

        public static Color RandomColor() => Color.FromArgb(Rng.Next(256), Rng.Next(256), Rng.Next(256));

        public static (FullPlayerBoardSpec, FullPlayerBoardSpec) DummyPlayerSpecPair(int boardSize)
            => RandomPositionsPair(boardSize).Map(p => new FullPlayerBoardSpec(DummyPlayer(), p, boardSize / 2));


        public static FullPlayerBoardSpec DummyPlayerSpec(IEnumerable<Position2D> positions, int ballIndex)
            => new FullPlayerBoardSpec(DummyPlayer(), positions, ballIndex);


        public static FullPlayerBoardSpec DummyPlayerSpec(int boardSize, IEnumerable<Position2D> positions)
            => new FullPlayerBoardSpec(DummyPlayer(), positions, boardSize / 2);


        public static IPlayer DummyPlayer() => DummyPlayer(RandomColor(), "dummy" + Rng.Next(100));


        /// Returns a dummy player of any type
        public static IPlayer DummyPlayer(Color c, string name) {
            var random = Rng.Next(100);

            if (random < 33) {
                return new NoobAiPlayer(c, name);
            }
            if (random < 66) {
                return new StartingAiPlayer(c, name);
            }
            return new ProgressiveAiPlayer(c, name);
        }

        #endregion

        #region Move mocking

        /// Used as source for randomly chosen moves.
        private static readonly NoobAiAlgo MoveDecisionAlgo = new NoobAiAlgo();

        /// <summary>
        ///     Gets a random move that's valid on the given state.
        /// 
        ///     The distribution of moves is tweaked to reduce bias towards MovePiece action
        ///     and represent each type of move realistically, if not evenly. 
        /// 
        ///     See the implementation in NoobAiAlgo.cpp
        /// </summary>
        public static IUpdateAction GetAMove(GameState state) {
            return MoveDecisionAlgo.NextMove(state, state.CurrentPlayer).UpdateStatisticsAndPass();
        }


        #region Statistics collection

        public static int MoveBalls;
        public static int MovePieces;
        public static int Passes;
        public static int Undos;
        public static int Redos;

        /// Updates the creation statistics of moves
        private static IUpdateAction UpdateStatisticsAndPass(this IUpdateAction action) {
            switch (action) {
                case MoveBallAction moveBallAction:
                    MoveBalls++;
                    break;
                case MovePieceAction movePieceAction:
                    MovePieces++;
                    break;
                case PassAction passAction:
                    Passes++;
                    break;
            }
            return action;
        }


        public static void ResetStats() {
            MoveBalls = MovePieces = Passes = Undos = Redos = 0;
        }

        public static void PrintStats(string title) {
            Console.WriteLine($"Stats for {title}:");
            Console.WriteLine($"MovePieces: {MovePieces}");
            Console.WriteLine($"MoveBalls: {MoveBalls}");
            Console.WriteLine($"Passes: {Passes}");
            Console.WriteLine($"Undos: {Undos}");
            Console.WriteLine($"Redos: {Redos}");
        }

        #endregion

        #endregion

        #region State mocking

        /// Gets a state with the given number of moves left.
        /// Effectively gets a random state.
        public static GameState AnyState(int size, int movesLeftCount) {
            var specs = DummyPlayerSpecPair(size);
            var cur = GameState.InitialState(size, specs, true);

            while (cur.NumMovesLeft != movesLeftCount) {
                cur = GetAMove(cur).UpdateState(cur);
            }

            return cur;
        }

        public static GameState AnyState(int size) {
            return AnyState(size, Rng.Next(1, Game.MaxMovesPerTurn + 1));
        }

        #endregion

        #region Game mocking

        public static Game AnyGame() {
            var rand = Rng.Next(5, 25);
            return AnyGame(rand % 2 == 0 ? rand + 1 : rand);
        }

        public static Game AnyGame(int size) {
            return AnyGame(size, Rng.Next(0, 40));
        }


        /// Constructs a game from a random initial state and updating it with valid moves
        public static Game AnyGame(int size, int historySize) {
            var specs = DummyPlayerSpecPair(size);
            var game = Game.Init(size, specs);

            while (historySize-- > 0) {
                game.Update(GetAMove(game.State));
            }

            return game;
        }

        #endregion

        #region Miscellaneous utilities

        public static readonly Random Rng = new Random();


        /// Generates an infinite sequence from a generator function.
        public static IEnumerable<T> Generate<T>(Func<T> f) {
            while (true) yield return f();
        }

        #endregion
    }
}