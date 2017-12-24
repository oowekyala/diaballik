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
    /// 
    ///     Equality between two GameMementos takes their full
    ///     ancestry into account. In the unlikely case that
    ///     proves unpractical, we could take shortcuts using 
    ///     hashes (think Git commit hash).
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
        public abstract GameState ToState();


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

        protected bool Equals(GameMemento other) {
            return Equals(Parent, other.Parent);
        }

        public override bool Equals(object obj) {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((GameMemento) obj);
        }

        public override int GetHashCode() {
            return (Parent != null ? Parent.GetHashCode() : 0);
        }

        /// Returns a description of this memento and its parents, if any.
        public string FullAncestryString() {
            return $"{this}\n{Parent?.ToString() ?? ""}";
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

        protected bool Equals(MementoNode other) {
            return Action.Equals(other.Action) && base.Equals(other);
        }

        public override bool Equals(object obj) {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((MementoNode) obj);
        }

        public override int GetHashCode() {
            return Action.GetHashCode();
        }

        public override string ToString() {
            return $"MementoNode({Action})"; // displaying all the parents was cumbersome
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


        public override GameState ToState() {
            return _thisGameState ?? (_thisGameState = ((UpdateAction) Action).UpdateState(Parent.ToState()));
        }
    }


    /// <inheritdoc />
    /// <summary>
    ///     Represents an undo action in the update chain. Special 
    ///     handling because we don't create a new state when undoing, 
    ///     we delegate the call to ToState on a previous node.
    /// </summary>
    public class UndoMementoNode : MementoNode {
        public UndoMementoNode(GameMemento memento, UndoAction undoAction) : base(memento, undoAction) {
        }


        public override GameState ToState() {
            return Parent.Parent.ToState();
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


        public override GameState ToState() {
            return _initialState ?? (_initialState = GameState.InitialState(BoardSize, Specs, IsFirstPlayerPlaying));
        }

        // equality members

        protected bool Equals(RootMemento other) {
            return BoardSize == other.BoardSize
                   && IsFirstPlayerPlaying == other.IsFirstPlayerPlaying
                   && Specs.Equals(other.Specs);
        }

        public override bool Equals(object obj) {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((RootMemento) obj);
        }

        public override int GetHashCode() {
            unchecked {
                var hashCode = BoardSize;
                hashCode = (hashCode * 397) ^ IsFirstPlayerPlaying.GetHashCode();
                hashCode = (hashCode * 397) ^ Specs.GetHashCode();
                return hashCode;
            }
        }

        public override string ToString() {
            return "Root Memento";
        }
    }
}