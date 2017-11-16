using System;
using System.Collections.Generic;
using System.Drawing;

namespace CSDiaballik
{
    /// <summary>
    /// Builds player instances.
    /// </summary>
    public class PlayerBuilder
    {
        private static bool _isFirstPlayer;
        private AiPlayer.AiLevel _aiLevel;
        private List<Position2D> _piecesList;

        public PlayerBuilder()
        {
            _isFirstPlayer = !_isFirstPlayer;
            Color = _isFirstPlayer ? Color.Blue : Color.Red;
        }

        public PlayerBuilder SetIsAi(AiPlayer.AiLevel level)
        {
            _aiLevel = level;
            return this;
        }

        public PlayerBuilder Pieces(List<Position2D> pos)
        {
            _piecesList = pos;
            return this;
        }

        public Color Color { set; get; }

        public string Name { set; get; }

        /// <summary>
        /// Builds a player with the specified configuration.
        /// </summary>
        /// <returns>A new player</returns>
        public IPlayer Build()
        {

            switch (_aiLevel)
            {
                case AiPlayer.AiLevel.Noob:
                    break;
                case AiPlayer.AiLevel.Starting:
                    break;
                case AiPlayer.AiLevel.Progressive:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            
            
        }
    }

    public class GameBuilder
    {
        public IInitStrategy InitStrategy { private get; set; }

        private PlayerBuilder _playerBuilder1 = new PlayerBuilder();
        private PlayerBuilder _playerBuilder2 = new PlayerBuilder();

        public GameBuilder()
        {
        }


        public GameBuilder PlayerColors(Color c1, Color c2)
        {
            _playerBuilder1.Color = c1;
            _playerBuilder2.Color = c2;
            return this;
        }


        public Game Build()
        {
            var board = InitStrategy.InitBoard(_playerBuilder1, _playerBuilder2);
            var player1 = _playerBuilder1.Build();
            var player2 = _playerBuilder2.Build();

            return new Game(board, player1, player2);
        }
    }
}