using System;
using System.Drawing;
using Diaballik.Core;
using Diaballik.Core.Util;

namespace Diaballik.Players {
    /// <summary>
    ///     Builds a game, enforcing the correctness of its arguments throughout the process.
    /// </summary>
    public class GameBuilder {
        #region Private fields

        #endregion

        #region Properties and setters

        public PlayerBuilder PlayerBuilder1 { get; } = new PlayerBuilder();

        public PlayerBuilder PlayerBuilder2 { get; } = new PlayerBuilder();


        private IInitStrategy InitStrategy => StrategyFromScenario(Scenario);

        public int BoardSize { get; set; } = 7;


        /// Determines where the pieces of each player will be initialised.
        public GameScenario Scenario { get; set; } = GameScenario.Standard;

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
            DiaballikUtil.Assert(BoardSize % 2 == 1 && BoardSize > 1, "The size of the board must be odd and > 1");

            var players = (PlayerBuilder1, PlayerBuilder2).Map(x => x.Build());
            var specs = InitStrategy.InitPositions(BoardSize)
                                    .Zip(players, (spec, player) => new FullPlayerBoardSpec(player, spec));

            return Game.Init(BoardSize, specs);
        }

        #endregion
    }
}