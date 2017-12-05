namespace CSDiaballik {
    /// <summary>
    ///     Represents a delta from a previous game state.
    /// 
    ///     This recursive data structure stores the chain
    ///     of actions building up to a current game state.
    ///     Each <see cref="GameState"/> stores the chain
    ///     leading to its creation, and can thus be serialised
    ///     painlessly.
    /// 
    ///     GameMementos can build the game they represent.
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
        public abstract GameState ToGame();

    }


    public class MementoNode : GameMemento {

        /// Action to perform on the previous state to get this state
        private readonly PlayerAction _action;

        /// Previous memento in the chain
        private readonly GameMemento _previous;

        /// Cached game state
        private GameState _thisGameState;


        public MementoNode(GameState previous, PlayerAction action) {
            _previous = previous.Memento;
            _action = action;
            _thisGameState = previous;
        }


        public MementoNode(GameMemento previous, PlayerAction action) {
            _previous = previous;
            _action = action;
        }


        public override GameMemento GetParent() {
            return _previous;
        }


        // Only works with immutable games
        public override GameState ToGame() {
            return _thisGameState ?? (_thisGameState = _previous.ToGame().Update(_action));
        }

    }


    /// <inheritdoc />
    /// <summary>
    ///     Contains enough info to build the initial state of the game. Has no parent.
    /// </summary>
    public class RootMemento : GameMemento {

        private readonly int _boardSize;
        private readonly bool _isFirstPlayerPlaying;
        private readonly PlayerBuilder _p1Spec;
        private readonly PlayerBuilder _p2Spec;
        private readonly PlayerBoardSpec _boardSpec1;
        private readonly PlayerBoardSpec _boardSpec2;
        private GameState _initialState;


        public RootMemento(GameState initialState, (PlayerBoardSpec, PlayerBoardSpec) specs) {
            _isFirstPlayerPlaying = initialState.CurrentPlayer == initialState.Player1;
            _p1Spec = initialState.Player1.ToBuilder();
            _p2Spec = initialState.Player2.ToBuilder();

            _boardSize = initialState.BoardSize;
            _initialState = initialState;

            _boardSpec1 = specs.Item1;
            _boardSpec2 = specs.Item2;
        }


        public override GameState ToGame() {
            var players = (_p1Spec.Build(), _p2Spec.Build());
            var specs = players.Zip((_boardSpec1, _boardSpec2),
                                    (player, spec) => new FullPlayerBoardSpec(player, spec));

            return _initialState ?? (_initialState = GameState.InitialState(_boardSize, specs, _isFirstPlayerPlaying));
        }


        public override GameMemento GetParent() {
            return null;
        }

    }
}
