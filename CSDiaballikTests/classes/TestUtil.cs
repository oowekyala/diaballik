using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace CSDiaballik.Tests
{
    /// <summary>
    ///     Utility methods to mock objects.
    /// </summary>
    public static class TestUtil
    {
        private static readonly Random rng = new Random();


        public static IEnumerable<Position2D> RandomPositions(int n, int boardSize)
        {
            var pos = OrderedPositionsPool(boardSize).ToList();
            if (pos.Count < n)
            {
                throw new InvalidOperationException("Not enough positions on the board "
                                                    + "(" + pos.Count + ", need " + n + ")");
            }

            return pos.Shuffle().Take(n);
        }


        // Implicitly uses the board size as the number of pieces
        public static IEnumerable<Position2D> RandomPositions(int n) => RandomPositions(n, n);


        internal static IEnumerable<Position2D> OrderedPositionsPool(int boardSize)
        {
            return from col in Enumerable.Range(0, boardSize)
                   from row in Enumerable.Range(0, boardSize)
                   select new Position2D(col, row);
        }


        public static (IEnumerable<Position2D>, IEnumerable<Position2D>) RandomPositionsPair(int n, int boardSize)
        {
            var pool = RandomPositions(2 * n, boardSize).ToList();
            return (pool.GetRange(0, n), pool.GetRange(n, n));
        }


        public static (IEnumerable<Position2D>, IEnumerable<Position2D>) RandomPositionsPair(int n)
            => RandomPositionsPair(n, n);


        private static IEnumerable<T> Shuffle<T>(this IList<T> list)
        {
            for (var i = 0; i < list.Count; i++)
            {
                var j = rng.Next(i, list.Count);
                yield return list[j];

                list[j] = list[i];
            }
        }


        public static IEnumerable<int> RandomInts(int size, int bound)
        {
            if (size-- > 0)
            {
                yield return rng.Next(bound);
            }
        }


        public static Color RandomColor() => Color.FromArgb(rng.Next(256), rng.Next(256), rng.Next(256));


        public static IPlayer DummyPlayer(Color c, string name) => new NoobAiPlayer(c, name);


        public static (FullPlayerBoardSpec, FullPlayerBoardSpec) DummyPlayerSpecPair(int boardSize)
        {
            var positions = RandomPositionsPair(boardSize);

            var sp1 = new FullPlayerBoardSpec(DummyPlayer(), positions.Item1, boardSize / 2);
            var sp2 = new FullPlayerBoardSpec(DummyPlayer(), positions.Item2, boardSize / 2);

            return (sp1, sp2);
        }


        public static FullPlayerBoardSpec DummyPlayerSpec(int boardSize)
            => new FullPlayerBoardSpec(DummyPlayer(), RandomPositions(boardSize), boardSize / 2);


        public static FullPlayerBoardSpec DummyPlayerSpec(int boardSize, IEnumerable<Position2D> positions)
            => new FullPlayerBoardSpec(DummyPlayer(), positions, boardSize / 2);


        public static IPlayer DummyPlayer() => new NoobAiPlayer(RandomColor(), "dummy" + rng.Next(100));


    }
}
