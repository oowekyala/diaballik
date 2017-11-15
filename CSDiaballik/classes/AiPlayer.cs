using System.Drawing;

namespace CSDiaballik
{
    public abstract class AiPlayer : IPlayer
    {
        Color IPlayer.Color { get; }
        string IPlayer.Name { get; }
        Piece IPlayer.BallBearer { get; set; }
        public abstract PlayerAction GetNextMove();
        public AiLevel Level { get; }

        public enum AiLevel
        {
            Noob,
            Starting,
            Progressive
        }


    }

    public class NoobAiPlayer : AiPlayer
    {
        public override PlayerAction GetNextMove()
        {
            throw new System.NotImplementedException();
        }
    }

    public class StartingAiPlayer : AiPlayer
    {
        public override PlayerAction GetNextMove()
        {
            throw new System.NotImplementedException();
        }
    }

    public class ProgressiveAiPlayer : AiPlayer
    {
        public override PlayerAction GetNextMove()
        {
            throw new System.NotImplementedException();
        }
    }

}