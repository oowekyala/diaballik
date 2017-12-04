using System.Linq;
using NUnit.Framework;

namespace CSDiaballik.Tests {
    [TestFixture]
    public class GameBuilderTests {

        [Test]
        public void TestSetSize([Range(1, 15)] int size) {
            var builder = new GameBuilder();

            if (size % 2 == 0 || size < 2) {
                Assert.That(() => builder.Size = size, Throws.ArgumentException);
            }
            else {
                builder.Size = size;
                var game = builder.Build();
                Assert.AreEqual(size, game.BoardSize);
            }
        }


        [Test]
        public void TestStandardInitStrategy([Range(3, 15, 2)] int size) {
            var game = new GameBuilder {
                Size = size,
                InitStrategy = new StandardInitStrategy()
            }.Build();
            var board = game.Board;
            var (p1Pos, p2Pos) = (board.Player1Positions, board.Player2Positions).Map(x => x.ToList());
            var bothPositions = (p1Pos, p2Pos);

            bothPositions.Foreach(l => Assert.AreEqual(size, l.Count));

            p1Pos.ForEach(x => Assert.AreEqual(size - 1, x.X)); // bottom row
            p2Pos.ForEach(x => Assert.AreEqual(0, x.X)); // top row

            // ball bearers are middle pieces
            (board.BallBearer1, board.BallBearer2).Foreach(x => Assert.AreEqual(size / 2, x.Y)); 
        }

        
        
    }
}
