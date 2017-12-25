using System.Collections.Generic;

namespace Diaballik.Core {
    /// <summary>
    ///     Utility datatype to store a position.
    /// </summary>
    public struct Position2D {
        public int X { get; }
        public int Y { get; }


        public Position2D(int x, int y) {
            X = x;
            Y = y;
        }


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


        public override string ToString() {
            return "(" + X + ", " + Y + ")";
        }


        public static Position2D New(int x, int y) {
            return new Position2D(x, y);
        }

    }
}