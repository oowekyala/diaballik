using System.Collections.Generic;
using System.Drawing;

namespace CSDiaballik
{
    public abstract class AbstractPlayer : IPlayer
    {
        private readonly Color _color;
        private readonly string _name;

        Color IPlayer.Color => _color;
        string IPlayer.Name => _name;
        Piece IPlayer.BallBearer { get; set; }
        public List<Piece> Pieces { get; }
        public abstract PlayerAction GetNextMove();


        protected AbstractPlayer(Color color, string name, List<Piece> pieces)
        {
            _color = color;
            _name = name;
            Pieces = pieces;
        }
    }
}