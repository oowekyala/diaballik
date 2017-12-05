using System;
using System.Drawing;

namespace CSDiaballik {
    public abstract class AiPlayer : AbstractPlayer {

        public enum AiLevel {

            Noob,
            Starting,
            Progressive

        }


        protected AiPlayer(Color color, string name) : base(color, name) {
        }


        public abstract AiLevel Level { get; }


        public sealed override bool IsAi() => true;


        public sealed override PlayerBuilder ToBuilder() => base.ToBuilder().SetIsAi(Level);

    }


    public class NoobAiPlayer : AiPlayer {

        public NoobAiPlayer(Color color, string name) : base(color, name) {
        }


        public override AiLevel Level => AiLevel.Noob;


        public override IPlayerAction GetNextMove(GameBoard board) {
            throw new NotImplementedException();
        }

    }


    public class StartingAiPlayer : AiPlayer {

        public StartingAiPlayer(Color color, string name) : base(color, name) {
        }


        public override AiLevel Level => AiLevel.Starting;


        public override IPlayerAction GetNextMove(GameBoard board) {
            throw new NotImplementedException();
        }

    }


    public class ProgressiveAiPlayer : AiPlayer {

        public ProgressiveAiPlayer(Color color, string name) : base(color, name) {
        }


        public override AiLevel Level => AiLevel.Progressive;


        public override IPlayerAction GetNextMove(GameBoard board) {
            throw new NotImplementedException();
        }

    }
}
