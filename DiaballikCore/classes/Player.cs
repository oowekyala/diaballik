using System.CodeDom;
using System.Windows.Media;

namespace Diaballik.Core {
    // clarifies intent
    using StateWithHistory = GameMemento;

    /// <summary>
    ///     Player of a game. This object is generally used as a placeholder
    ///     in classes like BoardLikes, therefore, immutability is key.
    ///     
    ///     The behavior of players is defined either in an AiDecisionAlgo,
    ///     if the player is an AI, or in the WPF logic for human players.
    /// </summary>
    public sealed class Player {
        #region Properties

        public Color Color { get; }
        public string Name { get; }
        public PlayerType Type { get; }


        /// <summary>
        ///     Is true if this player is an AI.
        /// </summary>
        private bool IsAi => Type == PlayerType.NoobAi
                             || Type == PlayerType.ProgressiveAi
                             || Type == PlayerType.StartingAi;

        #endregion

        public Player(Color color, string name, PlayerType type) {
            Color = color;
            Name = name;
            Type = type;
        }


        public override string ToString() {
            return $"Player(type: {Type}, color: {new ColorConverter().ConvertToString(Color)}, name: {Name})";
        }


        #region Equality members

        private bool Equals(Player other) {
            return Color.Equals(other.Color)
                   && string.Equals(Name, other.Name)
                   && Equals(Type, other.Type);
        }

        public override bool Equals(object obj) {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Player) obj);
        }

        public override int GetHashCode() {
            unchecked {
                return (Color.GetHashCode() * 397) ^ Name.GetHashCode();
            }
        }

        public static bool operator ==(Player left, Player right) {
            return Equals(left, right);
        }

        public static bool operator !=(Player left, Player right) {
            return !Equals(left, right);
        }

        #endregion
    }

    /// <summary>
    ///     Represents a type of player, suitable for use in the UI.
    /// </summary>
    public enum PlayerType {
        Human,
        NoobAi,
        StartingAi,
        ProgressiveAi
    }
}