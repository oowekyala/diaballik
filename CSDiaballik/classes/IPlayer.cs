using System.Collections.Generic;
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
        /// Gets the next move of this player. If the player is human, may block until the player has committed their action.
        /// </summary>
        /// <returns>The next move of this player</returns>
        PlayerAction GetNextMove();
    }
}
