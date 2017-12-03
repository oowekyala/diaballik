using System;
using System.Drawing;

namespace CSDiaballik {
    /// <summary>
    ///     Builds a game, enforcing the correctness of its arguments throughout the process.
    /// </summary>
    public class GameBuilder {

        private readonly PlayerBuilder _playerBuilder1 = new PlayerBuilder();
        private readonly PlayerBuilder _playerBuilder2 = new PlayerBuilder();
        private int _size;


        public IInitStrategy InitStrategy { get; set; }

        public int Size {
            get => _size;
            set => _size = value % 2 == 1 && value > 1
                               ? value
                               : throw new ArgumentException("The size of the board must be odd and > 1");
        }


        public GameBuilder PlayerColors(Color c1, Color c2) {
            _playerBuilder1.Color = c1;
            _playerBuilder2.Color = c2;
            return this;
        }


        public Game Build() {
            var player1 = _playerBuilder1.Build();
            var player2 = _playerBuilder2.Build();

            var specs = InitStrategy.InitPositions(Size);

            var board = GameBoard.New(Size, new FullPlayerBoardSpec(player1, specs.Item1),
                                      new FullPlayerBoardSpec(player2, specs.Item2));

            return new Game(board, player1, player2);
        }

    }
}
