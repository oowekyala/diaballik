using System;
using System.Collections.Generic;

namespace CSDiaballik.Tests
{
    public class TestUtil
    {
        public static IEnumerable<Position2D> RandomPositions(int left, int boardSize)
        {
            var random = new Random();
            if (left-- > 0)
            {
                yield return new Position2D(random.Next(boardSize), random.Next(boardSize));
            }
        }
    }
}
