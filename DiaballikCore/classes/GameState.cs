using System;

namespace Diaballik.Core {
    using FullPlayerSpecPair = ValueTuple<FullPlayerBoardSpec, FullPlayerBoardSpec>;


    /// <summary>
    ///     Represents a state in the game. Decorates a GameBoard with logic about the current
    ///     player and number of moves.
    /// </summary>
    public sealed class GameState : BoardLikeDecorator<GameBoard> {
        /* Core state of the game */
        // The gameboard is decorated by this.

        /// Number of moves left to the current player before automatic player change.
        public int NumMovesLeft { get; } = Game.MaxMovesPerTurn;

        /// Current player of the game
        public IPlayer CurrentPlayer { get; }


        // Called to build an initial state
        private GameState(int size, FullPlayerSpecPair specs, bool isFirstPlayerPlaying)
            : base(GameBoard.Create(size, specs)) {
            CurrentPlayer = isFirstPlayerPlaying ? Player1 : Player2;
        }


        // Called when updating an existing game
        private GameState(GameBoard board, IPlayer currentPlayer, int numMoves) : base(board) {
            CurrentPlayer = currentPlayer;
            NumMovesLeft = numMoves;
        }

        /// <summary>
        ///     Creates an initial state, with the given board size and player specifications.
        /// </summary>
        /// <param name="size">Size of the board</param>
        /// <param name="specs">Player specifications</param>
        /// <param name="isFirstPlayerPlaying">True if the first player will be the first to play</param>
        /// <returns>A new gamestate</returns>
        public static GameState InitialState(int size, FullPlayerSpecPair specs, bool isFirstPlayerPlaying) {
            return new GameState(size, specs, isFirstPlayerPlaying);
        }


        private GameState GetNextStateWithBoard(GameBoard board) {
            var nextPlayer = NumMovesLeft == 1 ? GetOtherPlayer(CurrentPlayer) : CurrentPlayer;
            var nextMovesLeft = NumMovesLeft == 1 ? Game.MaxMovesPerTurn : NumMovesLeft - 1;
            return new GameState(board, nextPlayer, nextMovesLeft);
        }

        /// <summary>
        ///     Moves a ball from src to dst. Returns the updated state.
        /// </summary>
        /// <param name="src">Original position</param>
        /// <param name="dst">New position</param>
        /// <returns>The updated state</returns>
        public GameState MoveBall(Position2D src, Position2D dst) {
            return GetNextStateWithBoard(UnderlyingBoard.MoveBall(src, dst));
        }

        /// <summary>
        ///     Moves a piece from src to dst. Returns the updated state.
        /// </summary>
        /// <param name="src">Original position</param>
        /// <param name="dst">New position</param>
        /// <returns>The updated state</returns>
        public GameState MovePiece(Position2D src, Position2D dst) {
            return GetNextStateWithBoard(UnderlyingBoard.MovePiece(src, dst));
        }

        /// <summary>
        ///     Changes the current player. Returns the updated state.
        /// </summary>
        /// <returns>The updated state</returns>
        public GameState Pass() {
            return new GameState(UnderlyingBoard, GetOtherPlayer(CurrentPlayer), Game.MaxMovesPerTurn);
        }


        private bool Equals(GameState other) {
            return UnderlyingBoard.Equals(other.UnderlyingBoard)
                   && NumMovesLeft == other.NumMovesLeft
                   && CurrentPlayer.Equals(other.CurrentPlayer);
        }


        public override bool Equals(object obj) {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((GameState) obj);
        }


        public override int GetHashCode() {
            unchecked {
                var hashCode = UnderlyingBoard.GetHashCode();
                hashCode = (hashCode * 397) ^ NumMovesLeft;
                hashCode = (hashCode * 397) ^ CurrentPlayer.GetHashCode();
                return hashCode;
            }
        }

        public static bool operator ==(GameState left, GameState right) {
            return Equals(left, right);
        }

        public static bool operator !=(GameState left, GameState right) {
            return !Equals(left, right);
        }

        public override string ToString() {
            return $"GameState(numMovesLeft: {NumMovesLeft}, player: {CurrentPlayer})";
        }

        public string FullDescription() {
            return $"{this} {{\n{UnderlyingBoard.ToString().PadLeft(4)}\n}}";
        }
    }
}