using System;
using System.Windows.Media;
using Diaballik.Core;

namespace Diaballik.Players {
    using StateWithHistory = GameMemento;

    /// <summary>
    ///     Human player. Waits for UI events to get the next move.
    /// </summary>
    public class HumanPlayer : AbstractPlayer {
        public HumanPlayer(Color color, string name) : base(color, name) {
        }


        public override bool IsAi { get; } = false;


        public override IUpdateAction GetNextMove(StateWithHistory board) {
            throw new NotImplementedException();
        }
    }
}