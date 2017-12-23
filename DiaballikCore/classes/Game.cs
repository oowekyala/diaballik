using System;


namespace Diaballik.Core {
    using FullPlayerSpecPair = ValueTuple<FullPlayerBoardSpec, FullPlayerBoardSpec>;

    /// <summary>
    ///    Represents a game. This class is mutable, and maintains a GameState 
    ///    instance and the memento instance. 
    /// 
    ///    This class decouples GameStates from GameMementos, and allows to reuse 
    ///    the same GameState objects in case of Undo actions while still registering
    ///    these actions in the memento.
    /// </summary>
    public sealed class Game {
        private static readonly Random Rng = new Random();
        public const int MaxMovesPerTurn = 3;


        /// Stores the chain of actions leading to this state
        /// Equals(Memento.ToGame(), State)
        public GameMemento Memento { get; private set; }

        public GameState State => Memento.ToGame();

        // Only to initialise the game
        private Game(int size, FullPlayerSpecPair specs, bool isFirstPlayerPlaying) {
            var state = GameState.InitialState(size, specs, isFirstPlayerPlaying);
            Memento = new RootMemento(state, specs);
        }


        /// <summary>
        ///     Creates a new game, initialised with the given player specs. 
        /// </summary>
        /// <param name="size">Size of the board</param>
        /// <param name="specs">Board configurations for each player</param>
        /// <param name="isFirstPlayerPlaying">Whether player 1 is the first to start playing or not</param>
        /// <returns>A new game</returns>
        public static Game Init(int size, FullPlayerSpecPair specs,
            bool isFirstPlayerPlaying) {
            return new Game(size, specs, isFirstPlayerPlaying);
        }


        /// <summary>
        ///     Creates a new game, initialised with the given player specs. 
        ///     The first player to play is chosen randomly.
        /// </summary>
        /// <param name="size">Size of the board</param>
        /// <param name="specs">Board configuration for each player</param>
        /// <returns>A new game</returns>
        public static Game Init(int size, FullPlayerSpecPair specs) {
            return new Game(size, specs, Rng.Next(0, 1) == 1);
        }


        /// <summary>
        ///     Updates the state of the game with the current action.
        /// </summary>
        /// <param name="action">The action to be played by the current player</param>
        /// <exception cref="ArgumentException">
        ///     If the move is invalid. The action's validity should be verified upstream.
        /// </exception>
        public void Update(PlayerAction action) {
            if (!action.IsMoveValid(State)) {
                throw new ArgumentException("Invalid move: " + action);
            }

            switch (action) {
                case UpdateAction update:
                    Memento = Memento.Append(update.UpdateState(State), update);
                    break;
                case UndoAction undo:
                    Memento = Memento.Append(undo);
                    break;
            }
        }
    }
}