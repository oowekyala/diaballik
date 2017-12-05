namespace CSDiaballik {
    /// <summary>
    ///     Represents a delta from a previous game state.
    /// 
    ///     This recursive data structure stores the chain
    ///     of actions building up to a current game state.
    ///     They can build the game they represent, and reuse
    ///     already built states.
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
        /// <returns>A new memento with this as its parent</returns>
        public GameMemento Append(IUpdateAction action) {
            return new MementoNode(this, action);
        }


        /// <summary>
        ///     Creates a new memento based on this one.
        /// </summary>
        /// <param name="state">The state obtained after the transition</param>
        /// <param name="action">The transition from this memento to the result</param>
        /// <returns>A new memento with this as its parent</returns>
        public GameMemento Append(GameState state, IUpdateAction action) {
            return new MementoNode(state, this, action);
        }
        
        /// <summary>
        ///     Creates a new memento based on this one, adding an undo action.
        /// </summary>
        /// <param name="undoAction">Undo action</param>
        /// <returns>A new memento with this as its parent</returns>
        public GameMemento Undo(UndoAction undoAction) {
            return new UndoMementoNode(this, undoAction);
        }


        /// <summary>
        ///     Turns this memento into a Game.
        /// </summary>
        /// <returns>A game</returns>
        public abstract GameState ToGame();

    }


    /// <inheritdoc />
    /// <summary>
    ///     Abstract class for memento nodes that have a parent.
    /// </summary>
    public abstract class AbstractMementoNode : GameMemento {

        /// Previous memento in the chain
        protected readonly GameMemento Previous;


        protected AbstractMementoNode(GameMemento previous) {
            Previous = previous;
        }


        public sealed override GameMemento GetParent() {
            return Previous;
        }

    }


    public class MementoNode : AbstractMementoNode {

        /// Action to perform on the previous state to get this state
        private readonly IUpdateAction _action;

        /// Cached game state
        private GameState _thisGameState;


        public MementoNode(GameState thisState, GameMemento previous, IUpdateAction action) : base(previous) {
            _action = action;
            _thisGameState = thisState;
        }


        public MementoNode(GameMemento previous, IUpdateAction action) : base(previous) {
            _action = action;
        }


        // Only works with immutable games
        public override GameState ToGame() {
            return _thisGameState ?? (_thisGameState = _action.UpdateState(Previous.ToGame()));
        }

    }


    /// <inheritdoc />
    /// <summary>
    ///     Represents an undo action in the update chain. Special 
    ///     handling because we don't create a new state when undoing, 
    ///     we delegate the call to ToGame to the previous nodes.
    /// </summary>
    public class UndoMementoNode : AbstractMementoNode {

        private readonly UndoAction _undoAction; // may be useful for eg timestamps


        public UndoMementoNode(GameMemento memento, UndoAction undoAction) : base(memento) {
            _undoAction = undoAction;
        }


        public override GameState ToGame() {
            return Previous.GetParent().ToGame();
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
