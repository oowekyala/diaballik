using System.Drawing;
using Diaballik.Core;

namespace Diaballik.Players {
    using StateWithHistory = GameMemento;

    /// <summary>
    ///     Base class for players.
    /// </summary>
    public abstract class AbstractPlayer : IPlayer {
        #region Overridden properties

        public Color Color { get; }

        public string Name { get; }

        public abstract bool IsAi { get; }

        #endregion

        #region Base constructor

        protected AbstractPlayer(Color color, string name) {
            Color = color;
            Name = name;
        }

        #endregion

        #region Methods

        public abstract IUpdateAction GetNextMove(StateWithHistory board);

        /// call base implementation if overriding
        public virtual PlayerBuilder ToBuilder() => new PlayerBuilder {
            Color = Color,
            Name = Name,
            SelectedPlayerType = PlayerTypes.FromPlayer(this)
        };

        public override string ToString() {
            return $"Player(type: {GetType().Name}, color: {ColorTranslator.ToHtml(Color)}, name: {Name})";
        }

        #endregion

        #region Equality members

        protected bool Equals(AbstractPlayer other) {
            return Color.Equals(other.Color) && string.Equals(Name, other.Name);
        }

        public override bool Equals(object obj) {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((AbstractPlayer) obj);
        }

        public override int GetHashCode() {
            unchecked {
                return (Color.GetHashCode() * 397) ^ Name.GetHashCode();
            }
        }

        public static bool operator ==(AbstractPlayer left, AbstractPlayer right) {
            return Equals(left, right);
        }

        public static bool operator !=(AbstractPlayer left, AbstractPlayer right) {
            return !Equals(left, right);
        }

        #endregion
    }
}