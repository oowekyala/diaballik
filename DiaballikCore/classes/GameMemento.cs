using System;
using System.Collections.Generic;

namespace Diaballik.Core {
    using FullPlayerSpecPair = ValueTuple<FullPlayerBoardSpec, FullPlayerBoardSpec>;


    /// <summary>
    ///     Represents the chain of states went through by
    ///     a game.
    /// 
    ///     They can be converted to the game they represent, 
    ///     and be serialized to and deserialized from XML 
    ///     using Diaballik.Players.MementoSerializationUtil.
    /// </summary>
    public abstract class GameMemento {
        /// <summary>
        ///     Gets the previous memento. Returns null if this is the root.
        /// </summary>
        /// <returns>The parent memento</returns>
        public GameMemento Parent { get; }


        protected GameMemento(GameMemento parent) {
            Parent = parent;
        }


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
                cur = cur.Parent;
            }
            nodes.Reverse();
            return ((RootMemento) cur, nodes);
        }
    }


    /// <inheritdoc />
    /// <summary>
    ///     Abstract class for memento nodes that have a parent.
    ///     They store the transition from the previous state to
    ///     this one as a PlayerAction.
    /// </summary>
    public abstract class MementoNode : GameMemento {
        /// Action to perform on the previous state to get this state
        public PlayerAction Action { get; }

        protected MementoNode(GameMemento previous, PlayerAction action) : base(previous) {
            Action = action;
        }
    }

    /// <summary>
    ///     Specialises MementoNode for an update action.
    /// </summary>
    public class ActionMementoNode : MementoNode {
        /// Cached game state
        private GameState _thisGameState;


        public ActionMementoNode(GameMemento previous, UpdateAction action) : base(previous, action) {
        }


        public override GameState ToGame() {
            return _thisGameState ?? (_thisGameState = ((UpdateAction) Action).UpdateState(Parent.ToGame()));
        }
    }


    /// <inheritdoc />
    /// <summary>
    ///     Represents an undo action in the update chain. Special 
    ///     handling because we don't create a new state when undoing, 
    ///     we delegate the call to ToGame on a previous node.
    /// </summary>
    public class UndoMementoNode : MementoNode {
        public UndoMementoNode(GameMemento memento, UndoAction undoAction) : base(memento, undoAction) {
        }

    
        public override GameState ToGame() {
            return Parent.Parent.ToGame();
        }
    }


    /// <inheritdoc />
    /// <summary>
    ///     Contains enough info to build the initial state of the game. Has no parent.
    /// </summary>
    public class RootMemento : GameMemento {
        public readonly int BoardSize;
        public readonly bool IsFirstPlayerPlaying;
        public readonly FullPlayerSpecPair Specs;

        public IPlayer Player1 => Specs.Item1.Player;
        public IPlayer Player2 => Specs.Item2.Player;

        // Cached state
        private GameState _initialState;


        public RootMemento(FullPlayerSpecPair specs, int boardSize, bool isFirstPlayerPlaying) : base(null) {
            IsFirstPlayerPlaying = isFirstPlayerPlaying;
            BoardSize = boardSize;
            Specs = specs;
        }

        public override GameState ToGame() {
            return _initialState ?? (_initialState = GameState.InitialState(BoardSize, Specs, IsFirstPlayerPlaying));
        }
    }
}