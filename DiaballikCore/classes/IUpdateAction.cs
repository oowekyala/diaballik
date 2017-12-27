using System;


namespace Diaballik.Core {
    /// <inheritdoc />
    /// <summary>
    ///     Represents an action that can update the game's state. Undo is excluded from this
    ///     category, as it's implemented by state switch in Game.
    /// </summary>
    public interface IUpdateAction : IPlayerAction {
        /// <summary>
        ///     Returns true if the move is valid, in the context of 
        ///     the specified state.
        /// </summary>
        /// <param name="state">The context</param>
        /// <returns>True if the move is valid</returns>
        bool IsValidOn(GameState state);

        /// <summary>
        ///     Updates the given current state with this action.
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        GameState UpdateState(GameState state);
    }

    #region Derived classes

    /// <inheritdoc />
    /// <summary>
    ///     Base class for MovePiece and MoveBall.
    /// </summary>
    public abstract class MoveAction : IUpdateAction {
        #region Properties

        public Position2D Src { get; }
        public Position2D Dst { get; }

        #endregion

        #region Base constructor

        protected MoveAction(Position2D src, Position2D dst) {
            if (src == dst) throw new ArgumentException("Illegal: cannot move to the same piece");
            Src = src;
            Dst = dst;
        }

        #endregion

        #region Unimplemented interface methods

        public abstract GameState UpdateState(GameState state);

        public abstract bool IsValidOn(GameState state);

        #endregion

        #region Methods

        public override string ToString() {
            return $"{GetType().Name}(src: {Src}, dst: {Dst})";
        }

        #endregion

        #region Equality members

        protected bool Equals(MoveAction other) {
            return Src.Equals(other.Src) && Dst.Equals(other.Dst);
        }

        public override bool Equals(object obj) {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((MoveAction) obj);
        }

        public override int GetHashCode() {
            unchecked {
                return (Src.GetHashCode() * 397) ^ Dst.GetHashCode();
            }
        }

        public static bool operator ==(MoveAction left, MoveAction right) {
            return Equals(left, right);
        }

        public static bool operator !=(MoveAction left, MoveAction right) {
            return !Equals(left, right);
        }

        #endregion
    }


    /// <inheritdoc />
    /// <summary>
    ///     Move the ball to another piece.
    /// </summary>
    public class MoveBallAction : MoveAction {
        #region Constructor and factory

        private MoveBallAction(Position2D src, Position2D dst) : base(src, dst) {
        }

        // can be used as method group
        public static MoveBallAction New(Position2D src, Position2D dst) {
            return new MoveBallAction(src, dst);
        }

        #endregion

        #region Overridden methods

        // [R21_9_GAMEPLAY_MOVE_BALL]
        // A ball carried by a piece shall be moved to another piece 
        // of the same player if there is a horizontal, vertical,
        // diagonal piece-free line between these two pieces
        public override bool IsValidOn(GameState state) {
            return state.NumMovesLeft > 0
                   && Equals(state.PlayerOn(Src), state.CurrentPlayer)
                   && Equals(state.PlayerOn(Src), state.PlayerOn(Dst))
                   && state.IsLineFreeBetween(Src, Dst);
        }


        public override GameState UpdateState(GameState state) {
            return state.MoveBall(Src, Dst);
        }

        #endregion
    }


    /// <inheritdoc />
    /// <summary>
    ///     Move a piece to a new location.
    /// </summary>
    public class MovePieceAction : MoveAction {
        #region Constructor and factory

        private MovePieceAction(Position2D src, Position2D dst) : base(src, dst) {
        }

        // can be used as method group
        public static MovePieceAction New(Position2D src, Position2D dst) {
            return new MovePieceAction(src, dst);
        }

        #endregion

        #region Overridden methods

        // [R21_11_GAMEPLAY_MOVE_PIECE_WITH_BALL]
        // A piece shall not move if it carries the ball.
        // [R21_10_GAMEPLAY_MOVE_PIECE]
        // A piece shall be moved to the direct left, right, up, or bottom tile if free.
        public override bool IsValidOn(GameState state) {
            var dX = Math.Abs(Src.X - Dst.X);
            var dY = Math.Abs(Src.Y - Dst.Y);

            return state.NumMovesLeft > 0
                   && (dX == 1 && dY == 0 || dX == 0 && dY == 1)
                   && state.IsFree(Dst)
                   && Equals(state.PlayerOn(Src), state.CurrentPlayer)
                   && !state.HasBall(Src);
        }


        public override GameState UpdateState(GameState state) {
            return state.MovePiece(Src, Dst);
        }

        #endregion
    }


    /// <inheritdoc />
    /// <summary>
    ///     End the turn and give initiative to the other player prematurely.
    /// </summary>
    public class PassAction : IUpdateAction {
        #region Overridden methods

        // [R21_8_GAMEPLAY_ACTIONS]
        // A player shall do one to three actions per turn.
        public bool IsValidOn(GameState state) {
            return state.NumMovesLeft < Game.MaxMovesPerTurn; // at least one move has been played
        }


        public GameState UpdateState(GameState state) {
            return state.Pass();
        }

        public override string ToString() {
            return "PassAction";
        }

        #endregion

        #region Equality members

        protected bool Equals(PassAction other) {
            return true;
        }

        public override bool Equals(object obj) {
            return null != obj && obj.GetType() == GetType();
        }

        public override int GetHashCode() {
            return GetType().GetHashCode();
        }

        public static bool operator ==(PassAction left, PassAction right) {
            return Equals(left, right);
        }

        public static bool operator !=(PassAction left, PassAction right) {
            return !Equals(left, right);
        }

        #endregion
    }

    #endregion
}