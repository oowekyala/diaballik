using System;
using System.Collections.Generic;
using System.Drawing;

namespace CSDiaballik
{
    public class HumanPlayer : AbstractPlayer
    {
        public HumanPlayer(Color color, string name, IEnumerable<Position2D> pieces) : base(color, name, pieces)
        {
        }


        public override bool IsAi() => false;


        public override PlayerAction GetNextMove()
        {
            throw new NotImplementedException();
        }
    }
}
