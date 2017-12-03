using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using NUnit.Framework;

namespace CSDiaballik.Tests {
    [TestFixture]
    public class PlayerTests {

        [Test]
        public void TestAiLevel() {
            AiPlayer noob = new NoobAiPlayer(Color.AliceBlue, "dummy");
            Assert.AreEqual(AiPlayer.AiLevel.Noob, noob.Level);

            AiPlayer starting = new StartingAiPlayer(Color.AliceBlue, "dummy");
            Assert.AreEqual(AiPlayer.AiLevel.Starting, starting.Level);

            AiPlayer progressive = new ProgressiveAiPlayer(Color.AliceBlue, "dummy");
            Assert.AreEqual(AiPlayer.AiLevel.Progressive, progressive.Level);
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
