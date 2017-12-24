using System.Collections.Generic;
using System.Linq;


namespace Diaballik.Core {
    /// <summary>
    ///     Summarises the initial config of the pieces of a player.
    /// </summary>
    public class PlayerBoardSpec {
        private readonly List<Position2D> _positions;


        /// <summary>
        ///     Create a new spec.
        /// </summary>
        /// <param name="positions">The positions of all the pieces</param>
        /// <param name="ballIndex">The index of the piece which carries the ball in positions</param>
        public PlayerBoardSpec(IEnumerable<Position2D> positions, int ballIndex) {
            _positions = positions.ToList();
            BallIndex = ballIndex;
        }

        protected bool Equals(PlayerBoardSpec other) {
            return _positions.SequenceEqual(other._positions) && BallIndex == other.BallIndex;
        }

        // equality members

        public override bool Equals(object obj) {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((PlayerBoardSpec) obj);
        }

        public override int GetHashCode() {
            unchecked {
                return (_positions.GetHashCode() * 397) ^ BallIndex;
            }
        }

        public static bool operator ==(PlayerBoardSpec left, PlayerBoardSpec right) {
            return Equals(left, right);
        }

        public static bool operator !=(PlayerBoardSpec left, PlayerBoardSpec right) {
            return !Equals(left, right);
        }


        /// The positions of all the pieces
        public IList<Position2D> Positions => _positions;

        /// The index of the piece which carries the ball in the positions
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


        /// The player
        public IPlayer Player { get; }

        // equality members

        protected bool Equals(FullPlayerBoardSpec other) {
            return base.Equals(other) && Player.Equals(other.Player);
        }

        public override bool Equals(object obj) {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((FullPlayerBoardSpec) obj);
        }

        public override int GetHashCode() {
            unchecked {
                return (base.GetHashCode() * 397) ^ Player.GetHashCode();
            }
        }

        public static bool operator ==(FullPlayerBoardSpec left, FullPlayerBoardSpec right) {
            return Equals(left, right);
        }

        public static bool operator !=(FullPlayerBoardSpec left, FullPlayerBoardSpec right) {
            return !Equals(left, right);
        }
    }
}