namespace CSDiaballik
{
    /// <summary>
    ///     Utility value to store a position.
    /// </summary>
    public struct Position2D
    {
        public int X { get; }
        public int Y { get; }


        public Position2D(int x, int y)
        {
            X = x;
            Y = y;
        }


        public override string ToString() => "Position2D(" + X + ", " + Y + ")";
    }
}
