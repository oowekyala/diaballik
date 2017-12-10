using System;
using System.Collections.Generic;

namespace CSDiaballik {
    /// <summary>
    ///    Represents a game. This class is mutable, and maintains a GameState 
    ///    instance and the memento instance. 
    /// 
    ///    This class decouples GameStates from GameMementos, and allows to reuse 
    ///    the same GameState objects in case of Undo actions while still registering
    ///    these actions in the memento.
    /// </summary>
    public class Game {
        public const int MaxMovesPerTurn = 3;


        /// Stores the chain of actions leading to this state
        /// Equals(Memento.ToGame(), State)
        public GameMemento Memento { get; private set; }

        /// Current state of the game
        public GameState State { get; private set; }

        public int BoardSize => State.BoardSize;
        public (IEnumerable<Position2D>, IEnumerable<Position2D>) PositionsPair => State.PositionsPair;
        public (Position2D, Position2D) BallBearerPair => State.BallBearerPair;


        // Only to initialise the game
        private Game(int size, (FullPlayerBoardSpec, FullPlayerBoardSpec) specs, bool isFirstPlayerPlaying) {
            State = GameState.InitialState(size, specs, isFirstPlayerPlaying);
            Memento = new RootMemento(State, specs);
        }


        /// <summary>
        ///     Creates a new game, initialised with the given player specs. 
        /// </summary>
        /// <param name="size">Size of the board</param>
        /// <param name="specs">Board configurations for each player</param>
        /// <param name="isFirstPlayerPlaying">Whether player 1 is the first to start playing or not</param>
        /// <returns>A new game</returns>
        public static Game Init(int size, (FullPlayerBoardSpec, FullPlayerBoardSpec) specs,
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
        public static Game Init(int size, (FullPlayerBoardSpec, FullPlayerBoardSpec) specs) {
            return new Game(size, specs, new Random().Next(0, 1) == 1);
        }


        /// <summary>
        ///     Updates the state of the game with the current action.
        /// </summary>
        /// <param name="action">The action to be played by the current player</param>
        /// <exception cref="ArgumentException">
        ///     If the move is invalid. The action's validity should be verified upstream.
        /// </exception>
        public void Update(IPlayerAction action) {
            if (!State.IsMoveValid(action)) {
                throw new ArgumentException("Invalid move: " + action);
            }

            switch (action) {
                case MoveBallAction moveBall:
                    State = State.MoveBall(moveBall.Src, moveBall.Dst);
                    Memento = Memento.Append(State, moveBall);
                    break;
                case MovePieceAction movePiece:
                    State = State.MovePiece(movePiece.Src, movePiece.Dst);
                    Memento = Memento.Append(State, movePiece);
                    break;
                case PassAction pass:
                    State = State.Pass();
                    Memento = Memento.Append(State, pass);
                    break;
                case UndoAction undo:
                    var previousState = Memento.GetParent().ToGame();
                    Memento = Memento.Undo(undo);
                    State = previousState;
                    break;
            }
        }
    }
}