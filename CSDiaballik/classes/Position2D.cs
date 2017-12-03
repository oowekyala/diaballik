using System.Collections.Generic;

namespace CSDiaballik {
    /// <summary>
    ///     Utility value to store a position.
    /// </summary>
    public struct Position2D {

        public int X { get; }
        public int Y { get; }


        public Position2D(int x, int y) {
            X = x;
            Y = y;
        }


        public IEnumerable<Position2D> Neighbours() {
            return new List<Position2D> {
                new Position2D(X - 1, Y),
                new Position2D(X + 1, Y),
                new Position2D(X, Y - 1),
                new Position2D(X, Y + 1)
            };
        }


        public override string ToString() {
            return "Position2D(" + X + ", " + Y + ")";
        }

    }
}
