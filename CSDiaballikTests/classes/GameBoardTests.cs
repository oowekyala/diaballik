using System.Collections.Generic;
using System.Drawing;
using NUnit.Framework;

namespace CSDiaballik.Tests
{
    [TestFixture]
    public class GameBoardTests
    {
        [Test]
        public void InitializationTest()
        {
            const int size = 7;
            var piecePos = new Position2D(size, 0);
            var p2Pieces = new List<Piece>();
            var ai = new NoobAiPlayer(Color.Blue, "toto", new List<Position2D>());
            var p1 = new Piece(ai);
            p1.Position = piecePos;
            var p1Pieces = new List<Piece> {p1};
            var gb = new GameBoard(size, p1Pieces, p2Pieces);
            Assert.IsTrue(gb.PositionHasPiece(piecePos));
        }
    }
}
