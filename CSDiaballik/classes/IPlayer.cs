using System.Collections.ObjectModel;
using System.Drawing;

namespace CSDiaballik
{
    public interface IPlayer
    {
        Color Color { get; }
        string Name { get; }
        Piece BallBearer { get; set; }
        ReadOnlyCollection<Piece> Pieces { get; }


        /// <summary>
        ///     Returns true if this player is an AI, in which case it can be downcast to AiPlayer safely.
        /// </summary>
        /// <returns>Whether the player is an AI</returns>
        bool IsAi();


        /// <summary>
        ///     Gets the next move of this player. If the player is human, may block until the player has committed their action.
        /// </summary>
        /// <returns>The next move of this player</returns>
        PlayerAction GetNextMove();
    }
}
