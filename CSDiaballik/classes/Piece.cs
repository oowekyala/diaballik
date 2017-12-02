using System.Drawing;


namespace CSDiaballik
{
    /// <summary>
    /// Represents a piece owned by a player.
    /// </summary>
    public class Piece
    {
        public Color Color { get; }
        public IPlayer Player { get; }
        public Position2D Position { get; set; }

        
        public Piece(IPlayer p, Position2D pos)
        {
            Player = p;
            Position = pos;
            Color = p.Color;
        }
    }
}