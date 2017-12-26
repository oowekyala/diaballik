using System.Collections.Generic;

namespace Diaballik.Core {
    /// <summary>
    ///     Utility datatype to store a position.
    /// </summary>
    public struct Position2D {
        #region Properties

        public int X { get; }
        public int Y { get; }

        #endregion

        #region Constructor and factory

        public Position2D(int x, int y) {
            X = x;
            Y = y;
        }

        // to be used as method group
        public static Position2D New(int x, int y) {
            return new Position2D(x, y);
        }

        #endregion

        #region Methods

        public override string ToString() {
            return "(" + X + ", " + Y + ")";
        }

        #endregion

        #region Equality members

        public bool Equals(Position2D other) {
            return X == other.X && Y == other.Y;
        }


        public override bool Equals(object obj) {
            if (ReferenceEquals(null, obj)) return false;
            return obj is Position2D d && Equals(d);
        }


        public override int GetHashCode() {
            unchecked {
                return (X * 397) ^ Y;
            }
        }


        public static bool operator ==(Position2D a, Position2D b) {
            return a.Equals(b);
        }


        public static bool operator !=(Position2D a, Position2D b) {
            return !(a == b);
        }

        #endregion
    }
}