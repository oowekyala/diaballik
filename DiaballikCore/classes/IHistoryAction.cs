namespace Diaballik.Core {
    // clarifies intent
    using StateWithHistory = GameMemento;

    /// <inheritdoc />
    /// <summary>
    ///     Represents a player action that only makes sense in the
    ///     context of a state with a history.
    /// 
    ///     Carrying out the action adds a node to the history and
    ///     is handled by the Game.
    /// </summary>
    public interface IHistoryAction : IPlayerAction {
    }

    /// <inheritdoc />
    /// <summary>
    ///     Undo the last action of the player.
    /// </summary>
    public class UndoAction : IHistoryAction {
        #region Overridden methods

        public override string ToString() {
            return "UndoAction";
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

        public static bool operator ==(UndoAction left, UndoAction right) {
            return Equals(left, right);
        }

        public static bool operator !=(UndoAction left, UndoAction right) {
            return !Equals(left, right);
        }

        #endregion
    }

    /// <inheritdoc />
    /// <summary>
    ///     Redo an action previously undone with an <see cref="UndoAction" />.
    /// </summary>
    public class RedoAction : IHistoryAction {
        #region Overridden methods

        public override string ToString() {
            return "RedoAction";
        }

        #endregion

        #region Equality members

        protected bool Equals(RedoAction other) {
            return true;
        }

        public override bool Equals(object obj) {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj.GetType() == this.GetType();
        }

        public override int GetHashCode() {
            return GetType().GetHashCode();
        }

        public static bool operator ==(RedoAction left, RedoAction right) {
            return Equals(left, right);
        }

        public static bool operator !=(RedoAction left, RedoAction right) {
            return !Equals(left, right);
        }

        #endregion
    }
}