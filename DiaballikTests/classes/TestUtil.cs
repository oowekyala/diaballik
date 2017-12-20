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
    ///     Utility methods to mock objects.
    /// </summary>
    public static class TestUtil {
        public static readonly Random Rng = new Random();


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


        public static Color RandomColor() => Color.FromArgb(Rng.Next(256), Rng.Next(256), Rng.Next(256));


        public static IPlayer DummyPlayer(Color c, string name) => new NoobAiPlayer(c, name);


        public static (FullPlayerBoardSpec, FullPlayerBoardSpec) DummyPlayerSpecPair(int boardSize)
            => RandomPositionsPair(boardSize).Map(p => new FullPlayerBoardSpec(DummyPlayer(), p, boardSize / 2));


        public static FullPlayerBoardSpec DummyPlayerSpec(IEnumerable<Position2D> positions, int ballIndex)
            => new FullPlayerBoardSpec(DummyPlayer(), positions, ballIndex);


        public static FullPlayerBoardSpec DummyPlayerSpec(int boardSize, IEnumerable<Position2D> positions)
            => new FullPlayerBoardSpec(DummyPlayer(), positions, boardSize / 2);


        public static IPlayer DummyPlayer() => new NoobAiPlayer(RandomColor(), "dummy" + Rng.Next(100));


        public static UpdateAction GetAMove(GameState state) {
            var ps = state.PositionsForPlayer(state.CurrentPlayer);

            foreach (var p in ps) {
                var moves = state.AvailableMoves(p).ToList();
                if (moves.Any()) {
                    return moves.First();
                }
            }

            return new PassAction();
        }

        public static GameState WhateverState(int size) {
            return WhateverState(size, Rng.Next() % 20);
        }

        public static GameState WhateverState(int size, int historySize) {
            var specs = DummyPlayerSpecPair(size);
            var cur = GameState.InitialState(size, specs, true);

            while (historySize-- > 0) {
                var move = GetAMove(cur);
                cur = move.UpdateState(cur);
            }

            return cur;
        }
    }
}