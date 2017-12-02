﻿using System.Drawing;
using System.Linq;
using NUnit.Framework;

namespace CSDiaballik.Tests
{
    [TestFixture]
    public class PlayerTests
    {
        [Test]
        public void TestPiecesHavePositions()
        {
            var positions = TestUtil.RandomPositions(15, 13).ToList();
            var player = new NoobAiPlayer(Color.Aqua, "dummy", positions);

            CollectionAssert.AreEquivalent(positions, player.Pieces.Select(p => p.Position));
        }


        [Test]
        public void TestPiecesMetadata()
        {
            var positions = TestUtil.RandomPositions(15, 13).ToList();
            var player = new NoobAiPlayer(Color.Aqua, "dummy", positions);

            foreach (var piece in player.Pieces)
            {
                Assert.AreSame(player, piece.Player);
                Assert.AreEqual(piece.Color, player.Color);
            }

            CollectionAssert.AreEquivalent(positions, player.Pieces.Select(p => p.Position));
        }


        [Test]
        public void TestIPlayerMembers()
        {
            // tests that the members of *IPlayer* are correctly set
            IPlayer player = new NoobAiPlayer(Color.Aqua, "dummy", Enumerable.Empty<Position2D>());
            Assert.AreEqual(player.Color, Color.Aqua);
            Assert.AreEqual(player.Name, "dummy");
        }


        [Test]
        public void TestAiLevel()
        {
            AiPlayer noob = new NoobAiPlayer(Color.AliceBlue, "dummy", Enumerable.Empty<Position2D>());
            Assert.AreEqual(AiPlayer.AiLevel.Noob, noob.Level);

            AiPlayer starting = new StartingAiPlayer(Color.AliceBlue, "dummy", Enumerable.Empty<Position2D>());
            Assert.AreEqual(AiPlayer.AiLevel.Starting, starting.Level);

            AiPlayer progressive = new ProgressiveAiPlayer(Color.AliceBlue, "dummy", Enumerable.Empty<Position2D>());
            Assert.AreEqual(AiPlayer.AiLevel.Progressive, progressive.Level);
        }
    }
}
