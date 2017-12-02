using System;
using System.Collections.Generic;
using System.Linq;

namespace CSDiaballik
{
    public interface IInitStrategy
    {
        /// <summary>
        ///     Gets the positions of the pieces of each player.
        /// </summary>
        /// <param name="size">Size of the board (square)</param>
        /// <param name="p1">First player</param>
        /// <param name="p2">Second player</param>
        /// <returns>A tuple containing the positions of the pieces of each player </returns>
        ValueTuple<PlayerBoardSpec, PlayerBoardSpec> InitPositions(int size, PlayerBuilder p1, PlayerBuilder p2);
    }

    /// <summary>
    ///     Summarises the initial config of the pieces of a player.
    /// </summary>
    public struct PlayerBoardSpec
    {
        public IEnumerable<Position2D> Positions { get; }
        public int Ball { get; }


        public PlayerBoardSpec(IEnumerable<Position2D> positions, int ball)
        {
            Positions = positions;
            Ball = ball;
        }
    }


    public class StandardInitStrategy : IInitStrategy
    {
        public ValueTuple<PlayerBoardSpec, PlayerBoardSpec> InitPositions(int size, PlayerBuilder p1, PlayerBuilder p2)
        {
            var pos1 = Enumerable.Range(0, size).Select(i => new Position2D(size - 1, i));
            var pos2 = Enumerable.Range(0, size).Select(i => new Position2D(0, i));

            return (new PlayerBoardSpec(pos1, size / 2), new PlayerBoardSpec(pos2, size / 2));
        }
    }


    public class BallRandomStrategy : IInitStrategy
    {
        public ValueTuple<PlayerBoardSpec, PlayerBoardSpec> InitPositions(int size, PlayerBuilder p1, PlayerBuilder p2)
        {
            var pos1 = Enumerable.Range(0, size).Select(i => new Position2D(size - 1, i));
            var pos2 = Enumerable.Range(0, size).Select(i => new Position2D(0, i));
            var rand = new Random();

            return (new PlayerBoardSpec(pos1, rand.Next(size)), new PlayerBoardSpec(pos2, rand.Next(size)));
        }
    }

    public class EnemyAmongUsStrategy : IInitStrategy
    {
        public ValueTuple<PlayerBoardSpec, PlayerBoardSpec> InitPositions(int size, PlayerBuilder p1, PlayerBuilder p2)
        {
            var pos1 = Enumerable.Range(0, size).Select(i => new Position2D(size - 1, i)).ToList();
            var pos2 = Enumerable.Range(0, size).Select(i => new Position2D(0, i)).ToList();

            if (size == 1)
            {
                throw new ArgumentException("Size is 1"); // infinite loop otherwise
            }

            SwapRandom(size, pos1, pos2);
            SwapRandom(size, pos1, pos2);


            return (new PlayerBoardSpec(pos1, size / 2), new PlayerBoardSpec(pos2, size / 2));
        }


        private static void SwapRandom(int size, IList<Position2D> pos1, IList<Position2D> pos2)
        {
            var rand = new Random();
            var num = rand.Next(size);
            while (num == size / 2)
            {
                num = rand.Next(size);
            }

            var tmp = pos1[num];
            pos1[num] = pos2[num];
            pos2[num] = tmp;
        }
    }
}
