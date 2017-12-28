using System;
using System.Drawing;
using Diaballik.Core;

namespace Diaballik.Players {
    /// <summary>
    ///     Builds player instances.
    /// </summary>
    public sealed class PlayerBuilder {
        #region Properties

        public Color Color { set; get; } = Color.Yellow;

        public string Name { set; get; }

        public PlayerType SelectedPlayerType { get; set; }

        #endregion

        #region Fluent interface methods

        // for fluent interface
        public PlayerBuilder SetName(string n) {
            Name = n;
            return this;
        }

        // for fluent interface
        public PlayerBuilder SetColor(Color c) {
            Color = c;
            return this;
        }

        // for fluent interface
        public PlayerBuilder SetPlayerType(PlayerType t) {
            SelectedPlayerType = t;
            return this;
        }

        #endregion

        #region Build method

        /// <summary>
        ///     Builds a player with the specified configuration.
        /// </summary>
        /// <returns>A new player</returns>
        public IPlayer Build() {
            switch (SelectedPlayerType) {
                case PlayerType.Human:
                    return new HumanPlayer(Color, Name);
                case PlayerType.NoobAi:
                    return new NoobAiPlayer(Color, Name);
                case PlayerType.StartingAi:
                    return new StartingAiPlayer(Color, Name);
                case PlayerType.ProgressiveAi:
                    return new ProgressiveAiPlayer(Color, Name);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        #endregion
    }
}