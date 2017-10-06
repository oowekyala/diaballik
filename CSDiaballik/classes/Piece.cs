using System.Drawing;


namespace CSDiaballik
{
    public class Piece
    {
        public Color color { get; }
        public Player player { get; }


        public Piece(Player p)
        {
            player = p;
            color = p.color;
        }
    }
}