using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using Diaballik.Core.Util;


namespace Diaballik.Core {
    using FullPlayerSpecPair = ValueTuple<FullPlayerBoardSpec, FullPlayerBoardSpec>;

    /// <summary>
    ///     Represents the game board and manages the moving of pieces.
    ///     By convention, if n is the value of BoardSize, the four corners
    ///     of the board have the positions:
    ///     <pre>
    ///         (0, 0) --- (0, n-1)
    ///         |            |
    ///         |            |
    ///         (n-1, 0) --- (n-1, n-1)
    ///     </pre>
    ///     By convention, the first player owns the row n-1 (the bottom
    ///     one), and the second player owns the row 0 (the top one).
    ///     This class is immutable.
    /// </summary>
    public sealed class GameBoard : BoardLike {
        #region Fields

        private readonly ImmutableDictionary<Position2D, Player> _boardLookup;
        private readonly ImmutableHashSet<Position2D> _player1Positions;
        private readonly ImmutableHashSet<Position2D> _player2Positions;

        #endregion

        #region BoardLike properties

        public override Player Player1 { get; }
        public override Player Player2 { get; }

        public override Position2D BallCarrier1 { get; }
        public override Position2D BallCarrier2 { get; }

        public override IEnumerable<Position2D> Player1Positions => _player1Positions;
        public override IEnumerable<Position2D> Player2Positions => _player2Positions;

        public override int BoardSize { get; }

        #endregion

        #region Constructors

        // Performs full consistency checks, only the first time
        private GameBoard(int boardSize, FullPlayerSpecPair specs) {
            BoardSize = boardSize;
            (Player1, Player2) = specs.Map(x => x.Player);

            var positions = specs.Map(x => x.Positions.ToList());

#if DEBUG
            CheckPieces(boardSize, positions);
#endif

            (BallCarrier1, BallCarrier2) = positions.Zip(specs, (l, spec) => l[spec.BallIndex]);
            (_player1Positions, _player2Positions) = positions.Map(ImmutableHashSet.CreateRange);

            var lookupBuilder = ImmutableDictionary.CreateBuilder<Position2D, Player>();
            specs.Map(spec => spec.Positions.Select(p => new KeyValuePair<Position2D, Player>(p, spec.Player)))
                 .Foreach(lookupBuilder.AddRange);
            _boardLookup = lookupBuilder.ToImmutable();
        }


        // used internally, for MovePiece updates
        private GameBoard(GameBoard previous, ImmutableDictionary<Position2D, Player> lookup,
            (ImmutableHashSet<Position2D>, ImmutableHashSet<Position2D>) positions) {
            BoardSize = previous.BoardSize;
            Player1 = previous.Player1;
            Player2 = previous.Player2;
            _boardLookup = lookup;
            (BallCarrier1, BallCarrier2) = previous.BallCarrierPair;
            (_player1Positions, _player2Positions) = positions;
        }


        // used internally, for MoveBall updates
        private GameBoard(GameBoard previous, (Position2D, Position2D) ballCarriers) {
            BoardSize = previous.BoardSize;
            Player1 = previous.Player1;
            Player2 = previous.Player2;
            _boardLookup = previous._boardLookup;
            (BallCarrier1, BallCarrier2) = ballCarriers;
            _player1Positions = previous._player1Positions;
            _player2Positions = previous._player2Positions;
        }

        #endregion

        #region Factory methods

        /// <summary>
        ///     Creates a new gameBoard.
        /// </summary>
        /// <param name="size">Size of the board</param>
        /// <param name="p1Spec">Spec of player 2</param>
        /// <param name="p2Spec">Spec of player 2</param>
        /// <returns>A new gameboard</returns>
        /// <exception cref="ArgumentException">
        ///     If the players have an incorrect number of pieces,
        ///     or there are duplicate pieces
        /// </exception>
        public static GameBoard Create(int size, FullPlayerBoardSpec p1Spec, FullPlayerBoardSpec p2Spec) {
            return new GameBoard(size, (p1Spec, p2Spec));
        }


        /// <summary>
        ///     Creates a new gameBoard.
        /// </summary>
        /// <param name="size">Size of the board</param>
        /// <param name="specs">Pair of the specs of each players</param>
        /// <returns>A new gameboard</returns>
        /// <exception cref="ArgumentException">
        ///     If the players have an incorrect number of pieces,
        ///     or there are duplicate pieces
        /// </exception>
        public static GameBoard Create(int size, FullPlayerSpecPair specs) {
            return new GameBoard(size, specs);
        }

        #endregion

        #region BoardLike methods

        public override bool IsFree(Position2D pos) {
            return !_boardLookup.ContainsKey(pos);
        }

        public override Player PlayerOn(Position2D pos) {
            var ok = _boardLookup.TryGetValue(pos, out var player);
            return ok ? player : null;
        }

        #endregion

        #region Update methods

