using System.Collections.Generic;
using System.Drawing;

namespace CSDiaballik
{
    public abstract class AiPlayer : AbstractPlayer
    {
        public abstract AiLevel Level { get; }

        protected AiPlayer(Color color, string name, List<Piece> pieces) : base(color, name, pieces)
        {
        }


        public enum AiLevel
        {
            Noob,
            Starting,
            Progressive
        }
    }

    public class NoobAiPlayer : AiPlayer
    {
        public override AiLevel Level => AiLevel.Noob;

        public override PlayerAction GetNextMove()
        {
            throw new System.NotImplementedException();
        }

        public NoobAiPlayer(Color color, string name, List<Piece> pieces) : base(color, name, pieces)
        {
        }
    }

    public class StartingAiPlayer : AiPlayer
    {
        public override AiLevel Level => AiLevel.Starting;


        public override PlayerAction GetNextMove()
        {
            throw new System.NotImplementedException();
        }

        public StartingAiPlayer(Color color, string name, List<Piece> pieces) : base(color, name, pieces)
        {
        }
    }

    public class ProgressiveAiPlayer : AiPlayer
    {
        public override AiLevel Level => AiLevel.Progressive;

        public override PlayerAction GetNextMove()
        {
            throw new System.NotImplementedException();
        }

        public ProgressiveAiPlayer(Color color, string name, List<Piece> pieces) : base(color, name, pieces)
        {
        }
    }
}