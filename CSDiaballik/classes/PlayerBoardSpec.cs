using System.Collections.Generic;
using System.Linq;

namespace CSDiaballik
{
    /// <summary>
    ///     Summarises the initial config of the pieces of a player.
    /// </summary>
    public class PlayerBoardSpec
    {
        private readonly List<Position2D> _positions;


        /// <summary>
        ///     Create a new spec.
        /// </summary>
        /// <param name="positions">The positions of all the pieces</param>
        /// <param name="ball">The index of the ball bearer piece in the positions</param>
        public PlayerBoardSpec(IEnumerable<Position2D> positions, int ball)
        {
            _positions = positions.ToList();
            Ball = ball;
        }


        /// <summary>
        ///     The positions of all the pieces
        /// </summary>
        public IEnumerable<Position2D> Positions => _positions;

        /// <summary>
        ///     The index of the ball bearer piece in the positions
        /// </summary>
        public int Ball { get; }
    }


    /// <summary>
    ///     A board spec bound to its player.
    /// </summary>
    public class FullPlayerBoardSpec : PlayerBoardSpec
    {
        public FullPlayerBoardSpec(IPlayer player, IEnumerable<Position2D> positions, int ball) : base(positions, ball)
        {
            Player = player;
        }


        public FullPlayerBoardSpec(IPlayer player, PlayerBoardSpec spec) : this(player, spec.Positions, spec.Ball)
        {
        }


        /// <summary>
        ///     The player.
        /// </summary>
        public IPlayer Player { get; }
    }
}
