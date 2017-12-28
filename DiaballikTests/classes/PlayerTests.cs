using System.Windows.Media;
using Diaballik.Core;
using Diaballik.Players;
using NUnit.Framework;
using Color = System.Drawing.Color;

namespace Diaballik.Tests {
    [TestFixture]
    public class PlayerTests {
        [Test]
        public void TestAiLevel() {
            AiPlayer noob = new NoobAiPlayer(Colors.AliceBlue, "dummy");
            Assert.AreEqual(AiLevel.Noob, noob.Level);

            AiPlayer starting = new StartingAiPlayer(Colors.AliceBlue, "dummy");
            Assert.AreEqual(AiLevel.Starting, starting.Level);

            AiPlayer progressive = new ProgressiveAiPlayer(Colors.AliceBlue, "dummy");
            Assert.AreEqual(AiLevel.Progressive, progressive.Level);
        }


        [Test]
        public void TestIPlayerMembers() {
            // tests that the members of *IPlayer* are correctly set
            var player = MockUtil.DummyPlayer(Colors.Aqua, "dummy");
            Assert.AreEqual(player.Color, Colors.Aqua);
            Assert.AreEqual(player.Name, "dummy");
        }
    }
}