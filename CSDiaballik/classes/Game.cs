using System;

namespace CSDiaballik
{
    /// <summary>
    ///     Represents the game. This class should be immutable to clients.
    /// </summary>
    public class Game
    {
        private readonly GameBoard _board;
        private GameMemento _lastMemento;


        public Game(GameBoard board, IPlayer player1, IPlayer player2)
            : this(board, player1, player2, new Random().Next(0, 1) == 1)
        {
        }


        public Game(GameBoard board, IPlayer player1, IPlayer player2, bool isFirstPlayerPlaying)
        {
            _board = board;
            Player1 = player1;
            Player2 = player2;

            CurrentPlayer = isFirstPlayerPlaying ? player1 : player2;
        }


        public IPlayer Player1 { get; }
        public IPlayer Player2 { get; }
        public IPlayer CurrentPlayer { get; }
        public int BoardSize => _board.Size;


        /// <summary>
        ///     Creates a new game, updated with the given player action. May change the current player as well.
        /// </summary>
        /// <param name="playerAction">The action to be played by the current player</param>
        /// <returns>The updated Game</returns>
        public Game Update(PlayerAction playerAction)
        {
            throw new NotImplementedException();
            return this;
        }
    }
}
