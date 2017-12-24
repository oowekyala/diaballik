using System;


namespace Diaballik.Core {
    using FullPlayerSpecPair = ValueTuple<FullPlayerBoardSpec, FullPlayerBoardSpec>;

    /// <summary>
    ///     Represents a game. This class is mutable, to provide a nice interface
    ///     to clients.
    /// 
    ///     Internally it's just a wrapper around a GameMemento, which already 
    ///     contains all the information about the current and past states of 
    ///     the game.
    /// </summary>
    public sealed class Game {
        private static readonly Random Rng = new Random();

        /// Maximum number of moves per turn
        public const int MaxMovesPerTurn = 3;

        /// Stores the chain of actions leading to this state
        public GameMemento Memento { get; private set; }

        /// Current state of the game
        public GameState State => Memento.ToGame(); // PlayerAction.UpdateState(GameState) is called lazily in that method

        // Only to initialise the game
        private Game(int size, FullPlayerSpecPair specs, bool isFirstPlayerPlaying) {
            Memento = new RootMemento(specs, size, isFirstPlayerPlaying);
        }


        /// <summary>
        ///     Creates a new game, initialised with the given player specs. 
        /// </summary>
        /// <param name="size">Size of the board</param>
        /// <param name="specs">Board configurations for each player</param>
        /// <param name="isFirstPlayerPlaying">Whether player 1 is the first to start playing or not</param>
        /// <returns>A new game</returns>
        public static Game Init(int size, FullPlayerSpecPair specs, bool isFirstPlayerPlaying) {
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
            if (action.IsMoveValid(State)) {
                Memento = Memento.Append(action);
            } else {
                throw new ArgumentException("Invalid move: " + action);
            }
        }
    }
}