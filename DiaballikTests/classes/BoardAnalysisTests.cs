using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Diaballik.AlgoLib;
using Diaballik.Core;
using Diaballik.Core.Util;
using NUnit.Framework;
using static Diaballik.Tests.MockUtil;

namespace Diaballik.Tests {
    [TestFixture]
    public class BoardAnalysisTests {
        // MovesForPiece is easy to check so we test it with a reference implementation.
        // MovesForBall is harder, in fact, the reference implementation was the one that
        // needed debugging in the beginning. We now test for regression against a handful 
        // of trusted test cases.

        [Test]
        public void TestMovesForPiece([Range(3, 13)] int size) {
            var specs = DummyPlayerSpecPair(size);
            var board = GameBoard.Create(size, specs);

            foreach (var p in board.Player1Positions.Union(board.Player2Positions)) {
                CollectionAssert.AreEquivalent(MovesForPieceReference(board, p).ToList(),
                                               board.MovesForPiece(p).ToList());
            }
        }

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


        [Test]
        // this is not so much a reference benchmark as a way to check that IsLineFreeBetween and MovesForBall are in sync.
        public void TestMovesForBall([Range(3, 17)] int size) {
            var specs = DummyPlayerSpecPair(size);
            var board = GameBoard.Create(size, specs);

            // Console.WriteLine(board); // to generate test cases
            // Console.WriteLine(GenerateTestCase(size, specs.Map(s => s.Positions)));

            board.BallCarrierPair
                 .Foreach(p => CollectionAssert.AreEquivalent(MovesForBallReference(board, p).ToList(),
                                                              board.MovesForBall(p).ToList(), "{0}\n{1}", p));
        }

        private static IEnumerable<Position2D> MovesForBallReference(GameBoard board, Position2D src) {
            return board.PositionsForPlayer(board.PlayerOn(src))
                        .Where(dst => src != dst)
                        .Where(dst => board.IsLineFreeBetween(src, dst));
        }

        /// <summary>
        ///    Assertions for a generated test case. Called by the test case when it's done constructing the board.
        /// </summary>
        /// <param name="board">The board described by the test case</param>
        /// <param name="expectedp1">Expected reachable positions of the player 1's ball</param>
        /// <param name="expectedp2">Expected reachable positions of the player 2's ball</param>
        private static void TestCaseAssert(GameBoard board, IList<Position2D> expectedp1,
            IList<Position2D> expectedp2) {
            board.BallCarrierPair
                 .Map(board.MovesForBall)
                 .Zip((expectedp1, expectedp2))
                 .Foreach(t => CollectionAssert.AreEquivalent(t.Item2, t.Item1, "BoardAnalysis.MovesForBall failed"));
            board.BallCarrierPair
                 .Map(bb => MovesForBallReference(board, bb))
                 .Zip((expectedp1, expectedp2))
                 .Foreach(t => CollectionAssert.AreEquivalent(t.Item2, t.Item1, "MovesForBallReference failed"));
        }


        #region Generated test cases for MoveBall tests

        //     0  1  2  3  4  5  6
        //  0        o  x         
        //  1        x  O  o     x
        //  2        o            
        //  3           o         
        //  4     x     o        x
        //  5                 x  o
        //  6           X         

        [Test]
        public void TestMoveBallScenario1273435222() {
            const int size = 7;
            var p1 = new List<Position2D> {
                new Position2D(1, 2),
                new Position2D(5, 5),
                new Position2D(4, 6),
                new Position2D(6, 3),
                new Position2D(4, 1),
                new Position2D(0, 3),
                new Position2D(1, 6),
            };
            var p2 = new List<Position2D> {
                new Position2D(1, 4),
                new Position2D(0, 2),
                new Position2D(5, 6),
                new Position2D(1, 3),
                new Position2D(3, 3),
                new Position2D(4, 3),
                new Position2D(2, 2),
            };
            var specs = (p1, p2).Map(ps => new FullPlayerBoardSpec(DummyPlayer(), ps, size / 2));
            var board = GameBoard.Create(size, specs);
            var expectedp1 = new List<Position2D> {
                new Position2D(4, 1),
            };
            var expectedp2 = new List<Position2D> {
                new Position2D(3, 3),
                new Position2D(1, 4),
                new Position2D(0, 2),
                new Position2D(2, 2),
            };
            TestCaseAssert(board, expectedp1, expectedp2);
        }

        //     0  1  2  3  4  5  6  7  8  9 10 11
        //  0                 o  o              x
        //  1                                   o
        //  2                 O           o      
        //  3  x        o     x                 x
        //  4  o                                o
        //  5     x  o              x            
        //  6                                    
        //  7           x                        
        //  8        x        o                  
        //  9  x                 x        x      
        // 10                          X         
        // 11              o                    o

