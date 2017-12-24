using System.CodeDom;
using System.Drawing;

namespace Diaballik.Core
{
    /// <summary>
    ///     Player of a game. Immutability must be preserved, because
    ///     players are composed into several other important immutable 
    ///     classes.
    /// </summary>
    public interface IPlayer
    {
        Color Color { get; }
        string Name { get; }

        /// <summary>
        ///     Is true if this player is an AI, in which case it
        ///     can be downcast to AiPlayer safely.
        /// </summary>
        bool IsAi { get; }


        /// <summary>
        ///     Gets the next move of this player. If the player is human,
        ///     may block until the player has committed to their decision.
        /// </summary>
        /// <param name="board">The board of the game</param>
        /// <returns>The next move of this player</returns>
        PlayerAction GetNextMove(GameBoard board);

    }

    public enum AiLevel
    {
        Noob,
        Starting,
        Progressive
    }

}