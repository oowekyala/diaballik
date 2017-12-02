using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;

namespace CSDiaballik
{
    public abstract class AbstractPlayer : IPlayer
    {
        public Color Color { get; }

        public string Name { get; }

        public Piece BallBearer { get; set; }

        public ReadOnlyCollection<Piece> Pieces { get; }

        public abstract PlayerAction GetNextMove();


        protected AbstractPlayer(Color color, string name, IEnumerable<Position2D> pieces)
        {
            Color = color;
            Name = name;
            Pieces = pieces.Select(p => new Piece(this, p)).ToList().AsReadOnly();
        }
    }
}