        [Test]
        public void TestMoveBallScenario2016331125() {
            const int size = 12;
            var p1 = new List<Position2D> {
                new Position2D(3, 5),
                new Position2D(9, 6),
                new Position2D(3, 11),
                new Position2D(5, 1),
                new Position2D(0, 11),
                new Position2D(3, 0),
                new Position2D(10, 8),
                new Position2D(5, 7),
                new Position2D(8, 2),
                new Position2D(9, 9),
                new Position2D(7, 3),
                new Position2D(9, 0),
            };
            var p2 = new List<Position2D> {
                new Position2D(3, 3),
                new Position2D(0, 5),
                new Position2D(11, 4),
                new Position2D(4, 0),
                new Position2D(1, 11),
                new Position2D(8, 5),
                new Position2D(2, 5),
                new Position2D(11, 11),
                new Position2D(4, 11),
                new Position2D(0, 6),
                new Position2D(2, 9),
                new Position2D(5, 2),
            };
            var specs = (p1, p2).Map(ps => new FullPlayerBoardSpec(DummyPlayer(), ps, size / 2));
            var board = GameBoard.Create(size, specs);
            var expectedp1 = new List<Position2D> {
                new Position2D(9, 9),
            };
            var expectedp2 = new List<Position2D> {
                new Position2D(0, 5),
                new Position2D(2, 9),
                new Position2D(5, 2),
            };
            TestCaseAssert(board, expectedp1, expectedp2);
        }


        //     0  1  2  3
        //  0           x
        //  1        x  O
        //  2     X  o  o
        //  3        x  o

        [Test]
        public void TestMoveBallScenario635746978() {
            const int size = 4;
            var p1 = new List<Position2D> {
                new Position2D(3, 2),
                new Position2D(0, 3),
                new Position2D(2, 1),
                new Position2D(1, 2),
            };
            var p2 = new List<Position2D> {
                new Position2D(3, 3),
                new Position2D(2, 3),
                new Position2D(1, 3),
                new Position2D(2, 2),
            };
            var specs = (p1, p2).Map(ps => new FullPlayerBoardSpec(DummyPlayer(), ps, size / 2));
            var board = GameBoard.Create(size, specs);
            var expectedp1 = new List<Position2D> {
                new Position2D(3, 2),
                new Position2D(1, 2),
            };
            var expectedp2 = new List<Position2D> {
                new Position2D(2, 3),
                new Position2D(2, 2),
            };
            TestCaseAssert(board, expectedp1, expectedp2);
        }


        //     0  1  2  3  4  5  6  7  8
        //  0                          x
        //  1              x     x      
        //  2        x        o     o   
        //  3                    x      
        //  4  o     O        x         
        //  5     x  o              X   
        //  6     o                     
        //  7        o  o               
        //  8  x           o            

        [Test]
        public void TestMoveBallScenario1270323850() {
            const int size = 9;
            var p1 = new List<Position2D> {
                new Position2D(8, 0),
                new Position2D(4, 5),
                new Position2D(1, 6),
                new Position2D(2, 2),
                new Position2D(5, 7),
                new Position2D(3, 6),
                new Position2D(0, 8),
                new Position2D(1, 4),
                new Position2D(5, 1),
            };
            var p2 = new List<Position2D> {
                new Position2D(6, 1),
                new Position2D(8, 4),
                new Position2D(7, 2),
                new Position2D(5, 2),
                new Position2D(4, 2),
                new Position2D(2, 5),
                new Position2D(7, 3),
                new Position2D(2, 7),
                new Position2D(4, 0),
            };
            var specs = (p1, p2).Map(ps => new FullPlayerBoardSpec(DummyPlayer(), ps, size / 2));
            var board = GameBoard.Create(size, specs);
            var expectedp1 = new List<Position2D> {
            };
            var expectedp2 = new List<Position2D> {
                new Position2D(5, 2),
                new Position2D(4, 0),
            };
            TestCaseAssert(board, expectedp1, expectedp2);
        }


        //     0  1  2  3  4  5  6  7  8  9 10 11 12 13 14 15 16
        //  0           x                          o  x         
        //  1                             x                     
        //  2           o  o  X  o                    o         
        //  3                                                   
        //  4                             x                     
        //  5                                o                  
        //  6        o                                   o      
        //  7              O  o     x                           
        //  8                 o                             o   
        //  9                       x                           
        // 10     o           o                                 
        // 11              o     x  x  x                        
        // 12                                            x  x   
        // 13                 x                                 
        // 14                       x                           
        // 15              x           x        x        o      
        // 16                                                  o

