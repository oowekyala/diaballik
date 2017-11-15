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
        private PlayerBuilder _p1Spec;
        private PlayerBuilder _p2Spec;
        private bool _isFirstPlayerPlaying;

        public RootMemento(Game game)
        {
            _isFirstPlayerPlaying = game.CurrentPlayer == game.Player1;
            _p1Spec = PlayerToSpec(game.Player1);
            _p2Spec = PlayerToSpec(game.Player2);
        }

        private static PlayerBuilder PlayerToSpec(IPlayer player)
        {
            var spec = new PlayerBuilder
            {
                Color = player.Color,
                Name = player.Name
            };

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

        public override GameMemento GetParent()
        {
            return null;
        }
    }
}