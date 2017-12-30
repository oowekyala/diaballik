using System.Windows.Media;
using Diaballik.Core;
using Diaballik.Core.Builders;
using NUnit.Framework;

namespace Diaballik.Tests {
    [TestFixture]
    public class PlayerBuilderTest {
        [Test]
        public void TestProperties([Values] PlayerType type, [Values("a", "b", "c")] string name) {
            var builder = new PlayerBuilder {
                Color = Colors.Beige,
                Name = name,
                SelectedPlayerType = type
            };

            var player = builder.Build();

            Assert.AreEqual(player.Color, Colors.Beige);
            Assert.AreEqual(player.Name, name);
            Assert.AreEqual(player.Type, type);
        }
    }
}