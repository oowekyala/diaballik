namespace CSDiaballik
{
    /// <summary>
    ///     Represents an action the player can carry out during their turn.
    /// </summary>
    public class PlayerAction
    {
        /// <summary>
        ///     End the turn and give initiative to the other player prematurely.
        /// </summary>
        public class Pass : PlayerAction
        {
        }

        /// <summary>
        ///     Move the ball to another piece.
        /// </summary>
        public class MoveBall : PlayerAction
        {
            public MoveBall(Piece src, Position2D dst)
            {
                Src = src.Position;
                Dst = dst;
            }


            public Position2D Src { get; }
            public Position2D Dst { get; }
        }

        /// <summary>
        ///     Move a piece to a new location.
        /// </summary>
        public class MovePiece : PlayerAction
        {
            public MovePiece(Piece p, Position2D dst)
            {
                Piece = p.Position;
                Dst = dst;
            }


            public Position2D Piece { get; }
            public Position2D Dst { get; }
        }

        /// <summary>
        ///     Undo the last action of the player.
        /// </summary>
        public class Undo : PlayerAction
        {
        }
    }
}
