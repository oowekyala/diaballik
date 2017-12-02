using System;
using System.Collections.Generic;
using System.Drawing;

namespace CSDiaballik
{
    public abstract class AiPlayer : AbstractPlayer
    {
        public enum AiLevel
        {
            Noob,
            Starting,
            Progressive
        }


        public sealed override bool IsAi() => true;


        protected AiPlayer(Color color, string name, IEnumerable<Position2D> pieces) : base(color, name, pieces)
        {
        }


        public abstract AiLevel Level { get; }
    }

    public class NoobAiPlayer : AiPlayer
    {
        public NoobAiPlayer(Color color, string name, IEnumerable<Position2D> pieces) : base(color, name, pieces)
        {
        }


        public override AiLevel Level => AiLevel.Noob;


        public override PlayerAction GetNextMove()
        {
            throw new NotImplementedException();
        }
    }

    public class StartingAiPlayer : AiPlayer
    {
        public StartingAiPlayer(Color color, string name, IEnumerable<Position2D> pieces) : base(color, name, pieces)
        {
        }


        public override AiLevel Level => AiLevel.Starting;


        public override PlayerAction GetNextMove()
        {
            throw new NotImplementedException();
        }
    }

    public class ProgressiveAiPlayer : AiPlayer
    {
        public ProgressiveAiPlayer(Color color, string name, IEnumerable<Position2D> pieces) : base(color, name, pieces)
        {
        }


        public override AiLevel Level => AiLevel.Progressive;


        public override PlayerAction GetNextMove()
        {
            throw new NotImplementedException();
        }
    }
}
