using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using static Diaballik.Core.PlayerAction;

namespace Diaballik.Core {
    /// <summary>
    ///     Represents a state in the game. This class is immutable.
    /// </summary>
    public class GameState {
        /* delegated properties */
        public IPlayer Player1 => Board.Player1;

        public IPlayer Player2 => Board.Player2;
        public int BoardSize => Board.Size;
        public (IEnumerable<Position2D>, IEnumerable<Position2D>) PositionsPair => Board.PositionsPair();
        public (Position2D, Position2D) BallBearerPair => Board.BallBearerPair();


        /* Core state of the game */
        /// Underlying gameboard
        private GameBoard Board { get; }

        /// Number of moves left to the current player before automatic player change.
        public int NumMovesLeft { get; } = Game.MaxMovesPerTurn;

        /// Current player of the game
        public IPlayer CurrentPlayer { get; }


        // Called to build an initial state
        private GameState(int size, (FullPlayerBoardSpec, FullPlayerBoardSpec) specs, bool isFirstPlayerPlaying) {
            Board = GameBoard.Create(size, specs);
            CurrentPlayer = isFirstPlayerPlaying ? Player1 : Player2;
        }


        // Called when updating an existing game
        private GameState(GameBoard board, IPlayer currentPlayer, int numMoves) {
            Board = board;
            CurrentPlayer = currentPlayer;
            NumMovesLeft = numMoves;
        }


        public static GameState InitialState(int size, (FullPlayerBoardSpec, FullPlayerBoardSpec) specs,
            bool isFirstPlayerPlaying) {
            return new GameState(size, specs, isFirstPlayerPlaying);
        }




        public GameState MoveBall(Position2D src, Position2D dst) {
            var nextPlayer = NumMovesLeft == 1 ? Board.GetOtherPlayer(CurrentPlayer) : CurrentPlayer;
            return new GameState(Board.MoveBall(src, dst), nextPlayer, NumMovesLeft - 1);
        }


        public GameState MovePiece(Position2D src, Position2D dst) {
            var nextPlayer = NumMovesLeft == 1 ? Board.GetOtherPlayer(CurrentPlayer) : CurrentPlayer;
            return new GameState(Board.MovePiece(src, dst), nextPlayer, NumMovesLeft - 1);
        }


        public GameState Pass() {
            return new GameState(Board, Board.GetOtherPlayer(CurrentPlayer), 3);
        }


        /// <summary>
        ///     Returns true is the given player action is valid in the 
        ///     context of this game state.
        /// </summary>
        /// <param name="action">The action to check</param>
        /// <returns>True if the action can be performed on this state</returns>
        public bool IsMoveValid(PlayerAction action) {
            return action.IsMoveValid(CurrentPlayer, Board, NumMovesLeft);
        }


        protected bool Equals(GameState other) {
            return Equals(Board, other.Board)
                   && NumMovesLeft == other.NumMovesLeft
                   && Equals(CurrentPlayer, other.CurrentPlayer);
        }


        public override bool Equals(object obj) {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj.GetType() == GetType() && Equals((GameState) obj);
        }


        public override int GetHashCode() {
            unchecked {
                var hashCode = Board != null ? Board.GetHashCode() : 0;
                hashCode = (hashCode * 397) ^ NumMovesLeft;
                hashCode = (hashCode * 397) ^ (CurrentPlayer != null ? CurrentPlayer.GetHashCode() : 0);
                return hashCode;
            }
        }
    }
}