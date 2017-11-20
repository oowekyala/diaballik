using System.Collections.Generic;
using System.Drawing;
using System.Linq;

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


        protected AbstractPlayer(Color color, string name, List<Position2D> pieces)
        {
            _color = color;
            _name = name;
            Pieces = pieces.Select(p => new Piece(this)).ToList();
        }
    }
}