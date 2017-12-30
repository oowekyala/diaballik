using System.Windows.Media;
using Diaballik.Core.Util;

namespace Diaballik.Core.Builders {
    /// <summary>
    ///     Builds player instances.
    /// </summary>
    public sealed class PlayerBuilder {
        #region Properties

        public Color Color { set; get; } = Colors.Yellow;

        public string Name { set; get; } = "";

        public PlayerType SelectedPlayerType { get; set; }

        public bool CannotBuild => !CanBuild;
        public bool CanBuild => ErrorMessage == string.Empty;

        public string ErrorMessage => string.Empty;

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
        public Player Build() {
            DiaballikUtil.Assert(CanBuild, ErrorMessage);
            return new Player(Color, Name, SelectedPlayerType);
        }

        #endregion
    }
}