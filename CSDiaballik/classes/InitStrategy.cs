namespace CSDiaballik
{
    public interface IInitStrategy
    {
        /// <summary>
        /// Initialises a new GameBoard and returns it. Populates the pieces in each player.
        /// </summary>
        /// <param name="p1">First player</param>
        /// <param name="p2">Second player</param>
        /// <returns>A new initialised gameboard. </returns>
        GameBoard InitBoard(PlayerBuilder p1, PlayerBuilder p2);
    }



    public class StandardInitStrategy : IInitStrategy
    {
        public GameBoard InitBoard(PlayerBuilder p1, PlayerBuilder p2)
        {
            throw new System.NotImplementedException();
        }
    }


    public class BallRandomStrategy : IInitStrategy
    {
        public GameBoard InitBoard(PlayerBuilder p1, PlayerBuilder p2)
        {
            throw new System.NotImplementedException();
        }
    }

    public class EnemyAmongUsStrategy : IInitStrategy
    {
        public GameBoard InitBoard(PlayerBuilder p1, PlayerBuilder p2)
        {
            throw new System.NotImplementedException();
        }
    }
}