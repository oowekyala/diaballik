using System;
using System.Windows.Media;
using NUnit.Framework;
using Diaballik.Players;

namespace Diaballik.Tests {
    [TestFixture]
    public class PlayerBuilderTest {
        [Test]
        public void TestProperties([Values("a", "b", "c")] string name) {
            var builder = new PlayerBuilder {
                Color = Colors.Beige,
                Name = name
            };

            var player = builder.Build();

            Assert.AreEqual(player.Color, Colors.Beige);
            Assert.AreEqual(player.Name, name);
        }


        [Test]
        public void TestBuiltType([Values] PlayerType level) {
            var player = new PlayerBuilder {
                Color = Colors.DeepPink,
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