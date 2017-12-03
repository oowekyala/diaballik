using System;

namespace CSDiaballik {
    /// <summary>
    ///     Represents the game. This class appears mutable to clients.
    /// </summary>
    public class Game {

        private readonly GameBoard _board;
        private GameMemento _lastMemento;


        public Game(GameBoard board, IPlayer player1, IPlayer player2)
            : this(board, player1, player2, new Random().Next(0, 1) == 1) {
        }


        public Game(GameBoard board, IPlayer player1, IPlayer player2, bool isFirstPlayerPlaying) {
            _board = board;
            Player1 = player1;
            Player2 = player2;

            CurrentPlayer = isFirstPlayerPlaying ? player1 : player2;

            _lastMemento = new RootMemento(this);
        }


        public IPlayer Player1 { get; }
        public IPlayer Player2 { get; }
        public IPlayer CurrentPlayer { get; }
        public int BoardSize => _board.Size;


        /// <summary>
        ///     Updates this game with the given player action. May change the current player as well.
        /// </summary>
        /// <param name="playerAction">The action to be played by the current player</param>
        /// <returns>This game</returns>
        public Game Update(PlayerAction playerAction) {
            return this;
        }

    }
}
