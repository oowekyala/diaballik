using System;
using System.Drawing;

namespace CSDiaballik {
    public class HumanPlayer : AbstractPlayer {
        public HumanPlayer(Color color, string name) : base(color, name) {
        }


        public sealed override bool IsAi() => false;


        public override IPlayerAction GetNextMove(GameBoard board) {
            throw new NotImplementedException();
        }


        public override PlayerBuilder ToBuilder() => base.ToBuilder().SetIsHuman();
    }
}