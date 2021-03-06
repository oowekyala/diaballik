﻿using System;
using System.Linq;
using System.Windows.Media;
using Diaballik.Core.Builders;
using Diaballik.Core.Util;
using NUnit.Framework;
using static Diaballik.Core.Builders.GameBuilder;

namespace Diaballik.Tests {
    [TestFixture]
    public class GameBuilderTests {
        [Test]
        public void TestSetSize([Range(1, 15)] int size) {
            var builder = new GameBuilder();
            builder.PlayerBuilder1.Color = Colors.Black;
            builder.PlayerBuilder2.Color = Colors.White;

            if (size % 2 == 0 || size < 2) {
                Assert.That(() => {
                    builder.BoardSize = size;
                    builder.Build();
                }, Throws.ArgumentException);
            } else {
                builder.BoardSize = size;
                var game = builder.Build();
                Assert.AreEqual(size, game.State.BoardSize);
            }
        }

        [Test]
        public void TestSameColor() {
            var builder = new GameBuilder {
                BoardSize = 7,
                Scenario = GameScenario.EnemyAmongUs
            };

            builder.PlayerBuilder1.Color = Colors.Blue;
            builder.PlayerBuilder1.Name = "foo";
            builder.PlayerBuilder2.Color = Colors.Blue;
            builder.PlayerBuilder2.Name = "bar";

            Assert.IsTrue(builder.CannotBuild);
            Assert.IsFalse(string.IsNullOrWhiteSpace(builder.ErrorMessage));
            Console.WriteLine(builder.ErrorMessage);
        }


        [Test]
        public void TestStandardInitStrategy([Range(3, 15, 2)] int size) {
            var builder = new GameBuilder {
                BoardSize = size,
                Scenario = GameScenario.Standard
            };
            builder.PlayerBuilder1.Color = Colors.Black;
            builder.PlayerBuilder2.Color = Colors.White;

            var game = builder.Build();
            var (p1Pos, p2Pos) = game.State.PositionsPair.Map(x => x.ToList());

            Assert.AreEqual(size, p1Pos.Count);
            Assert.AreEqual(size, p2Pos.Count);

            p1Pos.ForEach(x => Assert.AreEqual(size - 1, x.X)); // bottom row
            p2Pos.ForEach(x => Assert.AreEqual(0, x.X)); // top row

            // ball carriers are middle pieces
            game.State.BallCarrierPair.ForEach(x => Assert.AreEqual(size / 2, x.Y));
        }


        [Test]
        public void TestBallRandomInitStrategy([Range(3, 15, 2)] int size) {
            var builder = new GameBuilder {
                BoardSize = size,
                Scenario = GameScenario.BallRandom
            };
            builder.PlayerBuilder1.Color = Colors.Black;
            builder.PlayerBuilder2.Color = Colors.White;

            var game = builder.Build();
            var (p1Pos, p2Pos) = game.State.PositionsPair.Map(x => x.ToList());

            game.State.PositionsPair.ForEach(ps => Assert.AreEqual(size, ps.Count()));

            p1Pos.ForEach(x => Assert.AreEqual(size - 1, x.X)); // bottom row
            p2Pos.ForEach(x => Assert.AreEqual(0, x.X)); // top row

            // ball carriers can be any piece in the line
        }


        [Test]
        public void TestEnemyAmongUsInitStrategy([Range(3, 15, 2)] int size) {
            var builder = new GameBuilder {
                BoardSize = size,
                Scenario = GameScenario.EnemyAmongUs
            };
            builder.PlayerBuilder1.Color = Colors.Black;
            builder.PlayerBuilder2.Color = Colors.White;

            var game = builder.Build();
            var positionsTuple = game.State.PositionsPair.Map(x => x.ToList());

            positionsTuple.ForEach(ps => Assert.AreEqual(size, ps.Count));

            positionsTuple.Zip((size - 1, 0))
                          .ForEach(t => {
                              var (ps, row) = t;
                              Assert.AreEqual(size - 2, ps.Count(p => p.X == row)); // friend row
                              Assert.AreEqual(2, ps.Count(p => p.X == size - 1 - row)); // enemy row
                          });

            // ball carriers are middle pieces, in friend row
            game.State.BallCarrierPair
                .Zip((size - 1, 0))
                .ForEach(t => {
                    var (ball, row) = t;
                    Assert.AreEqual(size / 2, ball.Y);
                    Assert.AreEqual(row, ball.X);
                });
        }
    }
}