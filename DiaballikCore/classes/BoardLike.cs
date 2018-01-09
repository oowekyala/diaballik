using System;
using System.Collections.Generic;
using System.Linq;
using Diaballik.Core.Util;

namespace Diaballik.Core {
    /// <summary>
    ///     Object that has the characteristics of a board. Enriches the interface of GameBoard
    ///     using few abstract methods. Allows to sharee functionality between GameBoard and GameState
    ///     without breaking encapsulation.
    /// </summary>
    public abstract class BoardLike {
        #region Abstract members

        public abstract int BoardSize { get; }

        public abstract Player Player1 { get; }
        public abstract Player Player2 { get; }

        public abstract IEnumerable<Position2D> Player1Positions { get; }
        public abstract IEnumerable<Position2D> Player2Positions { get; }

        public abstract Position2D BallCarrier1 { get; }
        public abstract Position2D BallCarrier2 { get; }

        /// <summary>
        ///     Returns true if the position has no piece on it.
        ///     Doesn't check for validity of the position.
        /// </summary>
        public abstract bool IsFree(Position2D p);

        /// <summary>
        ///     Returns the player at the specified position.
        ///     Returns null if the tile is empty or out of bounds.
        /// </summary>
        /// <returns>The player, or null</returns>
        public abstract Player PlayerOn(Position2D p);

        #endregion

        #region Added properties

        /// <summary>
        ///     An ordered pair of the positions of the pieces of each player.
        /// </summary>
        public (IEnumerable<Position2D>, IEnumerable<Position2D>) PositionsPair => (Player1Positions, Player2Positions);

        /// <summary>
        ///     An ordered pair of the positions of the pieces which carry the 
        ///     ball for each player.
        /// </summary>
        public (Position2D, Position2D) BallCarrierPair => (BallCarrier1, BallCarrier2);


        // do *not* change that to a nullable value tuple, it causes reference errors with the cpp lib
        private (FullPlayerBoardSpec, FullPlayerBoardSpec) _specs;
        private bool _specsEmpty = true;

        /// <summary>
        ///     An ordered pair of player board specs, to describe concisely the full state of this boardlike.
        /// </summary>
        public (FullPlayerBoardSpec, FullPlayerBoardSpec) SpecPair {
            get {
                if (_specsEmpty) {
                    _specs = PositionsPair.Map(Enumerable.ToList)
                                          .Zip(BallCarrierPair,
                                               (ps, bc) => new PlayerBoardSpec(ps, ps.IndexOf(bc)))
                                          .Zip((Player1, Player2),
                                               (spec, p) => new FullPlayerBoardSpec(p, spec));
                    _specsEmpty = false;
                }

                return _specs;
            }
        }


        private bool? _isVictory = null;

        /// True if one player is victorious.
        public bool IsVictory {
            get {
                if (_isVictory.HasValue) return _isVictory.Value;
                ComputeVictory();
                return _isVictory.Value;
            }
        }

        private Player _victoriousPlayer;

        /// The victorious player, or null if there isn't one.
        public Player VictoriousPlayer {
            get {
                if (_isVictory.HasValue) return _victoriousPlayer;
                ComputeVictory();
                return _victoriousPlayer;
            }
        }

        /// Sets values for IsVictory and VictoriousPlayer via side effects.
        private void ComputeVictory() {
            if (IsVictoriousPlayer(Player1)) {
                _victoriousPlayer = Player1;
                _isVictory = true;
            } else if (IsVictoriousPlayer(Player2)) {
                _victoriousPlayer = Player2;
                _isVictory = true;
            } else {
                _isVictory = false;
            }
        }

        /// <summary>
        ///     Returns true if the given player is victorious in the current
        ///     configuration.
        /// </summary>
        private bool IsVictoriousPlayer(Player player) {
            return BallCarrierForPlayer(player).X == BoardSize - 1 - GetRowIndexOfInitialLine(player);
        }

        #endregion

        #region Properties depending on the player

        private T GetPlayerProperty<T>(Player player, (T, T) choices) {
            return player == Player1
                ? choices.Item1
                : player == Player2
                    ? choices.Item2
                    : throw new ArgumentException("Unknown player");
        }

        /// <summary>
        ///     Gets the positions of the pieces of a player.
        /// </summary>
        /// <exception cref="ArgumentException">If the player is unknown</exception>
        public IEnumerable<Position2D> PositionsForPlayer(Player player) {
            return GetPlayerProperty(player, PositionsPair);
        }