        /// <summary>
        ///     Moves a piece to a new location. This method cannot move the piece which carries the ball.
        /// </summary>
        /// <param name="src">Piece to move</param>
        /// <param name="dst">New position</param>
        /// <returns>The updated gameboard</returns>
        /// <exception cref="ArgumentException">If the piece or destination position is invalid</exception>
        public GameBoard MovePiece(Position2D src, Position2D dst) {
#if DEBUG
            DiaballikUtil.Assert(!HasBall(src), "Illegal: cannot move the piece which carries the ball");

            CheckPositionIsValid(src);
            CheckPositionIsValid(dst);

            DiaballikUtil.Assert(!IsFree(src), "Illegal: no piece to move");
            DiaballikUtil.Assert(IsFree(dst), "Illegal: destination is not free");
#endif

            var player = PlayerOn(src);
            var lookup = _boardLookup.ToBuilder();
            lookup.Remove(src);
            lookup.Add(dst, player);

            var positions = ((ImmutableHashSet<Position2D>) PositionsForPlayer(player)).ToBuilder();
            positions.Remove(src);
            positions.Add(dst);

            return player == Player1
                ? new GameBoard(this, lookup.ToImmutable(), (positions.ToImmutable(), _player2Positions))
                : new GameBoard(this, lookup.ToImmutable(), (_player1Positions, positions.ToImmutable()));
        }


        /// <summary>
        ///     Moves the ball from one piece to another friendly piece.
        /// </summary>
        /// <param name="src">Piece to move</param>
        /// <param name="dst">New position</param>
        /// <returns>The updated game</returns>
        /// <exception cref="ArgumentException">If the piece or destination position is invalid</exception>
        public GameBoard MoveBall(Position2D src, Position2D dst) {
#if DEBUG
            CheckPositionIsValid(src);
            DiaballikUtil.Assert(!IsFree(src) && HasBall(src),
                                 "Illegal: no ball to move on position " + src);
            CheckPositionIsValid(dst);
            DiaballikUtil.Assert(!IsFree(dst) && PlayerOn(dst) == PlayerOn(src),
                                 "Illegal: no friendly piece on position " + dst);
#endif


            return PlayerOn(src) == Player1
                ? new GameBoard(this, (dst, BallCarrier2))
                : new GameBoard(this, (BallCarrier1, dst));
        }

        #endregion

        #region Debug assertions

#if DEBUG
        private static void CheckPieces(int size, (List<Position2D>, List<Position2D>) positions) {
            DiaballikUtil.Assert(positions.Map(l => l.Count).Forall(i => i == size),
                                 "One or more players have an incorrect number of pieces");

            var (p1List, p2List) = positions;

            DiaballikUtil.Assert(p1List.Distinct().Count() == p1List.Count
                                 && p2List.Distinct().Count() == p2List.Count
                                 && !p1List.Intersect(p2List).Any(),
                                 "One or more players have duplicate pieces");
        }


        private void CheckPositionIsValid(Position2D p) {
            if (!IsOnBoard(p)) {
                throw new ArgumentException("Illegal: position is out of the board " + p);
            }
        }
#endif

        #endregion

        #region Inherited System.Object members

        public override string ToString() {
            var sb = new StringBuilder();
            sb.Append("   ");

            void AppendItem(object o) {
                sb.AppendFormat("{0,3}", o);
            }

            foreach (var i in Enumerable.Range(0, BoardSize)) {
                AppendItem(i);
            }
            sb.AppendLine();


            for (var x = 0; x < BoardSize; x++) {
                AppendItem(x);
                for (var y = 0; y < BoardSize; y++) {
                    var pos = new Position2D(x, y);
                    if (pos == BallCarrier1) {
                        AppendItem('X');
                        continue;
                    }
                    if (pos == BallCarrier2) {
                        AppendItem('O');
                        continue;
                    }
                    var p = PlayerOn(pos);
                    if (p == Player1) {
                        AppendItem('x');
                    } else if (p == Player2) {
                        AppendItem('o');
                    } else {
                        AppendItem(' ');
                    }
                }
                sb.AppendLine();
            }
            return sb.ToString();
        }


        public bool Equals(GameBoard other) {
            return _player1Positions.SetEquals(other._player1Positions)
                   && _player2Positions.SetEquals(other._player2Positions)
                   && Player1.Equals(other.Player1)
                   && Player2.Equals(other.Player2)
                   && BallCarrier1.Equals(other.BallCarrier1)
                   && BallCarrier2.Equals(other.BallCarrier2)
                   && BoardSize == other.BoardSize;
        }

        public override bool Equals(object obj) {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((GameBoard) obj);
        }

        public override int GetHashCode() {
            unchecked {
                var hashCode = _player1Positions.GetHashCode();
                hashCode = (hashCode * 397) ^ _player2Positions.GetHashCode();
                hashCode = (hashCode * 397) ^ Player1.GetHashCode();
                hashCode = (hashCode * 397) ^ Player2.GetHashCode();
                hashCode = (hashCode * 397) ^ BallCarrier1.GetHashCode();
                hashCode = (hashCode * 397) ^ BallCarrier2.GetHashCode();
                hashCode = (hashCode * 397) ^ BoardSize;
                return hashCode;
            }
        }

        public static bool operator ==(GameBoard left, GameBoard right) {
            return Equals(left, right);
        }

        public static bool operator !=(GameBoard left, GameBoard right) {
            return !Equals(left, right);
        }

        #endregion
    }
}