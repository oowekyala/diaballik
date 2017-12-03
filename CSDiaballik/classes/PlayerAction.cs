namespace CSDiaballik {
    public interface IBoardAction {

    }


    /// <summary>
    ///     Represents an action the player can carry out during their turn.
    /// </summary>
    public interface IPlayerAction {

    }


    /// <summary>
    ///     Move the ball to another piece.
    /// </summary>
    public class MoveBallAction : IPlayerAction, IBoardAction {

        public MoveBallAction(Position2D src, Position2D dst) {
            Src = src;
            Dst = dst;
        }


        public Position2D Src { get; }
        public Position2D Dst { get; }

    }


    /// <summary>
    ///     Move a piece to a new location.
    /// </summary>
    public class MovePieceAction : IPlayerAction, IBoardAction {

        public MovePieceAction(Position2D p, Position2D dst) {
            Piece = p;
            Dst = dst;
        }


        public Position2D Piece { get; }
        public Position2D Dst { get; }

    }


    /// <summary>
    ///     Undo the last action of the player.
    /// </summary>
    public class UndoAction : IPlayerAction {

    }


    /// <summary>
    ///     End the turn and give initiative to the other player prematurely.
    /// </summary>
    public class PassAction : IPlayerAction {

    }
}
