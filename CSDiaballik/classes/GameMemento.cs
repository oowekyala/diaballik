using System.Linq;

namespace CSDiaballik
{
    public abstract class GameMemento
    {
        /// <summary>
        /// Gets the previous memento. Returns null if this is the root.
        /// </summary>
        /// <returns>The parent memento</returns>
        public abstract GameMemento GetParent();

        /// <summary>
        /// Gets a new memento based on this one.
        /// </summary>
        /// <param name="action">The transition from this memento to the result</param>
        /// <returns>A new memento</returns>
        public MementoNode Next(PlayerAction action)
        {
            return new MementoNode(this, action);
        }
    }


    public class MementoNode : GameMemento
    {
        private GameMemento _previous;
        private PlayerAction _action;


        public MementoNode(GameMemento previous, PlayerAction action)
        {
            _previous = previous;
            _action = action;
        }


        public override GameMemento GetParent()
        {
            return _previous;
        }
    }


    /// <summary>
    /// Contains enough info to build the initial state of the game. Has no parent.
    /// </summary>
    public class RootMemento : GameMemento
    {
        private readonly PlayerBuilder _p1Spec;
        private readonly PlayerBuilder _p2Spec;
        private readonly bool _isFirstPlayerPlaying;
        private readonly int _boardSize;


        public RootMemento(Game game)
        {
            _isFirstPlayerPlaying = game.CurrentPlayer == game.Player1;
            _p1Spec = PlayerToSpec(game.Player1);
            _p2Spec = PlayerToSpec(game.Player2);
            _boardSize = game.BoardSize;
        }

        /// <summary>
        /// Destructures the player into a builder, which can be used to build an equivalent player.
        /// </summary>
        /// <param name="player">Player to destructure</param>
        /// <returns>A builder describing the player</returns>
        private static PlayerBuilder PlayerToSpec(IPlayer player)
        {
            var spec = new PlayerBuilder
            {
                Color = player.Color,
                Name = player.Name
            };

            spec.Pieces(player.Pieces.Select(p => p.Position));

            switch (player)
            {
                case NoobAiPlayer _:
                    spec.SetIsAi(AiPlayer.AiLevel.Noob);
                    break;
                case StartingAiPlayer _:
                    spec.SetIsAi(AiPlayer.AiLevel.Starting);
                    break;
                case ProgressiveAiPlayer _:
                    spec.SetIsAi(AiPlayer.AiLevel.Progressive);
                    break;
            }

            return spec;
        }

        public Game ToGame()
        {
            var p1 = _p1Spec.Build();
            var p2 = _p2Spec.Build();

            var board = new GameBoard(_boardSize, p1.Pieces, p2.Pieces);


            return new Game(board, p1, p2, _isFirstPlayerPlaying);
        }

        public override GameMemento GetParent()
        {
            return null;
        }
    }
}