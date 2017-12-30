using System;
using Diaballik.Core.Util;

namespace Diaballik.Core.Builders {
    /// <summary>
    ///     Builds a game, enforcing the correctness of its arguments throughout the process.
    /// </summary>
    public sealed class GameBuilder {
        #region Private fields

        #endregion

        #region Properties and setters

        public PlayerBuilder PlayerBuilder1 { get; } = new PlayerBuilder();

        public PlayerBuilder PlayerBuilder2 { get; } = new PlayerBuilder();


        private IInitStrategy InitStrategy => StrategyFromScenario(Scenario);

        public int BoardSize { get; set; } = 7;


        /// Determines where the pieces of each player will be initialised.
        public GameScenario Scenario { get; set; } = GameScenario.Standard;

        public bool CannotBuild => !CanBuild;
        public bool CanBuild => ErrorMessage == string.Empty;

        public string ErrorMessage =>
            PlayerBuilder1.CannotBuild
                ? $"Player 1 error: {PlayerBuilder1.ErrorMessage}"
                : PlayerBuilder2.CannotBuild
                    ? $"Player 2 error: {PlayerBuilder2.ErrorMessage}"
                    : PlayerBuilder1.Color == PlayerBuilder2.Color
                        ? "Players cannot have the same color"
                        : BoardSize % 2 == 0 || BoardSize < 3
                            ? "Board size must be odd and >= 3"
                            : string.Empty;


        public enum GameScenario {
            Standard,
            BallRandom,
            EnemyAmongUs
        }

        public static IInitStrategy StrategyFromScenario(GameScenario scenario) {
            switch (scenario) {
                case GameScenario.Standard: return new StandardInitStrategy();
                case GameScenario.BallRandom: return new BallRandomStrategy();
                case GameScenario.EnemyAmongUs: return new EnemyAmongUsStrategy();
                default:
                    throw new ArgumentOutOfRangeException(nameof(scenario), scenario, null);
            }
        }

        #endregion

        #region Build method

        /// <summary>
        ///     Builds and returns the game specified by this builder.
        /// </summary>
        /// <returns>A new game</returns>
        public Game Build() {
            DiaballikUtil.Assert(CanBuild, ErrorMessage);

            var players = (PlayerBuilder1, PlayerBuilder2).Map(x => x.Build());
            var specs = InitStrategy.InitPositions(BoardSize)
                                    .Zip(players, (spec, player) => new FullPlayerBoardSpec(player, spec));

            return Game.Init(BoardSize, specs);
        }

        #endregion
    }
}