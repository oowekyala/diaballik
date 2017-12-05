using System.Collections.Generic;
using System.Linq;

namespace CSDiaballik {
    /// <summary>
    ///     Summarises the initial config of the pieces of a player.
    /// </summary>
    public class PlayerBoardSpec {

        private readonly List<Position2D> _positions;


        /// <summary>
        ///     Create a new spec.
        /// </summary>
        /// <param name="positions">The positions of all the pieces</param>
        /// <param name="ballIndex">The index of the ball bearer piece in the positions</param>
        public PlayerBoardSpec(IEnumerable<Position2D> positions, int ballIndex) {
            _positions = positions.ToList();
            BallIndex = ballIndex;
        }


        /// <summary>
        ///     The positions of all the pieces
        /// </summary>
        public IEnumerable<Position2D> Positions => _positions;

        /// <summary>
        ///     The index of the ball bearer piece in the positions
        /// </summary>
        public int BallIndex { get; }

    }


    /// <inheritdoc />
    /// <summary>
    ///     A board spec bound to its player.
    /// </summary>
    public class FullPlayerBoardSpec : PlayerBoardSpec {

        public FullPlayerBoardSpec(IPlayer player, IEnumerable<Position2D> positions, int ballIndex) :
            base(positions, ballIndex) {
            Player = player;
        }


        public FullPlayerBoardSpec(IPlayer player, PlayerBoardSpec spec) :
            this(player, spec.Positions, spec.BallIndex) {
        }


        /// The player.
        public IPlayer Player { get; }

    }
}
