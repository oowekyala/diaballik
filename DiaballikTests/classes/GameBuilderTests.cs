using System.Linq;
using Diaballik.Core;
using Diaballik.Core.Util;
using Diaballik.Players;
using NUnit.Framework;

namespace Diaballik.Tests {
    [TestFixture]
    public class GameBuilderTests {
        [Test]
        public void TestSetSize([Range(1, 15)] int size) {
            var builder = new GameBuilder();

            if (size % 2 == 0 || size < 2) {
                Assert.That(() => builder.Size = size, Throws.ArgumentException);
            } else {
                builder.Size = size;
                var game = builder.Build();
                Assert.AreEqual(size, game.BoardSize);
            }
        }


        [Test]
        public void TestStandardInitStrategy([Range(3, 15, 2)] int size) {
            var builder = new GameBuilder {
                Size = size,
                InitStrategy = new StandardInitStrategy()
            };
            var game = builder.Build();
            var (p1Pos, p2Pos) = game.PositionsPair.Map(x => x.ToList());

            Assert.AreEqual(size, p1Pos.Count);
            Assert.AreEqual(size, p2Pos.Count);

            p1Pos.ForEach(x => Assert.AreEqual(size - 1, x.X)); // bottom row
            p2Pos.ForEach(x => Assert.AreEqual(0, x.X)); // top row

            // ball bearers are middle pieces
            game.BallBearerPair.Foreach(x => Assert.AreEqual(size / 2, x.Y));
        }


        [Test]
        public void TestBallRandomInitStrategy([Range(3, 15, 2)] int size) {
            var builder = new GameBuilder {
                Size = size,
                InitStrategy = new BallRandomStrategy()
            };
            var game = builder.Build();
            var (p1Pos, p2Pos) = game.PositionsPair.Map(x => x.ToList());

            game.PositionsPair.Foreach(ps => Assert.AreEqual(size, ps.Count()));

            p1Pos.ForEach(x => Assert.AreEqual(size - 1, x.X)); // bottom row
            p2Pos.ForEach(x => Assert.AreEqual(0, x.X)); // top row

            // ball bearers can be any piece in the line
        }


        [Test]
        public void TestEnemyAmongUsInitStrategy([Range(3, 15, 2)] int size) {
            var builder = new GameBuilder {
                Size = size,
                InitStrategy = new EnemyAmongUsStrategy()
            };
            var game = builder.Build();
            var positionsTuple = game.PositionsPair.Map(x => x.ToList());

            positionsTuple.Foreach(ps => Assert.AreEqual(size, ps.Count));

            positionsTuple.Zip((size - 1, 0))
                .Foreach(t => {
                    var (ps, row) = t;
                    Assert.AreEqual(size - 2, ps.Count(p => p.X == row)); // friend row
                    Assert.AreEqual(2, ps.Count(p => p.X == size - 1 - row)); // enemy row
                });

            // ball bearers are middle pieces, in friend row
            game.BallBearerPair
                .Zip((size - 1, 0))
                .Foreach(t => {
                    var (ball, row) = t;
                    Assert.AreEqual(size / 2, ball.Y);
                    Assert.AreEqual(row, ball.X);
                });
        }
    }
}