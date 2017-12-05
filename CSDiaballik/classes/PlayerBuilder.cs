using System;
using System.Drawing;

namespace CSDiaballik {
    /// <summary>
    ///     Builds player instances.
    /// </summary>
    public class PlayerBuilder {

        private static bool _isFirstPlayer;
        private AiPlayer.AiLevel _aiLevel;
        private bool _isHuman;


        public PlayerBuilder() {
            _isFirstPlayer = !_isFirstPlayer; // true for the first instantiation, false for the second. Quite shitty tbh
            Color = _isFirstPlayer ? Color.Blue : Color.Red;
        }


        public Color Color { set; get; }

        public string Name { set; get; }


        /// <summary>
        ///     Specify that the current player is an Ai.
        ///     This overrides previous calls to setIsAi
        ///     and setIsHuman.
        /// </summary>
        /// <param name="level">The difficulty level of the AI</param>
        /// <returns>The same builder</returns>
        public PlayerBuilder SetIsAi(AiPlayer.AiLevel level) {
            _aiLevel = level;
            _isHuman = false;
            return this;
        }


        /// <summary>
        ///     Specify that the current player is a human.
        ///     This overrides previous calls to setIsAi.
        /// </summary>
        /// <returns>The same builder</returns>
        public PlayerBuilder SetIsHuman() {
            _isHuman = true;
            return this;
        }


        /// <summary>
        ///     Builds a player with the specified configuration.
        /// </summary>
        /// <returns>A new player</returns>
        public IPlayer Build() {
            if (_isHuman) {
                return new HumanPlayer(Color, Name);
            }

            switch (_aiLevel) {
                case AiPlayer.AiLevel.Noob:
                    return new NoobAiPlayer(Color, Name);
                case AiPlayer.AiLevel.Starting:
                    return new StartingAiPlayer(Color, Name);
                case AiPlayer.AiLevel.Progressive:
                    return new ProgressiveAiPlayer(Color, Name);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

    }
}
