using System;
using System.Drawing;

namespace CSDiaballik
{
    public class GameBuilder
    {
        public IInitStrategy InitStrategy { get; set; }

        public int Size
        {
            get => _size;
            set => _size = value % 2 == 1 && value > 1
                               ? value
                               : throw new ArgumentException("The size of the board must be odd");
        }

        private readonly PlayerBuilder _playerBuilder1 = new PlayerBuilder();
        private readonly PlayerBuilder _playerBuilder2 = new PlayerBuilder();
        private int _size;

        public GameBuilder()
        {
        }


        public GameBuilder PlayerColors(Color c1, Color c2)
        {
            _playerBuilder1.Color = c1;
            _playerBuilder2.Color = c2;
            return this;
        }

        private static IPlayer CreatePlayer(PlayerBoardSpec spec, PlayerBuilder builder)
        {
            var player = builder.Pieces(spec.Positions).Build();
            player.BallBearer = player.Pieces.Find(p => p.Position.Y == spec.Ball);
            return player;
        }

        public Game Build()
        {
            var specs = InitStrategy.InitPositions(Size, _playerBuilder1, _playerBuilder2);

            var player1 = CreatePlayer(specs.Item1, _playerBuilder1);
            var player2 = CreatePlayer(specs.Item2, _playerBuilder2);

            var board = new GameBoard(Size, player1.Pieces, player2.Pieces);

            return new Game(board, player1, player2);
        }
    }
}
