using System.Drawing;


namespace CSDiaballik
{
    public class Piece
    {
        public Color Color { get; }
        public IPlayer Player { get; }
        public Position2D Position { get; set; }


        public Piece(IPlayer p)
        {
            Player = p;
            Color = p.Color;
        }
    }
}