using System.Drawing;

namespace CSDiaballik {
    public abstract class AbstractPlayer : IPlayer {

        protected AbstractPlayer(Color color, string name) {
            Color = color;
            Name = name;
        }


        public Color Color { get; }

        public string Name { get; }

        public abstract bool IsAi();

        public abstract Game.Action GetNextMove(GameBoard board);

    }
}
