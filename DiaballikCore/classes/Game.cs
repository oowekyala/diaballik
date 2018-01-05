using System;
using System.Collections.Generic;
using System.Linq;


namespace Diaballik.Core {
    using FullPlayerSpecPair = ValueTuple<FullPlayerBoardSpec, FullPlayerBoardSpec>;

    /// <summary>
    ///     Represents a game. This class is mutable, to provide a nice interface
    ///     to clients.
    /// 
    ///     Internally it wraps a GameMemento, which already 
    ///     contains all the information about the current and past states of 
    ///     the game. 
    /// 
    ///     It adds the necessary logic to undo and redo actions.
    /// </summary>
    public sealed class Game {
        #region Constants

        private static readonly Random Rng = new Random();

        /// Maximum number of moves per turn
        public const int MaxMovesPerTurn = 3;

        #endregion

        #region Properties

        /// Stores the chain of actions leading to this state
        public GameMemento Memento { get; private set; }

        /// Current state of the game
        public GameState State => Memento.State;

        /// Size of the board
        public int BoardSize => State.BoardSize;

        public Player Player1 => State.Player1;
        public Player Player2 => State.Player2;


        /// True if <see cref="Undo"/> can be executed.
        public bool CanUndo => Memento.Parent != null;

        /// True if <see cref="Redo"/> can be executed.
        /// A player can only call Redo if the last action taken was to call Undo.
        public bool CanRedo => _breadCrumbs.Any();

        /// Stores the mementos we just undid to go back with Redo.
        /// Cleared whenever another action is taken.
        private readonly Stack<GameMemento> _breadCrumbs = new Stack<GameMemento>();

        private readonly object _syncLock = new object();

        #endregion

        #region Constructors

        // Only to initialise the game
        private Game(int size, FullPlayerSpecPair specs, bool isFirstPlayerPlaying) {
            Memento = new RootMemento(specs, size, isFirstPlayerPlaying);
        }

        // Only when loading an existing memento
        private Game(GameMemento memento) {
            Memento = memento;
        }

        #endregion

        #region Factory methods

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
            return new Game(size, specs, Rng.Next(0, 2) == 1);
        }

        /// <summary>
        ///     Returns a new game corresponding to the given memento.
        /// </summary>
        /// <param name="memento"></param>
        /// <returns></returns>
        public static Game FromMemento(GameMemento memento) {
            return new Game(memento);
        }

        /// <summary>
        ///     Forks a new game from the given one, clearing the redo 
        ///     stack.
        /// </summary>
        /// <param name="game">The game to fork</param>
        /// <returns>
        ///     A new game, with the same current state as the given one.
        /// </returns>
        public static Game Fork(Game game) {
            return FromMemento(game.Memento);
        }

        #endregion

        #region Behavior

        /// <summary>
        ///     Updates the state of the game with the current action.
        /// </summary>
        /// <param name="action">The action to be played by the current player</param>
        /// <exception cref="ArgumentException">
        ///     If the move is invalid. The action's validity should be verified upstream.
        /// </exception>
        public void Update(IUpdateAction action) {
            lock (_syncLock) {
                if (action.IsValidOn(State)) {
                    Memento = Memento.Update(action);
                    _breadCrumbs.Clear();
                } else {
                    throw new ArgumentException("Invalid move: " + action);
                }
            }
        }

        /// <summary>
        ///     Undoes the last action taken. The action can be retaken
        ///     using <see cref="Redo"/>, if there were no 
        ///     <see cref="Update"/> calls in between.
        /// </summary>
        public void Undo() {
            lock (_syncLock) {
                if (CanUndo) {
                    _breadCrumbs.Push(Memento);
                    Memento = Memento.Parent;
                } else {
                    throw new ArgumentException("Nothing to undo");
                }
            }
        }

        /// <summary>
        ///     Redoes the last undone action.
        /// </summary>
        public void Redo() {
            lock (_syncLock) {
                if (CanRedo) {
                    Memento = _breadCrumbs.Pop();
                } else {
                    throw new ArgumentException("Nothing to redo");
                }
            }
        }

        #endregion

        #region Equality members

        private bool Equals(Game other) {
            return Equals(Memento, other.Memento);
        }

        public override bool Equals(object obj) {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj is Game && Equals((Game) obj);
        }

        public override int GetHashCode() {
            return (Memento != null ? Memento.GetHashCode() : 0);
        }

        public static bool operator ==(Game left, Game right) {
            return Equals(left, right);
        }

        public static bool operator !=(Game left, Game right) {
            return !Equals(left, right);
        }

        #endregion
    }
}