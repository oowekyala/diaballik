using System;
using System.Drawing;
using Diaballik.Core;

namespace Diaballik.Players {

    /// <summary>
    ///     Human player. Waits for UI events to get the next move.
    /// </summary>
    public class HumanPlayer : AbstractPlayer {
        public HumanPlayer(Color color, string name) : base(color, name) {
        }


        public override bool IsAi { get; } = false;


        public override IPlayerAction GetNextMove(GameBoard board) {
            throw new NotImplementedException();
        }


        public override PlayerBuilder ToBuilder() => base.ToBuilder().SetIsHuman();
    }
}