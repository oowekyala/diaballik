using System;
using System.Drawing;
using NUnit.Framework;
using static System.Drawing.KnownColor;

namespace CSDiaballik.Tests {
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
        public void TestBuildAi([Values] AiPlayer.AiLevel level) {
            var builder = new PlayerBuilder {
                Color = Color.DeepPink,
                Name = "dummy"
            };

            var player = builder.SetIsAi(level).Build();

            switch (level) {
                case AiPlayer.AiLevel.Noob:
                    Assert.AreEqual(player.GetType(), typeof(NoobAiPlayer));
                    break;
                case AiPlayer.AiLevel.Starting:
                    Assert.AreEqual(player.GetType(), typeof(StartingAiPlayer));
                    break;
                case AiPlayer.AiLevel.Progressive:
                    Assert.AreEqual(player.GetType(), typeof(ProgressiveAiPlayer));
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(level), level, null);
            }

            Assert.IsTrue(player.IsAi());
            Assert.IsInstanceOf<AiPlayer>(player);
        }

    }
}