        /// <summary>
        ///     Gets the position of the piece carrying the ball of a player.
        /// </summary>
        /// <exception cref="ArgumentException">If the player is not recognised</exception>
        public Position2D BallCarrierForPlayer(Player player) {
            return GetPlayerProperty(player, BallCarrierPair);
        }


        /// <summary>
        ///     Returns the opponent of the specified player.
        /// </summary>
        /// <exception cref="ArgumentException">If the player is unknown</exception>
        public Player GetOtherPlayer(Player player) {
            return GetPlayerProperty(player, (Player2, Player1));
        }


        /// Gets the index of the starting row of a player. Used to check for victory
        public int GetRowIndexOfInitialLine(Player player) {
            return GetPlayerProperty(player, (BoardSize - 1, 0));
        }

        #endregion

        #region Other methods

        /// <summary>
        ///     Returns true if the position is on the board.
        /// </summary>
        public bool IsOnBoard(Position2D p) {
            return p.X >= 0 && p.X < BoardSize
                   && p.Y >= 0 && p.Y < BoardSize;
        }

        /// <summary>
        ///     Returns true if the position is that of one of the
        ///     ball carriers.
        /// </summary>
        public bool HasBall(Position2D p) {
            return p == BallCarrier1 || p == BallCarrier2;
        }


        /// <summary>
        ///     Returns true if there is a piece-free vertical, horizontal, or diagonal line
        ///     between the two positions on the Board.
        /// </summary>
        /// <returns>True if there is a free line between the specified positions</returns>
        /// <exception cref="ArgumentException">If the positions are identical</exception>
        public bool IsLineFreeBetween(Position2D p1, Position2D p2) {
            var dX = p2.X - p1.X;
            var dY = p2.Y - p1.Y;
            var deltaX = Math.Abs(dX);
            var deltaY = Math.Abs(dY);

            DiaballikUtil.Assert(deltaX != 0 || deltaY != 0, "Illegal: cannot move to the same piece");


            if (deltaX <= 1 && deltaY <= 1) {
                return true; // pieces are side by side
            }

            // neither straight line nor diagonal
            if (deltaX != 0 && deltaY != 0 && deltaX != deltaY) {
                return false;
            }

            // compute the range only if count > 0. Otherwise we won't use the value anyway
            var ys = deltaY > 0 ? Enumerable.Range(Math.Min(p1.Y, p2.Y) + 1, deltaY - 1) : null;
            var xs = deltaX > 0 ? Enumerable.Range(Math.Min(p1.X, p2.X) + 1, deltaX - 1) : null;

            if (deltaX == 0) {
                // same row
                return ys.Select(y => new Position2D(p1.X, y)).All(IsFree);
            }

            if (deltaY == 0) {
                // same column
                return xs.Select(x => new Position2D(x, p1.Y)).All(IsFree);
            }

            // we have to reverse one list in two quadrants, otherwise the zipped positions cross the diagonal
            return deltaX == deltaY && xs.Zip(dX * dY > 0 ? ys : ys.Reverse(), Position2D.New).All(IsFree); // diagonal
        }

        #endregion
    }

    /// <summary>
    ///     Abstract class for board like decorators. Mainly used to extract all that boilerplate
    ///     from GameState.
    /// </summary>
    /// <typeparam name="T">Concrete type of board this decorator decorates</typeparam>
    public abstract class BoardLikeDecorator<T> : BoardLike where T : BoardLike {
        #region Protected property

        protected readonly T UnderlyingBoard;

        #endregion

        #region Constructor

        protected BoardLikeDecorator(T underlyingBoard) {
            UnderlyingBoard = underlyingBoard;
        }

        #endregion

        #region Delegated members

        public override IEnumerable<Position2D> Player1Positions => UnderlyingBoard.Player1Positions;
        public override IEnumerable<Position2D> Player2Positions => UnderlyingBoard.Player2Positions;
        public override int BoardSize => UnderlyingBoard.BoardSize;
        public override Player Player1 => UnderlyingBoard.Player1;
        public override Player Player2 => UnderlyingBoard.Player2;
        public override Position2D BallCarrier1 => UnderlyingBoard.BallCarrier1;
        public override Position2D BallCarrier2 => UnderlyingBoard.BallCarrier2;
        public override bool IsFree(Position2D p) => UnderlyingBoard.IsFree(p);
        public override Player PlayerOn(Position2D p) => UnderlyingBoard.PlayerOn(p);

        #endregion
    }
}