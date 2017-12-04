using System;
using System.Drawing;

namespace CSDiaballik {
    /// <summary>
    ///     Builds a game, enforcing the correctness of its arguments throughout the process.
    /// </summary>
    public class GameBuilder {

        private readonly PlayerBuilder _playerBuilder1 = new PlayerBuilder();
        private readonly PlayerBuilder _playerBuilder2 = new PlayerBuilder();
        private int _size = 7;


        public IInitStrategy InitStrategy { get; set; } = new StandardInitStrategy();

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
            var players = (_playerBuilder1, _playerBuilder2).Map(x => x.Build());
            var specs = InitStrategy.InitPositions(Size);
            var fullSpecs = specs.Zip(players, (spec, player) => new FullPlayerBoardSpec(player, spec));

            var game = Game.New(GameBoard.New(Size, fullSpecs));
            (game.Memento as RootMemento)?.SetBoardSpecs(specs);
            return game;
        }

    }
}
