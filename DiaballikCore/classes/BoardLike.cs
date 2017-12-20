using System;
using System.Collections.Generic;
using System.Linq;

namespace Diaballik.Core {
    /// <summary>
    ///     Object that has the characteristics of a board. Enriches the interface of GameBoard
    ///     using few abstract methods.
    /// </summary>
    public abstract class BoardLike {
        public abstract IEnumerable<Position2D> Player1Positions { get; }
        public abstract IEnumerable<Position2D> Player2Positions { get; }
        public abstract int BoardSize { get; }

        public abstract IPlayer Player1 { get; }
        public abstract IPlayer Player2 { get; }


        public abstract Position2D BallBearer1 { get; }
        public abstract Position2D BallBearer2 { get; }

        /// <summary>
        ///     Returns true if the position has no piece on it.
        ///     Doesn't check for validity of the position.
        /// </summary>
        /// <param name="p">Position to check</param>
        /// <returns>True if the position is free</returns>
        public abstract bool IsFree(Position2D p);

        /// <summary>
        ///     Returns the player at the specified position.
        ///     Returns null if the tile is empty or out of bounds.
        /// </summary>
        /// <param name="p">The position to test</param>
        /// <returns>The player, or null</returns>
        public abstract IPlayer PlayerOn(Position2D p);


        public bool IsOnBoard(Position2D p) {
            return p.X >= 0 && p.X < BoardSize
                   && p.Y >= 0 && p.Y < BoardSize;
        }


        public (IEnumerable<Position2D>, IEnumerable<Position2D>) PositionsPair => (Player1Positions, Player2Positions);


        public (Position2D, Position2D) BallBearerPair => (BallBearer1, BallBearer2);


        public IPlayer GetOtherPlayer(IPlayer player) {
            return player == Player1
                ? Player2
                : player == Player2
                    ? Player1
                    : throw new ArgumentException("Unknown player");
        }


        public bool HasBall(Position2D p) {
            return p == BallBearer1 || p == BallBearer2;
        }


        /// <summary>
        ///     Gets the positions of the pieces of a player.
        /// </summary>
        /// <param name="player">The player</param>
        /// <returns>The positions</returns>
        /// <exception cref="ArgumentException">If the player is not recognised</exception>
        public IEnumerable<Position2D> PositionsForPlayer(IPlayer player) {
            return player == Player1
                ? Player1Positions
                : player == Player2
                    ? Player2Positions
                    : throw new ArgumentException("Unknown player");
        }


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


        /// <summary>
        ///     Gets the position of the piece bearing the ball of a player.
        /// </summary>
        /// <param name="player">The player</param>
        /// <returns>The position</returns>
        /// <exception cref="ArgumentException">If the player is not recognised</exception>
        public Position2D BallBearerForPlayer(IPlayer player) {
            return player == Player1
                ? BallBearer1
                : player == Player2
                    ? BallBearer2
                    : throw new ArgumentException("Unknown player");
        }
    }

    /// <summary>
    ///     Abstract class for board like decorators. It's a means of sharing functionality between
    ///     GameState, Game and GameBoard without breaking encapsulation.
    /// </summary>
    /// <typeparam name="T">Concrete type of board this decorator decorates</typeparam>
    public abstract class BoardLikeDecorator<T> : BoardLike where T : BoardLike {
        protected T UnderlyingBoard;

        public override IEnumerable<Position2D> Player1Positions => UnderlyingBoard.Player1Positions;
        public override IEnumerable<Position2D> Player2Positions => UnderlyingBoard.Player2Positions;
        public override int BoardSize => UnderlyingBoard.BoardSize;
        public override IPlayer Player1 => UnderlyingBoard.Player1;
        public override IPlayer Player2 => UnderlyingBoard.Player2;
        public override Position2D BallBearer1 => UnderlyingBoard.BallBearer1;
        public override Position2D BallBearer2 => UnderlyingBoard.BallBearer2;
        public override bool IsFree(Position2D p) => UnderlyingBoard.IsFree(p);
        public override IPlayer PlayerOn(Position2D p) => UnderlyingBoard.PlayerOn(p);

        protected BoardLikeDecorator(T underlyingBoard) {
            UnderlyingBoard = underlyingBoard;
        }
    }
}