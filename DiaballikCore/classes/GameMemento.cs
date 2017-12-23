using System;
using System.Collections.Generic;

namespace Diaballik.Core {
    /// <summary>
    ///     Represents the chain of states went through by
    ///     a game.
    /// 
    ///     They can be converted to the game they represent, 
    ///     and reuse be serialized to and deserialized from 
    ///     XML using Diaballik.Players.MementoSerializationUtil.
    /// </summary>
    public abstract class GameMemento {
        /// <summary>
        ///     Gets the previous memento. Returns null if this is the root.
        /// </summary>
        /// <returns>The parent memento</returns>
        public abstract GameMemento GetParent();


        /// <summary>
        ///     Gets a new memento based on this one.
        /// </summary>
        /// <param name="action">The transition from this memento to the result</param>
        /// <returns>A new memento with this as its parent</returns>
        public MementoNode Append(PlayerAction action) {
            switch (action) {
                case UndoAction undo:
                    return new UndoMementoNode(this, undo);
                case UpdateAction up:
                    return new ActionMementoNode(this, up);
                default: throw new ArgumentOutOfRangeException();
            }
        }


        /// <summary>
        ///     Creates a new memento based on this one, caching the already computed state.
        /// </summary>
        /// <param name="state">The state obtained after the transition</param>
        /// <param name="action">The transition from this memento to the result</param>
        /// <returns>A new memento with this as its parent</returns>
        public MementoNode Append(GameState state, UpdateAction action) {
            return new ActionMementoNode(state, this, action);
        }


        /// <summary>
        ///     Turns this memento into a GameState.
        /// </summary>
        /// <returns>A game corresponding to this memento</returns>
        public abstract GameState ToGame();

        /// <summary>
        ///     Deconstructs this memento into its root and an enumerable of
        ///     MementoNodes, ordered chronologically.
        /// </summary>
        /// <returns>A pair of root and ordered nodes</returns>
        public (RootMemento, IEnumerable<MementoNode>) Deconstruct() {
            var nodes = new List<MementoNode>();
            var cur = this;
            while (cur is MementoNode n) {
                nodes.Add(n);
                cur = cur.GetParent();
            }
            nodes.Reverse();
            return ((RootMemento) cur, nodes);
        }
    }


    /// <inheritdoc />
    /// <summary>
    ///     Abstract class for memento nodes that have a parent.
    /// </summary>
    public abstract class MementoNode : GameMemento {
        /// Previous memento in the chain
        protected readonly GameMemento Previous;

        /// Action to perform on the previous state to get this state
        public PlayerAction Action { get; }

        protected MementoNode(GameMemento previous, PlayerAction action) {
            Previous = previous;
            Action = action;
        }


        public sealed override GameMemento GetParent() {
            return Previous;
        }
    }


    public class ActionMementoNode : MementoNode {
        /// Cached game state
        private GameState _thisGameState;


        public ActionMementoNode(GameState thisState, GameMemento previous, UpdateAction action) : base(previous,
                                                                                                        action) {
            _thisGameState = thisState;
        }


        public ActionMementoNode(GameMemento previous, UpdateAction action) : base(previous, action) {
        }


        // Only works with immutable games
        public override GameState ToGame() {
            return _thisGameState ?? (_thisGameState = ((UpdateAction) Action).UpdateState(Previous.ToGame()));
        }
    }


    /// <inheritdoc />
    /// <summary>
    ///     Represents an undo action in the update chain. Special 
    ///     handling because we don't create a new state when undoing, 
    ///     we delegate the call to ToGame to the previous nodes.
    /// </summary>
    public class UndoMementoNode : MementoNode {
        public UndoMementoNode(GameMemento memento, UndoAction undoAction) : base(memento, undoAction) {
        }


        public override GameState ToGame() {
            return Previous.GetParent().ToGame();
        }
    }


    /// <inheritdoc />
    /// <summary>
    ///     Contains enough info to build the initial state of the game. Has no parent.
    /// </summary>
    public class RootMemento : GameMemento {
        public readonly int BoardSize;
        public readonly bool IsFirstPlayerPlaying;
        public readonly (FullPlayerBoardSpec, FullPlayerBoardSpec) Specs;

        public IPlayer Player1 => Specs.Item1.Player;
        public IPlayer Player2 => Specs.Item2.Player;


        private GameState _initialState;


        public RootMemento(GameState initialState, (FullPlayerBoardSpec, FullPlayerBoardSpec) specs)
            : this(specs, initialState.BoardSize, initialState.CurrentPlayer == specs.Item1.Player) {
            _initialState = initialState;
            BoardSize = initialState.BoardSize;
            IsFirstPlayerPlaying = initialState.CurrentPlayer == Player1;
        }

        public RootMemento((FullPlayerBoardSpec, FullPlayerBoardSpec) specs, int boardSize, bool isFirstPlayerPlaying) {
            IsFirstPlayerPlaying = isFirstPlayerPlaying;
            BoardSize = boardSize;
            Specs = specs;
        }

        public override GameState ToGame() {
            return _initialState ?? (_initialState = GameState.InitialState(BoardSize, Specs, IsFirstPlayerPlaying));
        }


        public override GameMemento GetParent() {
            return null;
        }
    }
}