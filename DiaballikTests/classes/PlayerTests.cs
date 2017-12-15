using System.Drawing;
using Diaballik.Core;
using Diaballik.Players;
using NUnit.Framework;

namespace Diaballik.Tests {
    [TestFixture]
    public class PlayerTests {
        [Test]
        public void TestAiLevel() {
            AiPlayer noob = new NoobAiPlayer(Color.AliceBlue, "dummy");
            Assert.AreEqual(AiLevel.Noob, noob.Level);

            AiPlayer starting = new StartingAiPlayer(Color.AliceBlue, "dummy");
            Assert.AreEqual(AiLevel.Starting, starting.Level);

            AiPlayer progressive = new ProgressiveAiPlayer(Color.AliceBlue, "dummy");
            Assert.AreEqual(AiLevel.Progressive, progressive.Level);
        }


        [Test]
        public void TestIPlayerMembers() {
            // tests that the members of *IPlayer* are correctly set
            var player = TestUtil.DummyPlayer(Color.Aqua, "dummy");
            Assert.AreEqual(player.Color, Color.Aqua);
            Assert.AreEqual(player.Name, "dummy");
        }
    }
}