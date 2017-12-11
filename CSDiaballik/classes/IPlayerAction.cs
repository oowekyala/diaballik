﻿using System;
using CppDiaballik;
using CppDiaballik.PlayerActions;

namespace CSDiaballik {
    /// <summary>
    ///     Represents an action the player can carry out during their turn.
    /// </summary>
    public abstract class PlayerAction {
        /// <summary>
        ///     Returns true if the move is valid, in the context of specified actor
        ///     player and the board.
        /// </summary>
        /// <param name="actor">The player making the move</param>
        /// <param name="board">The current state of the board</param>
        /// <param name="movesLeft">The number of moves left to the player for this turn</param>
        /// <returns>True if the move is valid</returns>
        public abstract bool IsMoveValid(IPlayer actor, GameBoard board, int movesLeft);

        /// <summary>
        ///     Converts a move descriptor (gotten from the Cpp) to a player action.
        /// </summary>
        /// <param name="desc">The descriptor</param>
        /// <returns>An update action</returns>
        public static implicit operator PlayerAction(ActionDescriptor desc) {
            switch (desc) {
                case MoveBallDescriptor mb:
                    return new MoveBallAction(mb.src, mb.dst);
                case MovePieceDescriptor mp:
                    return new MovePieceAction(mp.src, mp.dst);
                case PassDescriptor p:
                    return new PassAction();
                default: return null;
            }
        }
    }


    /// <inheritdoc />
    /// <summary>
    ///     Action that can update the game's state. Undo is excluded from this
    ///     category, as it's implemented by state switch in Game.
    /// </summary>
    public abstract class UpdateAction : PlayerAction {
        /// <summary>
        ///     Updates the given current state with this action.
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        public abstract GameState UpdateState(GameState state);
    }


    /// <inheritdoc />
    /// <summary>
    ///     Base class for MovePiece and MoveBall
    /// </summary>
    public abstract class MoveAction : UpdateAction {
        public Position2D Src { get; }
        public Position2D Dst { get; }


        protected MoveAction(Position2D src, Position2D dst) {
            if (src == dst) throw new ArgumentException("Illegal: cannot move to the same piece");
            Src = src;
            Dst = dst;
        }


        public abstract override GameState UpdateState(GameState state);

        public abstract override bool IsMoveValid(IPlayer actor, GameBoard board, int movesLeft);
    }


    /// <inheritdoc />
    /// <summary>
    ///     Move the ball to another piece.
    /// </summary>
    public class MoveBallAction : MoveAction {
        public MoveBallAction(Position2D src, Position2D dst) : base(src, dst) {
        }


        public override bool IsMoveValid(IPlayer actor, GameBoard board, int movesLeft) {
            return movesLeft > 0
                   && board.PlayerOn(Src) == board.PlayerOn(Dst)
                   && board.PlayerOn(Src) == actor
                   && board.IsLineFreeBetween(Src, Dst);
        }


        public override GameState UpdateState(GameState state) {
            return state.MoveBall(Src, Dst);
        }
    }


    /// <inheritdoc />
    /// <summary>
    ///     Move a piece to a new location.
    /// </summary>
    public class MovePieceAction : MoveAction {
        public MovePieceAction(Position2D src, Position2D dst) : base(src, dst) {
        }


        public override bool IsMoveValid(IPlayer actor, GameBoard board, int movesLeft) {
            return movesLeft > 0
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


    /// <inheritdoc />
    /// <summary>
    ///     Undo the last action of the player.
    /// </summary>
    public class UndoAction : PlayerAction {
        public override bool IsMoveValid(IPlayer actor, GameBoard board, int movesLeft) {
            return movesLeft < Game.MaxMovesPerTurn; // can only undo actions the player has played himself
        }
    }


    /// <inheritdoc />
    /// <summary>
    ///     End the turn and give initiative to the other player prematurely.
    /// </summary>
    public class PassAction : UpdateAction {
        public override bool IsMoveValid(IPlayer actor, GameBoard board, int movesLeft) {
            return movesLeft < Game.MaxMovesPerTurn; // at least one move has been played
        }


        public override GameState UpdateState(GameState state) {
            return state.Pass();
        }
    }
}