using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

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


        public static IPlayer DummyPlayer()
        {
            return new NoobAiPlayer(Color.AliceBlue, "dummy", Enumerable.Empty<Position2D>());
        }

        public static IPlayer DummyPlayer(Color color, string name, IEnumerable<Position2D> positions)
        {
            return new NoobAiPlayer(color, name, positions);
        }
        
        public static IPlayer DummyPlayer(IEnumerable<Position2D> positions)
        {
            return new NoobAiPlayer(Color.AntiqueWhite, "dummy2", positions);
        }
    }
}
