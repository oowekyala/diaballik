using System;
using System.Collections.Generic;
using System.Linq;

namespace CSDiaballik {
    public interface IInitStrategy {

        /// <summary>
        ///     Gets the positions of the pieces of each player.
        /// </summary>
        /// <param name="size">Size of the board (square)</param>
        /// <returns>A tuple containing the positions of the pieces of each player </returns>
        ValueTuple<PlayerBoardSpec, PlayerBoardSpec> InitPositions(int size);

    }

    // We assume that the board's size is odd

    public class StandardInitStrategy : IInitStrategy {

        public ValueTuple<PlayerBoardSpec, PlayerBoardSpec> InitPositions(int size) {
            var pos1 = Enumerable.Range(0, size).Select(i => new Position2D(size - 1, i));
            var pos2 = Enumerable.Range(0, size).Select(i => new Position2D(0, i));

            return (new PlayerBoardSpec(pos1, size / 2), new PlayerBoardSpec(pos2, size / 2));
        }

    }


    public class BallRandomStrategy : IInitStrategy {

        public ValueTuple<PlayerBoardSpec, PlayerBoardSpec> InitPositions(int size) {
            var pos1 = Enumerable.Range(0, size).Select(i => new Position2D(size - 1, i));
            var pos2 = Enumerable.Range(0, size).Select(i => new Position2D(0, i));
            var rand = new Random();

            return (new PlayerBoardSpec(pos1, rand.Next(size)), new PlayerBoardSpec(pos2, rand.Next(size)));
        }

    }


    public class EnemyAmongUsStrategy : IInitStrategy {

        private readonly Random rand = new Random();


        public ValueTuple<PlayerBoardSpec, PlayerBoardSpec> InitPositions(int size) {
            var pos1 = Enumerable.Range(0, size).Select(i => new Position2D(size - 1, i)).ToList();
            var pos2 = Enumerable.Range(0, size).Select(i => new Position2D(0, i)).ToList();

            if (size == 1) {
                throw new ArgumentException("Size is 1"); // infinite loop otherwise
            }

            var n = SwapRandom(size, pos1, pos2);
            SwapRandom(size, pos1, pos2, n);


            return (new PlayerBoardSpec(pos1, size / 2), new PlayerBoardSpec(pos2, size / 2));
        }


        private int SwapRandom(int size, List<Position2D> pos1, List<Position2D> pos2)
            => SwapRandom(size, pos1, pos2, -1);


        private int SwapRandom(int size, List<Position2D> pos1, List<Position2D> pos2, int except) {
            var num = rand.Next(size);
            while (num == size / 2 || num == except) {
                num = rand.Next(size);
            }

            var tmp = pos1[num];
            pos1[num] = pos2[num];
            pos2[num] = tmp;
            return num;
        }

    }
}
