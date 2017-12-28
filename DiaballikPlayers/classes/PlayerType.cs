using System;
using Diaballik.Core;

namespace Diaballik.Players {
    /// <summary>
    ///     Represents a type of player, suitable for use in the UI.
    /// </summary>
    public enum PlayerType {
        Human,
        NoobAi,
        StartingAi,
        ProgressiveAi
    }


    public static class PlayerTypes {
        public static PlayerType FromPlayer(IPlayer player) {
            switch (player) {
                case HumanPlayer _:
                    return PlayerType.Human;
                case NoobAiPlayer _:
                    return PlayerType.NoobAi;
                case ProgressiveAiPlayer _:
                    return PlayerType.ProgressiveAi;
                case StartingAiPlayer _:
                    return PlayerType.StartingAi;
                default: throw new ArgumentOutOfRangeException();
            }
        }
    }
}