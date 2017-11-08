using System;

namespace CSDiaballik
{
    public class Game
    {
        private GameBoard _board;
        private GameMemento _lastMemento;
        public IPlayer Player1 { get; }
        public IPlayer Player2 { get; }
        public IPlayer CurrentPlayer { get; }


        public Game(GameBoard board, IPlayer player1, IPlayer player2)
        {
            _board = board;
            Player1 = player1;
            Player2 = player2;

            CurrentPlayer = new Random().Next(0, 2) == 0 ? player1 : player2;
        }


        /// <summary>
        /// Updates the game with the given player action. May change the current player as well.
        /// </summary>
        /// <param name="playerAction">The action to be played by the current player</param>
        public void Update(PlayerAction playerAction)
        {
            throw new NotImplementedException();
        }
        
        
        
    }
}