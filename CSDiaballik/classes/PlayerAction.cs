using System;

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
        /// <param name="movesLeft"></param>
        /// <returns>True if the move is valid</returns>
        public abstract bool IsMoveValid(IPlayer actor, GameBoard board, int movesLeft);


        /// <summary>
        ///     Move the ball to another piece.
        /// </summary>
        public class MoveBall : PlayerAction {

            public MoveBall(Position2D src, Position2D dst) {
                Src = src;
                Dst = dst;
            }


            public Position2D Src { get; }
            public Position2D Dst { get; }


            public override bool IsMoveValid(IPlayer actor, GameBoard board, int movesLeft)
                => movesLeft > 0
                   && board.PlayerOn(Src) == board.PlayerOn(Dst)
                   && board.PlayerOn(Src) == actor
                   && board.IsLineFreeBetween(Src, Dst);

        }


        /// <summary>
        ///     Move a piece to a new location.
        /// </summary>
        public class MovePiece : PlayerAction {

            public MovePiece(Position2D p, Position2D dst) {
                Src = p;
                Dst = dst;
            }


            public Position2D Src { get; }
            public Position2D Dst { get; }


            public override bool IsMoveValid(IPlayer actor, GameBoard board, int movesLeft) {
                if (Src == Dst) {
                    throw new ArgumentException("Illegal: cannot move to the same piece");
                }
                return movesLeft > 0
                       && Src.X - Dst.X <= 1
                       && Src.Y - Dst.Y <= 1
                       && board.IsFree(Dst)
                       && board.PlayerOn(Src) == actor
                       && board.BallBearerForPlayer(actor) != Src;
            }

        }


        /// <summary>
        ///     Undo the last action of the player.
        /// </summary>
        public class Undo : PlayerAction {

            public override bool IsMoveValid(IPlayer actor, GameBoard board, int movesLeft) {
                return movesLeft < 3;
            }

        }


        /// <summary>
        ///     End the turn and give initiative to the other player prematurely.
        /// </summary>
        public class Pass : PlayerAction {

            public override bool IsMoveValid(IPlayer actor, GameBoard board, int movesLeft) {
                return movesLeft < 2; // at least one move has been played
            }

        }

    }
}
