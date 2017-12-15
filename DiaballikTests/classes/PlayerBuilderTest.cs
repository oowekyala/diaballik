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
        public void TestBuildAi([Values] AiLevel level) {
            var builder = new PlayerBuilder {
                Color = Color.DeepPink,
                Name = "dummy"
            };

            var player = builder.SetIsAi(level).Build();

            switch (level) {
                case AiLevel.Noob:
                    Assert.AreEqual(player.GetType(), typeof(NoobAiPlayer));
                    break;
                case AiLevel.Starting:
                    Assert.AreEqual(player.GetType(), typeof(StartingAiPlayer));
                    break;
                case AiLevel.Progressive:
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