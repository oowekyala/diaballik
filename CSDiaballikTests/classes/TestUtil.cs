﻿using System;
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


        public static ValueTuple<IEnumerable<Position2D>, IEnumerable<Position2D>>
            RandomPositionsPair(int n, int boardSize)
        {
            var pool = RandomPositions(2 * n, boardSize).ToList();
            return (pool.GetRange(0, n), pool.GetRange(n, n));
        }


        public static ValueTuple<IEnumerable<Position2D>, IEnumerable<Position2D>> RandomPositionsPair(int n)
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


        public static IPlayer DummyPlayer(int boardSize)
            => new NoobAiPlayer(RandomColor(), "dummy" + rng.Next(100), RandomPositions(boardSize, boardSize));


        public static GameBoard DummyBoard()
        {
            var size = rng.Next(100);
            if (size % 2 == 0)
            {
                size++;
            }

            var positions = RandomPositionsPair(size, size);
            var p1 = DummyPlayer(positions.Item1);
            var p2 = DummyPlayer(positions.Item2);

            return new GameBoard(size, p1.Pieces, p2.Pieces);
        }


        public static IPlayer DummyPlayer(Color color, string name, IEnumerable<Position2D> positions)
            => new NoobAiPlayer(color, name, positions);


        public static IPlayer DummyPlayer(IEnumerable<Position2D> positions)
            => new NoobAiPlayer(RandomColor(), "dummy" + rng.Next(100), positions);


        public static List<T> ToList<T>(this ValueTuple<T, T> tuple) => new List<T> {tuple.Item1, tuple.Item2};
    }
}