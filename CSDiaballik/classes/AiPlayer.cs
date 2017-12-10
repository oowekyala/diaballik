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


        public sealed override bool IsAi() {
            return true;
        }


        public sealed override PlayerBuilder ToBuilder() {
            return base.ToBuilder().SetIsAi(Level);
        }
    }


    public class NoobAiPlayer : AiPlayer {
        public NoobAiPlayer(Color color, string name) : base(color, name) {
        }


        public override AiLevel Level => AiLevel.Noob;


        public override IPlayerAction GetNextMove(GameBoard board) {
            var ba = BoardAnalyser.New(board);
            var moves = ba.NoobAiMoves(this);
            IPlayerAction action;
            if (moves.Count > 0) {
                action = new MovePieceAction(moves[0], moves[1]);
                moves.RemoveAt(0);
                moves.RemoveAt(1);
            } else action = new PassAction();
            return action;
        }
    }


    public class StartingAiPlayer : AiPlayer {
        public StartingAiPlayer(Color color, string name) : base(color, name) {
        }


        public override AiLevel Level => AiLevel.Starting;


        public override IPlayerAction GetNextMove(GameBoard board) {
            var ba = BoardAnalyser.New(board);
            var moves = ba.StartingAiMoves(this);
            IPlayerAction action;
            if (moves.Count > 0) {
                action = new MovePieceAction(moves[0], moves[1]);
                moves.RemoveAt(0);
                moves.RemoveAt(1);
            } else action = new PassAction();
            return action;
        }
    }


    public class ProgressiveAiPlayer : AiPlayer {
        int nbMoves;

        public ProgressiveAiPlayer(Color color, string name) : base(color, name) {
            nbMoves = 0;
        }


        public override AiLevel Level => AiLevel.Progressive;


        public override IPlayerAction GetNextMove(GameBoard board) {
            if (nbMoves < 10) {
                return new NoobAiPlayer(this.Color, this.Name).GetNextMove(board);
            } else {
                return new StartingAiPlayer(this.Color, this.Name).GetNextMove(board);
            }
        }
    }
}