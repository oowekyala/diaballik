using System;
using System.Collections.Generic;
using System.Linq;


namespace Diaballik.Core {
    public interface IInitStrategy {
        /// <summary>
        ///     Gets the positions of the pieces of each player.
        /// </summary>
        /// <param name="size">Size of the board (square)</param>
        /// <returns>A tuple containing the positions of the pieces of each player </returns>
        (PlayerBoardSpec, PlayerBoardSpec) InitPositions(int size);
    }


    public abstract class AbstractInitStrategy : IInitStrategy {
        protected static readonly Random Rand = new Random();


        public abstract (PlayerBoardSpec, PlayerBoardSpec) InitPositions(int size);


        /// <summary>
        /// Gets a tuple of positions corresponding to the default positions of each player,
        /// that is, that is, each player has the top or bottom row, according to the convention
        /// defined in <see cref="GameBoard"/>.
        /// </summary>
        /// <param name="size">The size of the board</param>
        /// <returns></returns>
        protected static (IEnumerable<Position2D>, IEnumerable<Position2D>) InitialPositions(int size)
            => (size - 1, 0).ZipWithPair(Enumerable.Range(0, size), (x, ys) => ys.Select(y => new Position2D(x, y)));
    }


    // We assume that the board's size is odd and > 1


    public class StandardInitStrategy : AbstractInitStrategy {
        public override (PlayerBoardSpec, PlayerBoardSpec) InitPositions(int size) {
            return InitialPositions(size).Map(ps => new PlayerBoardSpec(ps, size / 2));
        }
    }


    public class BallRandomStrategy : AbstractInitStrategy {
        public override (PlayerBoardSpec, PlayerBoardSpec) InitPositions(int size) {
            return InitialPositions(size).Map(ps => new PlayerBoardSpec(ps, Rand.Next(size)));
        }
    }


    public class EnemyAmongUsStrategy : AbstractInitStrategy {
        public override (PlayerBoardSpec, PlayerBoardSpec) InitPositions(int size) {
            var (pos1, pos2) = InitialPositions(size).Map(ps => ps.ToList());

            // swap two positions randomly twice
            SwapRandom(0, size / 2, pos1, pos2);
            SwapRandom(1 + size / 2, size, pos1, pos2);

            return (pos1, pos2).Map(ps => new PlayerBoardSpec(ps, size / 2));
        }


        private static void SwapRandom(int start, int end, IList<Position2D> pos1, IList<Position2D> pos2) {
            var num = Rand.Next(start, end);
            var tmp = pos1[num];
            pos1[num] = pos2[num];
            pos2[num] = tmp;
        }
    }
}