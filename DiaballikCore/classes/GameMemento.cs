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
        public MementoNode Update(IUpdateAction action) {
            return new MementoNode(this, action);
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

        /// <summary>
        ///     Specialises MementoNode for an update action.
        /// </summary>
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
    public class MementoNode : GameMemento {
        #region Properties

        /// Action to perform on the previous state to get this state
        public IUpdateAction Action { get; }

        #endregion

        #region Constructor

        public MementoNode(GameMemento previous, IUpdateAction action) : base(previous) {
            Action = action;
        }

        #endregion

        #region Methods

        /// Cached game state
        private GameState _thisGameState;


        public override GameState ToState() {
            return _thisGameState ?? (_thisGameState = Action.UpdateState(Parent.ToState()));
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

        private int _hashCode;
        private bool _hashCodeIsValid;

        public override int GetHashCode() {
            if (_hashCodeIsValid) {
                return _hashCode;
            }
            _hashCodeIsValid = true;
            return _hashCode = Action.GetHashCode() * 31 + Parent.GetHashCode();
        }

        public static bool operator ==(MementoNode left, MementoNode right) {
            return Equals(left, right);
        }

        public static bool operator !=(MementoNode left, MementoNode right) {
            return !Equals(left, right);
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

        public Player Player1 => Specs.Item1.Player;
        public Player Player2 => Specs.Item2.Player;

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