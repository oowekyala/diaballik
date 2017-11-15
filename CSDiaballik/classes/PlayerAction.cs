namespace CSDiaballik
{
    /// <summary>
    /// Represents an action the player can carry out during their turn.
    /// </summary>
    public class PlayerAction
    {
        /// <summary>
        /// End the turn and give initiative to the other player prematurely.
        /// </summary>
        public class Pass : PlayerAction
        {
        }

        /// <summary>
        /// Move the ball to another piece.
        /// </summary>
        public class MoveBall : PlayerAction
        {
            public Piece Dst { get; }

            public MoveBall(Piece dst)
            {
                Dst = dst;
            }
        }

        /// <summary>
        /// Move a piece to a new location.
        /// </summary>
        public class MovePiece : PlayerAction
        {
            public Piece Piece { get; }
            public Position2D Dst { get; }

            public MovePiece(Piece p, Position2D dst)
            {
                Piece = p;
                Dst = dst;
            }
        }

        public class Undo : PlayerAction{
            
        }
    }
}