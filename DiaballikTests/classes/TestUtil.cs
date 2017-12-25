using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.ExceptionServices;
using Diaballik.AlgoLib;
using Diaballik.Core;
using Diaballik.Core.Util;
using Diaballik.Players;

namespace Diaballik.Tests {
    /// <summary>
    ///     Utility methods to mock objects.
    /// </summary>
    public static class TestUtil {
        public static readonly Random Rng = new Random();


        public static IEnumerable<Position2D> RandomPositions(int n, int boardSize) {
            var pos = OrderedPositionsPool(boardSize).ToList();
            if (pos.Count < n) {
                throw new InvalidOperationException("Not enough positions on the board "
                                                    + "(" + pos.Count + ", need " + n + ")");
            }

            return pos.Shuffle().Take(n);
        }


        // Implicitly uses the board size as the number of pieces
        public static IEnumerable<Position2D> RandomPositions(int n) => RandomPositions(n, n);


        internal static IEnumerable<Position2D> OrderedPositionsPool(int boardSize)
            => from col in Enumerable.Range(0, boardSize)
                from row in Enumerable.Range(0, boardSize)
                select new Position2D(col, row);


        public static (IEnumerable<Position2D>, IEnumerable<Position2D>) RandomPositionsPair(int n, int boardSize) {
            var pool = RandomPositions(2 * n, boardSize).ToList();
            return (pool.GetRange(0, n), pool.GetRange(n, n));
        }


        public static (IEnumerable<Position2D>, IEnumerable<Position2D>) RandomPositionsPair(int n)
            => RandomPositionsPair(n, n);


        private static IEnumerable<T> Shuffle<T>(this IList<T> list) {
            for (var i = 0; i < list.Count; i++) {
                var j = Rng.Next(i, list.Count);
                yield return list[j];

                list[j] = list[i];
            }
        }


        public static Color RandomColor() => Color.FromArgb(Rng.Next(256), Rng.Next(256), Rng.Next(256));

        public static (FullPlayerBoardSpec, FullPlayerBoardSpec) DummyPlayerSpecPair(int boardSize)
            => RandomPositionsPair(boardSize).Map(p => new FullPlayerBoardSpec(DummyPlayer(), p, boardSize / 2));


        public static FullPlayerBoardSpec DummyPlayerSpec(IEnumerable<Position2D> positions, int ballIndex)
            => new FullPlayerBoardSpec(DummyPlayer(), positions, ballIndex);


        public static FullPlayerBoardSpec DummyPlayerSpec(int boardSize, IEnumerable<Position2D> positions)
            => new FullPlayerBoardSpec(DummyPlayer(), positions, boardSize / 2);


        public static IPlayer DummyPlayer() => DummyPlayer(RandomColor(), "dummy" + Rng.Next(100));


        /// Returns a dummy player of any type
        public static IPlayer DummyPlayer(Color c, string name) {
            var random = Rng.Next(100);

            if (random < 33) {
                return new NoobAiPlayer(c, name);
            }
            if (random < 66) {
                return new StartingAiPlayer(c, name);
            }
            return new ProgressiveAiPlayer(c, name);
        }


        /// Tries to get a valid MovePiece.
        private static UpdateAction TryGetAMovePiece(GameState state, IEnumerable<Position2D> ps,
            Position2D ballCarrier) {
            foreach (var p in ps.Where(p => p != ballCarrier)) {
                var movePieces = state.AvailableMoves(p).ToList();
                if (movePieces.Any()) return movePieces.First();
            }
            // We make the assumption that this line is never reached,
            // which is confirmed by coverage analysis.
            return new PassAction();
        }

        /// Tries to get a valid MoveBall, falling back on a MovePiece if none is available
        private static UpdateAction TryGetAMoveBall(GameState state, IEnumerable<Position2D> ps,
            Position2D ballCarrier) {
            var moveBalls = state.AvailableMoves(ballCarrier).ToList();
            return moveBalls.Any() ? moveBalls.First() : TryGetAMovePiece(state, ps, ballCarrier);
        }

        /// <summary>
        ///     Gets a valid move to perform on the state.
        /// 
        ///     Tries to return a proportion of 40% MovePiece, 
        ///     40% MoveBall, and 20% Pass. In reality, the 
        ///     proportion is about 50% MovePiece, 30% MoveBall, 
        ///     and 20% Pass, because the returned move must be valid.
        /// </summary>
        public static UpdateAction GetAMove(GameState state) {
            var ps = state.PositionsForPlayer(state.CurrentPlayer);
            var ballCarrier = state.BallCarrierForPlayer(state.CurrentPlayer);

            const int movePieceProportion = 40; // 40%
            const int moveBallProportion = 40; // 40%
            var random = Rng.Next(100);

            // type of the action to return
            var expectedType = random < movePieceProportion
                ? typeof(MovePieceAction)
                : random < movePieceProportion + moveBallProportion
                    ? typeof(MoveBallAction)
                    : typeof(PassAction);

            if (random < movePieceProportion) {
                return TryGetAMovePiece(state, ps, ballCarrier).UpdateStatisticsAndPass();
            }
            if (random < movePieceProportion + moveBallProportion) {
                return TryGetAMoveBall(state, ps, ballCarrier).UpdateStatisticsAndPass();
            }
            var pass = new PassAction();
            return (pass.IsValidOn(state) ? pass : TryGetAMovePiece(state, ps, ballCarrier)).UpdateStatisticsAndPass();
        }

        public static int MoveBalls;
        public static int MovePieces;
        public static int Passes;


        /// Updates the creation statistics of moves
        private static UpdateAction UpdateStatisticsAndPass(this UpdateAction action) {
            switch (action) {
                case MoveBallAction moveBallAction:
                    MoveBalls++;
                    break;
                case MovePieceAction movePieceAction:
                    MovePieces++;
                    break;
                case PassAction passAction:
                    Passes++;
                    break;
            }
            return action;
        }

        public static void PrintStats() {
            Console.WriteLine($"MovePieces: {MovePieces}");
            Console.WriteLine($"MoveBalls: {MoveBalls}");
            Console.WriteLine($"Passes: {Passes}");
        }

        /// Gets a state with the given number of moves left.
        /// Effectively gets a random state.
        public static GameState AnyState(int size, int movesLeftCount) {
            var specs = DummyPlayerSpecPair(size);
            var cur = GameState.InitialState(size, specs, true);

            while (cur.NumMovesLeft != movesLeftCount) {
                cur = GetAMove(cur).UpdateState(cur);
            }

            return cur;
        }

        public static GameState AnyState(int size) {
            return AnyState(size, Rng.Next(1, Game.MaxMovesPerTurn + 1));
        }

        public static Game AnyGame(int size) {
            return AnyGame(size, Rng.Next(0, 20));
        }


        /// Constructs a game from a random initial state and updating it with valid moves
        /// Moves don't contain Undo actions
        public static Game AnyGame(int size, int historySize) {
            var specs = DummyPlayerSpecPair(size);
            var game = Game.Init(size, specs);

            while (historySize-- > 0) {
                game.Update(GetAMove(game.State));
            }

            return game;
        }

        /// Generates an infinite sequence from a generator function.
        public static IEnumerable<T> Generate<T>(Func<T> f) {
            while (true) yield return f();
        }
    }
}