using System;
using System.Drawing;

namespace CSDiaballik {
    public class HumanPlayer : AbstractPlayer {

        public HumanPlayer(Color color, string name) : base(color, name) {
        }


        public sealed override bool IsAi() {
            return false;
        }


        public override IPlayerAction GetNextMove(GameBoard board) {
            throw new NotImplementedException();
        }

    }
}
