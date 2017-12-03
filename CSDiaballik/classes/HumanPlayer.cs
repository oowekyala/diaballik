using System;
using System.Drawing;

namespace CSDiaballik
{
    public class HumanPlayer : AbstractPlayer
    {
        public HumanPlayer(Color color, string name) : base(color, name)
        {
        }


        public override bool IsAi()
        {
            return false;
        }


        public override PlayerAction GetNextMove(GameBoard board)
        {
            throw new NotImplementedException();
        }
    }
}
