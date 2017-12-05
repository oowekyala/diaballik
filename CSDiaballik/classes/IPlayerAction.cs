using System;

namespace CSDiaballik {
    /// <summary>
    ///     Represents an action the player can carry out during their turn.
    /// </summary>
    public interface IPlayerAction {

        /// <summary>
        ///     Returns true if the move is valid, in the context of specified actor
        ///     player and the board.
        /// </summary>
        /// <param name="actor">The player making the move</param>
        /// <param name="board">The current state of the board</param>
        /// <param name="movesLeft">The number of moves left to the player for this turn</param>
        /// <returns>True if the move is valid</returns>
        bool IsMoveValid(IPlayer actor, GameBoard board, int movesLeft);

    }


    /// <summary>
    ///     Action that can update the game's state. Undo is excluded from this
    ///     category, as it's implemented by state switch in Game.
    /// </summary>
    public interface IUpdateAction : IPlayerAction {

        /// <summary>
        ///     Updates the given current state with this action.
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        GameState UpdateState(GameState state);

    }


    /// <inheritdoc />
    /// <summary>
    ///     Base class for MovePiece and MoveBall
    /// </summary>
    public abstract class MoveAction : IUpdateAction {

        public Position2D Src { get; }
        public Position2D Dst { get; }


        protected MoveAction(Position2D src, Position2D dst) {
            Src = src;
            Dst = dst;
        }


        // partial implementation
        public virtual bool IsMoveValid(IPlayer actor, GameBoard board, int movesLeft)
            => Src != Dst ? true : throw new ArgumentException("Illegal: cannot move to the same piece");


        public abstract GameState UpdateState(GameState state);

    }


    /// <summary>
    ///     Move the ball to another piece.
    /// </summary>
    public class MoveBall : MoveAction {

        public MoveBall(Position2D src, Position2D dst) : base(src, dst) {
        }


        public override bool IsMoveValid(IPlayer actor, GameBoard board, int movesLeft) {
            return base.IsMoveValid(actor, board, movesLeft)
                   && movesLeft > 0
                   && board.PlayerOn(Src) == board.PlayerOn(Dst)
                   && board.PlayerOn(Src) == actor
                   && board.IsLineFreeBetween(Src, Dst);
        }


        public override GameState UpdateState(GameState state) {
            return state.MoveBall(Src, Dst);
        }

    }


    /// <summary>
    ///     Move a piece to a new location.
    /// </summary>
    public class MovePiece : MoveAction {

        public MovePiece(Position2D src, Position2D dst) : base(src, dst) {
        }


        public override bool IsMoveValid(IPlayer actor, GameBoard board, int movesLeft) {
            return base.IsMoveValid(actor, board, movesLeft)
                   && movesLeft > 0
                   && Math.Abs(Src.X - Dst.X) <= 1
                   && Math.Abs(Src.Y - Dst.Y) <= 1
                   && board.IsFree(Dst)
                   && board.PlayerOn(Src) == actor
                   && board.BallBearerForPlayer(actor) != Src;
        }


        public override GameState UpdateState(GameState state) {
            return state.MovePiece(Src, Dst);
        }

    }


    /// <summary>
    ///     Undo the last action of the player.
    /// </summary>
    public class Undo : IPlayerAction {

        public bool IsMoveValid(IPlayer actor, GameBoard board, int movesLeft) {
            return movesLeft < Game.MaxMovesPerTurn; // can only undo actions the player has played himself
        }

    }


    /// <summary>
    ///     End the turn and give initiative to the other player prematurely.
    /// </summary>
    public class Pass : IUpdateAction {

        public bool IsMoveValid(IPlayer actor, GameBoard board, int movesLeft) {
            return movesLeft < Game.MaxMovesPerTurn; // at least one move has been played
        }


        public GameState UpdateState(GameState state) {
            return state.Pass();
        }

    }
}
