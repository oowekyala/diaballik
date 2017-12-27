using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Diaballik.Core.Util;

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
        #region Properties

        /// <summary>
        ///     Gets the previous memento. Returns null if this is the root.
        /// </summary>
        /// <returns>The parent memento</returns>
        public GameMemento Parent { get; }

        /// True if <see cref="Undo"/> can be executed.
        /// A player can only undo actions they have played themselves.
        public bool CanUndo => ToState().NumMovesLeft < Game.MaxMovesPerTurn;

        /// True if <see cref="Redo"/> can be executed.
        /// A player can only call Redo if the last action taken was to call Undo.
        public bool CanRedo => this is UndoMementoNode;


        /// <summary>
        ///     Gets a lazy enumeration of all the parents of this memento.
        /// </summary>
        /// <returns>A lazy enumeration of the parents</returns>
        protected IEnumerable<GameMemento> Parents {
            get {
                var cur = Parent;
                while (cur != null) {
                    yield return cur;
                    cur = cur.Parent;
                }
            }
        }

        #endregion

        #region Base constructor

        protected GameMemento(GameMemento parent) {
            Parent = parent;
        }

        #endregion

        #region Abstract members

        /// <summary>
        ///     Turns this memento into a GameState.
        /// </summary>
        /// <returns>A game corresponding to this memento</returns>
        public abstract GameState ToState();

        #endregion

        #region Methods

        /// <summary>
        ///     Gets a new memento based on this one.
        /// </summary>
        /// <param name="action">The transition from this memento to the result</param>
        /// <returns>A new memento with this as its parent</returns>
        public UpdateActionNode Update(IUpdateAction action) {
            return new UpdateActionNode(this, action);
        }

        public UndoMementoNode Undo() {
            return new UndoMementoNode(this, new UndoAction());
        }

        public RedoMementoNode Redo() {
            return new RedoMementoNode(this, new RedoAction());
        }

        /// <summary>
        ///     Gets the nth parent of this memento. Indices start at 0, 
        ///     e.g. <code>m.GetNthParent(0) == m.Parent</code>.
        /// </summary>
        /// <param name="n"></param>
        /// <returns></returns>
        public GameMemento GetNthParent(int n) {
            return Parents.ElementAt(n);
        }


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

        public override string ToString() {
            return $"{GetType().Name} <{GetHashCode()}>";
        }

        /// Returns a description of this memento and its parents, if any.
        public string FullAncestryString() {
            return $"{this}\n{Parent?.FullAncestryString() ?? ""}";
        }

        #endregion

        #region Equality members

        protected bool Equals(GameMemento other) {
            return Equals(Parent, other.Parent);
        }

        public override bool Equals(object obj) {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((GameMemento) obj);
        }

        public abstract override int GetHashCode();

        public static bool operator ==(GameMemento left, GameMemento right) {
            return Equals(left, right);
        }

        public static bool operator !=(GameMemento left, GameMemento right) {
            return !Equals(left, right);
        }

        #endregion
    }


    /// <inheritdoc />
    /// <summary>
    ///     Abstract class for memento nodes that have a parent.
    ///     They store the transition from the previous state to
    ///     this one as a PlayerAction.
    /// </summary>
    public abstract class MementoNode : GameMemento {
        #region Properties

        /// Action to perform on the previous state to get this state
        public IPlayerAction Action { get; }

        #endregion

        #region Constructor

        protected MementoNode(GameMemento previous, IPlayerAction action) : base(previous) {
            Action = action;
        }

        #endregion


        #region Equality members

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
            return Action.GetHashCode() * 31 + Parent.GetHashCode();
        }

        public static bool operator ==(MementoNode left, MementoNode right) {
            return Equals(left, right);
        }

        public static bool operator !=(MementoNode left, MementoNode right) {
            return !Equals(left, right);
        }

        #endregion
    }

    /// <summary>
    ///     Specialises MementoNode for an update action.
    /// </summary>
    public sealed class UpdateActionNode : MementoNode {
        #region Properties

        public new IUpdateAction Action => (IUpdateAction) base.Action;

        #endregion

        #region Constructor

        public UpdateActionNode(GameMemento previous, IUpdateAction action) : base(previous, action) {
        }

        #endregion

        #region Methods

        /// Cached game state
        private GameState _thisGameState;


        public override GameState ToState() {
            return _thisGameState ?? (_thisGameState = Action.UpdateState(Parent.ToState()));
        }

        #endregion
    }

    /// <inheritdoc />
    /// <summary>
    ///     Represents an <see cref="T:Diaballik.Core.IHistoryAction" /> 
    ///     in the update chain. Shares functionality across Undo and Redo 
    ///     actions. 
    /// 
    ///     Both these classes get a previously computed state, jumping 
    ///     over same type nodes to allow repeating the action several times.
    /// </summary>
    public abstract class HistoryActionNode : MementoNode {
        #region Properties

        public new IHistoryAction Action => (IHistoryAction) base.Action;

        public abstract GameMemento IndirectionTarget { get; }

        #endregion

        #region Base constructor

        protected HistoryActionNode(GameMemento previous, IHistoryAction action) : base(previous, action) {
//            var previousSameTypeNodes = Parents.TakeWhile(m => GetType().IsInstanceOfType(m)).Count();
//             we have to jump over same type nodes
//            _restoredMemento = GetNthParent(2 * previousSameTypeNodes + 1);
        }

        #endregion

        #region Methods

        public override string ToString() {
            return $"{base.ToString()}, IndirectionTarget: {IndirectionTarget}";
        }

        #endregion
    }

    /// <inheritdoc />
    /// <summary>
    ///     Represents an Undo action. See the superclass.
    /// </summary>
    public sealed class UndoMementoNode : HistoryActionNode {
        #region Properties

        public new UndoAction Action => (UndoAction) base.Action;

        public override GameMemento IndirectionTarget { get; }
        public GameMemento StateTarget { get; }

        #endregion

        #region Constructor

        public UndoMementoNode(GameMemento previous, UndoAction undoAction) : base(previous, undoAction) {
            switch (previous) {
                case UndoMementoNode undoMementoNode:
                    IndirectionTarget = undoMementoNode.IndirectionTarget.Parent;
                    break;
                case RedoMementoNode redoMementoNode:
                    IndirectionTarget = redoMementoNode.IndirectionTarget;
                    break;
                case var otherMementoType:
                    IndirectionTarget = otherMementoType;
                    break;
            }

            // resolve indirection
            var cur = IndirectionTarget.Parent;
            while (cur is UndoMementoNode u) {
                cur = u.IndirectionTarget.Parent;
            }
            StateTarget = cur;
            if (StateTarget == null) throw new ArgumentException("Cannot undo on the root");
        }

        #endregion

        #region Methods

        public override GameState ToState() {
            return StateTarget.ToState();
        }

        public override string ToString() {
            return $"{base.ToString()}, StateTarget: {IndirectionTarget.Parent}";
        }

        #endregion
    }

    /// <summary>
    ///     Represents a Redo action. See the superclass.
    /// </summary>
    public sealed class RedoMementoNode : HistoryActionNode {
        #region Properties

        public new RedoAction Action => (RedoAction) base.Action;

        public override GameMemento IndirectionTarget { get; }

        #endregion

        #region Constructor

        public RedoMementoNode(GameMemento previous, RedoAction action) : base(previous, action) {
            switch (previous) {
                case HistoryActionNode historyAction:
                    IndirectionTarget = historyAction.IndirectionTarget;
                    break;
                case var otherMementoType:
                    IndirectionTarget = otherMementoType;
                    break;
            }
        }

        #endregion

        #region Methods

        public override GameState ToState() {
            return IndirectionTarget.ToState();
        }

        #endregion
    }

    /// <inheritdoc />
    /// <summary>
    ///     Contains enough info to build the initial state of the game. Has no parent.
    /// </summary>
    public sealed class RootMemento : GameMemento {
        #region Properties

        public int BoardSize { get; }
        public bool IsFirstPlayerPlaying { get; }
        public FullPlayerSpecPair Specs { get; }

        public IPlayer Player1 => Specs.Item1.Player;
        public IPlayer Player2 => Specs.Item2.Player;

        #endregion

        #region Constructor

        public RootMemento(FullPlayerSpecPair specs, int boardSize, bool isFirstPlayerPlaying) : base(null) {
            IsFirstPlayerPlaying = isFirstPlayerPlaying;
            BoardSize = boardSize;
            Specs = specs;
        }

        #endregion

        #region Methods

        // Cached state
        private GameState _initialState;

        public override GameState ToState() {
            return _initialState ?? (_initialState = GameState.InitialState(BoardSize, Specs, IsFirstPlayerPlaying));
        }

        #endregion

        #region Equality members

        public bool Equals(RootMemento other) {
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

        public static bool operator ==(RootMemento left, RootMemento right) {
            return Equals(left, right);
        }

        public static bool operator !=(RootMemento left, RootMemento right) {
            return !Equals(left, right);
        }

        #endregion
    }
}