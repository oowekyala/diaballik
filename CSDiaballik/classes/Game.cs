using System;

namespace CSDiaballik {
    /// <summary>
    ///     Represents the game. This class appears mutable to clients.
    /// </summary>
    public class Game {

        public GameMemento Memento { get; }

        public IPlayer Player1 => Board.Player1;
        public IPlayer Player2 => Board.Player2;
        public int BoardSize => Board.Size;
        public GameBoard Board { get; }

        public IPlayer CurrentPlayer { get; }


        private Game(GameBoard board, bool isFirstPlayerPlaying) {
            Board = board;
            CurrentPlayer = isFirstPlayerPlaying ? Player1 : Player2;
            Memento = new RootMemento(this);
        }


        public static Game New(GameBoard board, bool isFirstPlayerPlaying) {
            return new Game(board, isFirstPlayerPlaying);
        }


        public static Game New(GameBoard board) {
            return New(board, new Random().Next(0, 1) == 1);
        }


        /// <summary>
        ///     Updates this game with the given player action. May change the current player as well.
        /// </summary>
        /// <param name="playerAction">The action to be played by the current player</param>
        /// <returns>This game</returns>
        public Game Update(IPlayerAction playerAction) {
            return this;
        }

    }
}
