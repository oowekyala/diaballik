using System.Drawing;
using Diaballik.Core;

namespace Diaballik.Players {
    public abstract class AbstractPlayer : IPlayer {
        protected AbstractPlayer(Color color, string name) {
            Color = color;
            Name = name;
        }


        public Color Color { get; }

        public string Name { get; }

        public abstract bool IsAi();

        public abstract PlayerAction GetNextMove(GameBoard board);


        public virtual PlayerBuilder ToBuilder() => new PlayerBuilder {
            Color = Color,
            Name = Name
        };

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
    }
}