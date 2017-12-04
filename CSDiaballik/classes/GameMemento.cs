namespace CSDiaballik {
    // TODO this needs to be reworked


    /// <summary>
    ///     Represents one state in the history of the game.
    /// </summary>
    public abstract class GameMemento {

        /// <summary>
        ///     Gets the previous memento. Returns null if this is the root.
        /// </summary>
        /// <returns>The parent memento</returns>
        public abstract GameMemento GetParent();


        /// <summary>
        ///     Gets a new memento based on this one.
        /// </summary>
        /// <param name="action">The transition from this memento to the result</param>
        /// <returns>A new memento</returns>
        public MementoNode CreateNext(PlayerAction action) {
            return new MementoNode(this, action);
        }


        /// <summary>
        ///     Turns this memento into a Game.
        /// </summary>
        /// <returns>A game</returns>
        public abstract Game ToGame();

    }


    public class MementoNode : GameMemento {

        private readonly PlayerAction _action;
        private readonly GameMemento _previous;
        private Game _gameInstance;


        public MementoNode(GameMemento previous, PlayerAction action) {
            _previous = previous;
            _action = action;
        }


        public override GameMemento GetParent() {
            return _previous;
        }


        // Only works with immutable games
        public override Game ToGame() {
            return _gameInstance ?? (_gameInstance = _previous.ToGame().Update(_action));
        }

    }


    /// <inheritdoc />
    /// <summary>
    ///     Contains enough info to build the initial state of the game. Has no parent.
    ///     Can be serialized on disk and rebuilt.
    /// </summary>
    public class RootMemento : GameMemento {

        private readonly int _boardSize;
        private readonly bool _isFirstPlayerPlaying;
        private readonly PlayerBuilder _p1Spec;
        private readonly PlayerBuilder _p2Spec;
        private PlayerBoardSpec _boardSpec1;
        private PlayerBoardSpec _boardSpec2;
        private Game _gameInstance;


        public RootMemento(Game game) {
            _isFirstPlayerPlaying = game.CurrentPlayer == game.Player1;
            _p1Spec = PlayerToSpec(game.Player1);
            _p2Spec = PlayerToSpec(game.Player2);
            _boardSize = game.BoardSize;
        }


        public void SetBoardSpecs((PlayerBoardSpec, PlayerBoardSpec) specs) {
            _boardSpec1 = specs.Item1;
            _boardSpec2 = specs.Item2;
        }


        /// <summary>
        ///     Destructures the player into a builder, which can be used to build an equivalent player.
        /// </summary>
        /// <param name="player">Player to destructure</param>
        /// <returns>A builder describing the player</returns>
        private static PlayerBuilder PlayerToSpec(IPlayer player) {
            var spec = new PlayerBuilder {
                Color = player.Color,
                Name = player.Name
            };


            switch (player) {
                case NoobAiPlayer _:
                    spec.SetIsAi(AiPlayer.AiLevel.Noob);
                    break;
                case StartingAiPlayer _:
                    spec.SetIsAi(AiPlayer.AiLevel.Starting);
                    break;
                case ProgressiveAiPlayer _:
                    spec.SetIsAi(AiPlayer.AiLevel.Progressive);
                    break;
                case HumanPlayer _:
                    spec.SetIsHuman();
                    break;
            }

            return spec;
        }


        public override Game ToGame() {
            var players = (_p1Spec.Build(), _p2Spec.Build());
            var specs = players.Zip((_boardSpec1, _boardSpec2),
                                      (player, spec) => new FullPlayerBoardSpec(player, spec));

            var board = GameBoard.New(_boardSize, specs);
            return _gameInstance ?? (_gameInstance = Game.New(board, _isFirstPlayerPlaying));
        }


        public override GameMemento GetParent() {
            return null;
        }

    }
}
