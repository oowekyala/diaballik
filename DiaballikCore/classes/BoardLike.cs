using System;
using System.Collections.Generic;
using System.Linq;
using Diaballik.Core.Util;

namespace Diaballik.Core {
    /// <summary>
    ///     Object that has the characteristics of a board. Enriches the interface of GameBoard
    ///     using few abstract methods.
    /// </summary>
    public abstract class BoardLike {
        public abstract int BoardSize { get; }

        public abstract IPlayer Player1 { get; }
        public abstract IPlayer Player2 { get; }

        public abstract IEnumerable<Position2D> Player1Positions { get; }
        public abstract IEnumerable<Position2D> Player2Positions { get; }

        public abstract Position2D BallCarrier1 { get; }
        public abstract Position2D BallCarrier2 { get; }

        /// <summary>
        ///     Returns true if the position has no piece on it.
        ///     Doesn't check for validity of the position.
        /// </summary>
        public abstract bool IsFree(Position2D p);

        /// <summary>
        ///     Returns the player at the specified position.
        ///     Returns null if the tile is empty or out of bounds.
        /// </summary>
        /// <returns>The player, or null</returns>
        public abstract IPlayer PlayerOn(Position2D p);

        /// <summary>
        ///     Returns true if the position is on the board.
        /// </summary>
        public bool IsOnBoard(Position2D p) {
            return p.X >= 0 && p.X < BoardSize
                   && p.Y >= 0 && p.Y < BoardSize;
        }

        /// <summary>
        ///     An ordered pair of the positions of the pieces of each player.
        /// </summary>
        public (IEnumerable<Position2D>, IEnumerable<Position2D>) PositionsPair => (Player1Positions, Player2Positions);

        /// <summary>
        ///     An ordered pair of the positions of the pieces which carry the 
        ///     ball for each player.
        /// </summary>
        public (Position2D, Position2D) BallCarrierPair => (BallCarrier1, BallCarrier2);


        /// <summary>
        ///     Returns the opponent of the specified player.
        /// </summary>
        /// <exception cref="ArgumentException">If the player is unknown</exception>
        public IPlayer GetOtherPlayer(IPlayer player) {
            return player == Player1
                ? Player2
                : player == Player2
                    ? Player1
                    : throw new ArgumentException("Unknown player");
        }


        /// <summary>
        ///     Returns true if the position is that of one of the
        ///     ball carriers.
        /// </summary>
        public bool HasBall(Position2D p) {
            return p == BallCarrier1 || p == BallCarrier2;
        }


        /// <summary>
        ///     Gets the positions of the pieces of a player.
        /// </summary>
        /// <exception cref="ArgumentException">If the player is unknown</exception>
        public IEnumerable<Position2D> PositionsForPlayer(IPlayer player) {
            return player == Player1
                ? Player1Positions
                : player == Player2
                    ? Player2Positions
                    : throw new ArgumentException("Unknown player");
        }


        /// <summary>
        ///     Returns true if the given player is victorious in the current
        ///     configuration.
        /// </summary>
        public bool IsVictoriousPlayer(IPlayer player) {
            return PositionsForPlayer(player)
                .Select(p => p.X)
                .Any(i => BoardSize - 1 - GetRowIndexOfInitialLine(player) == i);
        }


        // Gets the index of the starting row of a player. Used to check for victory
        protected int GetRowIndexOfInitialLine(IPlayer player) {
            return player == Player1
                ? BoardSize - 1
                : player == Player2
                    ? 0
                    : throw new ArgumentException("Unknown player");
        }

        // TODO Move this into the C++ lib
        /// <summary>
        ///     Returns true if there is a piece-free vertical, horizontal, or diagonal line
        ///     between the two positions on the Board.
        /// </summary>
        /// <returns>True if there is a free line between the specified positions</returns>
        /// <exception cref="ArgumentException">If the positions are identical</exception>
        public bool IsLineFreeBetween(Position2D p1, Position2D p2) {
            var dX = p2.X - p1.X;
            var dY = p2.Y - p1.Y;
            var deltaX = Math.Abs(dX);
            var deltaY = Math.Abs(dY);

            DiaballikUtil.Assert(deltaX != 0 || deltaY != 0, "Illegal: cannot move to the same piece");


            if (deltaX <= 1 && deltaY <= 1) {
                return true; // pieces are side by side
            }

            // neither straight line nor diagonal
            if (deltaX != 0 && deltaY != 0 && deltaX != deltaY) {
                return false;
            }

            // compute the range only if count > 0. Otherwise we won't use the value anyway
            var ys = deltaY > 0 ? Enumerable.Range(Math.Min(p1.Y, p2.Y) + 1, deltaY - 1) : null;
            var xs = deltaX > 0 ? Enumerable.Range(Math.Min(p1.X, p2.X) + 1, deltaX - 1) : null;

            if (deltaX == 0) {
                // same row
                return ys.Select(y => new Position2D(p1.X, y)).All(IsFree);
            }

            if (deltaY == 0) {
                // same column
                return xs.Select(x => new Position2D(x, p1.Y)).All(IsFree);
            }

            // we have to reverse one list in two quadrants, otherwise the zipped positions cross the diagonal
            return deltaX == deltaY && xs.Zip(dX * dY > 0 ? ys : ys.Reverse(), Position2D.New).All(IsFree); // diagonal
        }


        /// <summary>
        ///     Gets the position of the piece carrying the ball of a player.
        /// </summary>
        /// <exception cref="ArgumentException">If the player is not recognised</exception>
        public Position2D BallCarrierForPlayer(IPlayer player) {
            return player == Player1
                ? BallCarrier1
                : player == Player2
                    ? BallCarrier2
                    : throw new ArgumentException("Unknown player");
        }
    }

    /// <summary>
    ///     Abstract class for board like decorators. It's a means of sharing functionality between
    ///     GameState and GameBoard without breaking encapsulation.
    /// </summary>
    /// <typeparam name="T">Concrete type of board this decorator decorates</typeparam>
    public abstract class BoardLikeDecorator<T> : BoardLike where T : BoardLike {
        protected readonly T UnderlyingBoard;

        public override IEnumerable<Position2D> Player1Positions => UnderlyingBoard.Player1Positions;
        public override IEnumerable<Position2D> Player2Positions => UnderlyingBoard.Player2Positions;
        public override int BoardSize => UnderlyingBoard.BoardSize;
        public override IPlayer Player1 => UnderlyingBoard.Player1;
        public override IPlayer Player2 => UnderlyingBoard.Player2;
        public override Position2D BallCarrier1 => UnderlyingBoard.BallCarrier1;
        public override Position2D BallCarrier2 => UnderlyingBoard.BallCarrier2;
        public override bool IsFree(Position2D p) => UnderlyingBoard.IsFree(p);
        public override IPlayer PlayerOn(Position2D p) => UnderlyingBoard.PlayerOn(p);

        protected BoardLikeDecorator(T underlyingBoard) {
            UnderlyingBoard = underlyingBoard;
        }
    }
}