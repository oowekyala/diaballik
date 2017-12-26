﻿using System;
using System.Drawing;
using Diaballik.AlgoLib;
using Diaballik.Core;


namespace Diaballik.Players {
    /// <summary>
    ///     Base class for Ai players. They're backed by a strategy object which 
    ///     conditions which move the player will choose given a gamestate.
    /// </summary>
    public abstract class AiPlayer : AbstractPlayer {
        #region Private fields

        private readonly AiDecisionAlgo _algo;

        #endregion

        #region Properties

        public abstract AiLevel Level { get; }

        public override bool IsAi { get; } = true;

        #endregion

        #region Constructor

        protected AiPlayer(Color color, string name, AiDecisionAlgo algo) : base(color, name) {
            _algo = algo;
        }

        #endregion

        #region Methods

        public override IPlayerAction GetNextMove(GameBoard board) {
            return _algo.NextMove(board, this);
        }

        public sealed override PlayerBuilder ToBuilder() {
            return base.ToBuilder().SetIsAi(Level);
        }

        #endregion
    }

    /// <summary>
    ///     Chooses moves randomly.
    /// </summary>
    public class NoobAiPlayer : AiPlayer {
        #region Constructor

        public NoobAiPlayer(Color color, string name) : base(color, name, new NoobAiAlgo()) {
        }

        #endregion

        #region Overridden properties

        public override AiLevel Level => AiLevel.Noob;

        #endregion
    }

    /// <summary>
    ///     Picks a move randomly, unless it's in danger, in which case it picks the move
    ///     less likely to make it lose on the next turn.
    /// </summary>
    public class StartingAiPlayer : AiPlayer {
        #region Constructor

        public StartingAiPlayer(Color color, string name) : base(color, name, new StartingAiAlgo()) {
        }

        #endregion

        #region Overridden properties

        public override AiLevel Level => AiLevel.Starting;

        #endregion
    }

    /// <summary>
    ///     Noob for the first 10 moves, then Starting.
    /// </summary>
    public class ProgressiveAiPlayer : AiPlayer {
        #region Constructor

        public ProgressiveAiPlayer(Color color, string name) : base(color, name, new ProgressiveAiAlgo()) {
        }

        #endregion

        #region Overridden properties

        public override AiLevel Level => AiLevel.Progressive;

        #endregion
    }

    /// <summary>
    ///     Backing strategy for the ProgressiveAiPlayer.
    /// </summary>
    public class ProgressiveAiAlgo : AiDecisionAlgo {
        #region Fields

        private int _nbMoves = 0;
        private readonly NoobAiAlgo _noob = new NoobAiAlgo();
        private readonly StartingAiAlgo _starting = new StartingAiAlgo();

        #endregion

        #region Overridden methods

        public override IPlayerAction NextMove(GameBoard board, IPlayer player) {
            return _nbMoves++ < 10 ? _noob.NextMove(board, player) : _starting.NextMove(board, player);
        }

        #endregion
    }
}