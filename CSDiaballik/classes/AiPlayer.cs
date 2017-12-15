using System.Drawing;
using CppDiaballik;

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

    /// <summary>
    ///    Ai player backed by an <see cref="AiDecisionAlgo"/>, which is used
    ///    as a Strategy pattern.
    /// </summary>
    public abstract class StrategyBackedAiPlayer : AiPlayer {
        private readonly AiDecisionAlgo _algo;


        protected StrategyBackedAiPlayer(Color color, string name, AiDecisionAlgo algo) : base(color, name) {
            _algo = algo;
        }

        public override PlayerAction GetNextMove(GameBoard board) {
            var ba = board.Analyser();
            return _algo.NextMove(ba);
        }
    }

    public class NoobAiPlayer : StrategyBackedAiPlayer {
        public NoobAiPlayer(Color color, string name) : base(color, name, new NoobAiAlgo()) {
        }


        public override AiLevel Level => AiLevel.Noob;
    }


    public class StartingAiPlayer : StrategyBackedAiPlayer {
        public StartingAiPlayer(Color color, string name) : base(color, name, new StartingAiAlgo()) {
        }


        public override AiLevel Level => AiLevel.Starting;
    }


    public class ProgressiveAiPlayer : AiPlayer {
        private int _nbMoves = 0;
        private NoobAiPlayer _noob;
        private StartingAiPlayer _starting;

        public ProgressiveAiPlayer(Color color, string name) : base(color, name) {
        }


        public override AiLevel Level => AiLevel.Progressive;

        private NoobAiPlayer Noob() {
            return _noob ?? (_noob = new NoobAiPlayer(Color, Name));
        }


        private StartingAiPlayer Starting() {
            return _starting ?? (_starting = new StartingAiPlayer(Color, Name));
        }

        public override PlayerAction GetNextMove(GameBoard board) {
            return _nbMoves++ < 10 ? Noob().GetNextMove(board) : Starting().GetNextMove(board);
        }
    }
}
