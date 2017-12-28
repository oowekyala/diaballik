using System;
using System.Drawing;
using Diaballik.Core;
using NUnit.Framework;
using static System.Drawing.KnownColor;
using Diaballik.Players;

namespace Diaballik.Tests {
    [TestFixture]
    public class PlayerBuilderTest {
        [Test]
        public void TestProperties([Values(Blue, Red, HotPink)] KnownColor color, [Values("a", "b", "c")] string name) {
            var builder = new PlayerBuilder {
                Color = Color.FromKnownColor(color),
                Name = name
            };

            var player = builder.Build();

            Assert.AreEqual(player.Color, Color.FromKnownColor(color));
            Assert.AreEqual(player.Name, name);
        }


        [Test]
        public void TestBuiltType([Values] PlayerType level) {
            var player = new PlayerBuilder {
                Color = Color.DeepPink,
                Name = "dummy",
                SelectedPlayerType = level
            }.Build();


            switch (level) {
                case PlayerType.NoobAi:
                    Assert.AreEqual(player.GetType(), typeof(NoobAiPlayer));
                    break;
                case PlayerType.StartingAi:
                    Assert.AreEqual(player.GetType(), typeof(StartingAiPlayer));
                    break;
                case PlayerType.ProgressiveAi:
                    Assert.AreEqual(player.GetType(), typeof(ProgressiveAiPlayer));
                    break;
                case PlayerType.Human:
                    Assert.AreEqual(player.GetType(), typeof(HumanPlayer));
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(level), level, null);
            }

            Assert.IsInstanceOf<AbstractPlayer>(player);
        }
    }
}