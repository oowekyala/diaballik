using System.Drawing;

namespace CSDiaballik
{
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