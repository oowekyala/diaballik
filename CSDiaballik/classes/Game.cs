using System;

namespace CSDiaballik {
    /// <summary>
    ///     Represents the game. This class appears mutable to clients.
    /// </summary>
    public class Game {

        private readonly GameBoard _board;
        private readonly GameMemento _lastMemento;

        public GameMemento Memento => _lastMemento;
        public IPlayer Player1 => _board.Player1;
        public IPlayer Player2 => _board.Player2;
        public int BoardSize => _board.Size;
        public GameBoard Board => _board;

        public IPlayer CurrentPlayer { get; }


        private Game(GameBoard board, bool isFirstPlayerPlaying) {
            _board = board;
            CurrentPlayer = isFirstPlayerPlaying ? Player1 : Player2;
            _lastMemento = new RootMemento(this);
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
