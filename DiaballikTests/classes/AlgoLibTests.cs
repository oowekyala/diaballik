using System;
using System.Collections.Generic;
using System.Linq;
using Diaballik.AlgoLib;
using Diaballik.Core;
using NUnit.Framework;
using static Diaballik.AlgoLib.BoardAnalysis;
using static Diaballik.Tests.TestUtil;

namespace Diaballik.Tests {
    [TestFixture]
    public class AlgoLibTests {
        [Test]
        public void TestSimpleMoves([Range(3, 13)] int size) {
            var specs = (size - 1, 0).Map(row => Enumerable.Range(0, size).Select(y => new Position2D(row, y)))
                                     .Map(ps => DummyPlayerSpec(size, ps));

            var board = GameBoard.Create(size, specs);

            var possibleMoves = board.MovesForPiece(new Position2D(0, 0)).ToList();
            var possibleMovesBall = board.MovesForBall(board.BallBearer1);

            Assert.AreEqual(1, possibleMoves.Count); // direct front 
            Assert.AreEqual(2, possibleMovesBall.Count()); // direct left and right
        }


        [Test]
        public void TestMovesForPiece([Range(3, 13)] int size) {
            var specs = DummyPlayerSpecPair(size);
            var board = GameBoard.Create(size, specs);

            foreach (var p in board.Player1Positions.Union(board.Player2Positions)) {
                CollectionAssert.AreEquivalent(MovesForPieceReference(board, p).ToList(),
                                               board.MovesForPiece(p).ToList());
            }
        }

        [Test]
        public void TestMovesForBall([Range(3, 17)] int size) {
            var specs = DummyPlayerSpecPair(size);
            var board = GameBoard.Create(size, specs);

            board.BallBearerPair()
                 .Foreach(p => {
                     Console.WriteLine(board);
                     CollectionAssert.AreEquivalent(MovesForBallReference(board, p).ToList(),
                                                    board.MovesForBall(p).ToList(), "{0}\n{1}", p, board);
                 });
        }


        private static IEnumerable<Position2D> MovesForBallReference(GameBoard board, Position2D src) {
            return board.PositionsForPlayer(board.PlayerOn(src))
                        .Where(dst => src != dst)
                        // IsLineFreeBetween and the current BoardAnalysis algo 
                        // have been extensively debugged and could maybe be trusted
                        // to validate each other
                        .Where(dst => board.IsLineFreeBetween(src, dst));
        }

        // in this we trust
        private static IEnumerable<Position2D> MovesForPieceReference(GameBoard board, Position2D p) {
            // [R21_11_GAMEPLAY_MOVE_PIECE_WITH_BALL]
            // A piece shall not move if it carries the ball.
            if (board.HasBall(p)) {
                return Enumerable.Empty<Position2D>();
            }

            var neighbours =
                new List<Position2D> {
                    new Position2D(p.X - 1, p.Y),
                    new Position2D(p.X + 1, p.Y),
                    new Position2D(p.X, p.Y - 1),
                    new Position2D(p.X, p.Y + 1)
                };

            // [R21_10_GAMEPLAY_MOVE_PIECE]
            // A piece shall be moved to the direct left, right, up, or bottom tile if free.
            return neighbours.Where(board.IsOnBoard).Where(board.IsFree);
        }
    }
}