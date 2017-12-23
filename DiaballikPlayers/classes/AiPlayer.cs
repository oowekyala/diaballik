using System;
using System.Drawing;
using Diaballik.AlgoLib;
using Diaballik.Core;


namespace Diaballik.Players {
    /// <summary>
    ///     Base class for Ai players. They're backed by a strategy object which 
    ///     conditions which move the player will choose given a gamestate.
    /// </summary>
    public abstract class AiPlayer : AbstractPlayer {
        private readonly AiDecisionAlgo _algo;


        protected AiPlayer(Color color, string name, AiDecisionAlgo algo) : base(color, name) {
            _algo = algo;
        }

        public abstract AiLevel Level { get; }

        public override bool IsAi { get; } = true;


        public override PlayerAction GetNextMove(GameBoard board) {
            return _algo.NextMove(board, this);
        }

        public sealed override PlayerBuilder ToBuilder() {
            return base.ToBuilder().SetIsAi(Level);
        }
    }

    /// <summary>
    ///     Chooses moves randomly.
    /// </summary>
    public class NoobAiPlayer : AiPlayer {
        public NoobAiPlayer(Color color, string name) : base(color, name, new NoobAiAlgo()) {
        }

        public override AiLevel Level => AiLevel.Noob;
    }

    /// <summary>
    ///     Picks a move randomly, unless it's in danger, in which case it picks the move
    ///     less likely to make it lose on the next turn.
    /// </summary>
    public class StartingAiPlayer : AiPlayer {
        public StartingAiPlayer(Color color, string name) : base(color, name, new StartingAiAlgo()) {
        }

        public override AiLevel Level => AiLevel.Starting;
    }

    /// <summary>
    ///     Noob for the first 10 moves, then Starting.
    /// </summary>
    public class ProgressiveAiPlayer : AiPlayer {
        public ProgressiveAiPlayer(Color color, string name) : base(color, name, new ProgressiveAiAlgo()) {
        }

        public override AiLevel Level => AiLevel.Progressive;
    }

    /// <summary>
    ///     Backing strategy for the ProgressiveAiPlayer.
    /// </summary>
    public class ProgressiveAiAlgo : AiDecisionAlgo {
        private int _nbMoves = 0;
        private readonly NoobAiAlgo _noob = new NoobAiAlgo();
        private readonly StartingAiAlgo _starting = new StartingAiAlgo();

        public override PlayerAction NextMove(GameBoard board, IPlayer player) {
            return _nbMoves++ < 10 ? _noob.NextMove(board, player) : _starting.NextMove(board, player);
        }
    }
}