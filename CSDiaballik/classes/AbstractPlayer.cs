using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;

namespace CSDiaballik
{
    public abstract class AbstractPlayer : IPlayer
    {
        protected AbstractPlayer(Color color, string name, IEnumerable<Position2D> pieces)
        {
            Color = color;
            Name = name;

            var position2Ds = pieces as IList<Position2D> ?? pieces.ToList();
            Pieces = position2Ds.Select(p => new Piece(this, p)).Distinct().ToList().AsReadOnly();

            if (position2Ds.Count > Pieces.Count)
            {
                throw new ArgumentException("Duplicate positions, check calling code");
            }
        }


        public Color Color { get; }

        public string Name { get; }

        public Piece BallBearer { get; set; }

        public ReadOnlyCollection<Piece> Pieces { get; }

        public abstract bool IsAi();
        
        public abstract PlayerAction GetNextMove();
    }
}
