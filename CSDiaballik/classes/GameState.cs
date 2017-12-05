using System;
using System.Collections.Generic;
using System.Diagnostics;
using static CSDiaballik.PlayerAction;

namespace CSDiaballik {
    /// <summary>
    ///     Represents a state in the game. This class is immutable.
    /// </summary>
    public class GameState {

        public GameMemento Memento { get; }

        public IPlayer Player1 => Board.Player1;
        public IPlayer Player2 => Board.Player2;
        public int BoardSize => Board.Size;
        private GameBoard Board { get; }
        public int NumMovesLeft { get; } = 3;

        public (IEnumerable<Position2D>, IEnumerable<Position2D>) PositionsPair => Board.PositionsPair();
        public (Position2D, Position2D) BallBearerPair => Board.BallBearerPair();

        public IPlayer CurrentPlayer { get; }


        private GameState(GameBoard board, bool isFirstPlayerPlaying) {
            Board = board;
            CurrentPlayer = isFirstPlayerPlaying ? Player1 : Player2;
            Memento = new RootMemento(this);
        }


        private GameState(GameState state, PlayerAction action, GameBoard board,
                          IPlayer currentPlayer, int numMoves) {
            Memento = new MementoNode(state, action);
            Board = board;
            CurrentPlayer = currentPlayer;
            NumMovesLeft = numMoves;
        }


        public static GameState New(GameBoard board, bool isFirstPlayerPlaying) {
            return new GameState(board, isFirstPlayerPlaying);
        }


        public static GameState New(GameBoard board) {
            return New(board, new Random().Next(0, 1) == 1);
        }


        private IPlayer GetOtherPlayer(IPlayer player) => player == Player1 ? Player2 :
                                                          player == Player2 ? Player1 :
                                                          throw new ArgumentException("Unknown player");


        /// <summary>
        ///     Returns a game state updated with the given player action. 
        ///     May change the current player as well.
        /// </summary>
        /// <param name="action">The action to be played by the current player</param>
        /// <returns>The updated game</returns>
        public GameState Update(PlayerAction action) {
            if (!action.IsMoveValid(CurrentPlayer, Board, NumMovesLeft)) {
                throw new ArgumentException("Invalid move: " + action);
            }

            var nextPlayer = NumMovesLeft == 1 ? GetOtherPlayer(CurrentPlayer) : CurrentPlayer;

            switch (action) {
                case MoveBall moveBall:
                    return new GameState(this, action, Board.MoveBall(moveBall.Src, moveBall.Dst),
                                         nextPlayer, NumMovesLeft - 1);
                case MovePiece movePiece:
                    return new GameState(this, action, Board.MovePiece(movePiece.Src, movePiece.Dst),
                                         nextPlayer, NumMovesLeft - 1);
                case Pass pass:
                    return new GameState(this, action, Board, GetOtherPlayer(CurrentPlayer), 3);
                case Undo undo:
                    return Memento.ToGame();
            }
            return this;
        }

    }
}
