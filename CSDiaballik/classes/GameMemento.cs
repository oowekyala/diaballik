namespace CSDiaballik
{
    public class GameMemento
    {
        public GameMemento PreviousState { get; }
        public GameBoardMemento Board { get; }


        public GameMemento(GameBoardMemento board, GameMemento memento)
        {
            Board = board;
            PreviousState = memento;
        }
    }
}