        [Test]
        public void TestMoveBallScenario1087037362() {
            const int size = 17;
            var p1 = new List<Position2D> {
                new Position2D(11, 7),
                new Position2D(12, 14),
                new Position2D(0, 13),
                new Position2D(1, 9),
                new Position2D(9, 7),
                new Position2D(11, 6),
                new Position2D(11, 8),
                new Position2D(14, 7),
                new Position2D(2, 5),
                new Position2D(4, 9),
                new Position2D(15, 8),
                new Position2D(12, 15),
                new Position2D(15, 11),
                new Position2D(15, 4),
                new Position2D(13, 5),
                new Position2D(7, 7),
                new Position2D(0, 3),
            };
            var p2 = new List<Position2D> {
                new Position2D(2, 4),
                new Position2D(11, 4),
                new Position2D(5, 10),
                new Position2D(8, 5),
                new Position2D(7, 5),
                new Position2D(10, 1),
                new Position2D(2, 3),
                new Position2D(16, 16),
                new Position2D(7, 4),
                new Position2D(6, 14),
                new Position2D(15, 14),
                new Position2D(2, 13),
                new Position2D(0, 12),
                new Position2D(6, 2),
                new Position2D(10, 5),
                new Position2D(2, 6),
                new Position2D(8, 15),
            };
            var specs = (p1, p2).Map(ps => new FullPlayerBoardSpec(DummyPlayer(), ps, size / 2));
            var board = GameBoard.Create(size, specs);
            var expectedp1 = new List<Position2D> {
                new Position2D(12, 15),
                new Position2D(0, 3),
            };
            var expectedp2 = new List<Position2D> {
                new Position2D(2, 4),
                new Position2D(11, 4),
                new Position2D(7, 5),
                new Position2D(8, 5),
                new Position2D(10, 1),
            };
            TestCaseAssert(board, expectedp1, expectedp2);
        }


        //     0  1  2  3  4  5  6  7  8  9 10 11 12 13
        //  0  o  x     x     X                        
        //  1                    o                    x
        //  2                       o                  
        //  3                 x  x                    x
        //  4     o              o           o         
        //  5                          x     o         
        //  6     o              x     x               
        //  7     x           o     o  O               
        //  8                                      x   
        //  9                    x                     
        // 10                             x            
        // 11        o                             o   
        // 12                                      o   
        // 13                                          

        [Test]
        public void TestMoveBallScenario632314519() {
            const int size = 14;
            var p1 = new List<Position2D> {
                new Position2D(5, 8),
                new Position2D(8, 12),
                new Position2D(1, 13),
                new Position2D(0, 3),
                new Position2D(10, 9),
                new Position2D(7, 1),
                new Position2D(3, 13),
                new Position2D(0, 5),
                new Position2D(0, 1),
                new Position2D(3, 6),
                new Position2D(6, 6),
                new Position2D(9, 6),
                new Position2D(6, 8),
                new Position2D(3, 5),
            };
            var p2 = new List<Position2D> {
                new Position2D(7, 5),
                new Position2D(11, 2),
                new Position2D(11, 12),
                new Position2D(6, 1),
                new Position2D(4, 6),
                new Position2D(2, 7),
                new Position2D(5, 10),
                new Position2D(7, 8),
                new Position2D(7, 7),
                new Position2D(0, 0),
                new Position2D(4, 10),
                new Position2D(1, 6),
                new Position2D(4, 1),
                new Position2D(12, 12),
            };
            var specs = (p1, p2).Map(ps => new FullPlayerBoardSpec(DummyPlayer(), ps, size / 2));
            var board = GameBoard.Create(size, specs);
            var expectedp1 = new List<Position2D> {
                new Position2D(3, 5),
                new Position2D(0, 3),
            };
            var expectedp2 = new List<Position2D> {
                new Position2D(7, 7),
                new Position2D(11, 12),
                new Position2D(5, 10),
            };
            TestCaseAssert(board, expectedp1, expectedp2);
        }

        #endregion


        // Generates the code for a complete test case using 
        // the current implementation of BoardAnalysis.MovesForBall
        private static string GenerateTestCase(int size, (IList<Position2D>, IList<Position2D>) specs) {
            var sb = new StringBuilder();
            sb.AppendLine("[Test]")
              .Append("public void TestMoveBallScenario").Append(MockUtil.Rng.Next()).AppendLine("(){")
              .Append("const int size = ").Append(size).AppendLine(";")
              .AppendLine("var p1 = new List<Position2D> {");

            foreach (var p in specs.Item1) {
                sb.AppendLine("    new Position2D(" + p.X + ", " + p.Y + "),");
            }
            sb.AppendLine("};").AppendLine("var p2 = new List<Position2D> {");
            foreach (var p in specs.Item2) {
                sb.AppendLine("    new Position2D(" + p.X + ", " + p.Y + "),");
            }

            var board = GameBoard.Create(size, specs.Map(ps => new FullPlayerBoardSpec(DummyPlayer(), ps, size / 2)));

            sb.AppendLine("};")
              .AppendLine("var specs = (p1, p2).Map(ps => new FullPlayerBoardSpec(DummyPlayer(), ps, size / 2));")
              .AppendLine("var board = GameBoard.Create(size, specs);")
              .AppendLine("var expectedp1 = new List<Position2D> {");
            foreach (var p in board.MovesForBall(board.BallCarrier1)) {
                sb.AppendLine("    new Position2D(" + p.X + ", " + p.Y + "),");
            }
            sb.AppendLine("};")
              .AppendLine("var expectedp2 = new List<Position2D> {");
            foreach (var p in board.MovesForBall(board.BallCarrier2)) {
                sb.AppendLine("    new Position2D(" + p.X + ", " + p.Y + "),");
            }
            sb.AppendLine("};")
              .AppendLine("TestCaseAssert(board, expectedp1, expectedp2);")
              .AppendLine("}");

            return sb.ToString();
        }
    }
}