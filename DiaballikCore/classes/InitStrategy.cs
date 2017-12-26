using System;
using System.Collections.Generic;
using System.Linq;
using Diaballik.Core.Util;


namespace Diaballik.Core {
    using PlayerSpecPair = ValueTuple<PlayerBoardSpec, PlayerBoardSpec>;


    public interface IInitStrategy {
        /// <summary>
        ///     Gets the positions of the pieces of each player.
        /// </summary>
        /// <param name="size">Size of the board (square)</param>
        /// <returns>A tuple containing the positions of the pieces of each player </returns>
        PlayerSpecPair InitPositions(int size);
    }


    public abstract class AbstractInitStrategy : IInitStrategy {
        #region Unimplemented overridden members

        public abstract PlayerSpecPair InitPositions(int size);

        #endregion

        #region Shared behavior

        protected static readonly Random Rand = new Random();

        /// <summary>
        ///     Gets a tuple of positions corresponding to the default positions of each player,
        ///     that is, that is, each player has the top or bottom row, according to the convention
        ///     defined in <see cref="GameBoard"/>.
        /// </summary>
        /// <param name="size">The size of the board</param>
        protected static (IEnumerable<Position2D>, IEnumerable<Position2D>) InitialPositions(int size)
            => (size - 1, 0).ZipWithPair(Enumerable.Range(0, size), (x, ys) => ys.Select(y => new Position2D(x, y)));

        #endregion
    }


    // We assume that the board's size is odd and > 1


    public class StandardInitStrategy : AbstractInitStrategy {
        #region Overridden methods

        public override PlayerSpecPair InitPositions(int size) {
            return InitialPositions(size).Map(ps => new PlayerBoardSpec(ps, size / 2));
        }

        #endregion
    }


    public class BallRandomStrategy : AbstractInitStrategy {
        #region Overridden methods

        public override PlayerSpecPair InitPositions(int size) {
            return InitialPositions(size).Map(ps => new PlayerBoardSpec(ps, Rand.Next(size)));
        }

        #endregion
    }


    public class EnemyAmongUsStrategy : AbstractInitStrategy {
        #region Overridden methods

        public override PlayerSpecPair InitPositions(int size) {
            var (pos1, pos2) = InitialPositions(size).Map(ps => ps.ToList());

            // swap two positions randomly twice
            SwapRandom(0, size / 2, pos1, pos2);
            SwapRandom(1 + size / 2, size, pos1, pos2);

            return (pos1, pos2).Map(ps => new PlayerBoardSpec(ps, size / 2));
        }

        #endregion

        #region Helper methods

        private static void SwapRandom(int start, int end, IList<Position2D> pos1, IList<Position2D> pos2) {
            var num = Rand.Next(start, end);
            var tmp = pos1[num];
            pos1[num] = pos2[num];
            pos2[num] = tmp;
        }

        #endregion
    }
}