using System;

namespace CSDiaballik {
    /// <summary>
    ///     Represents the game. This class appears mutable to clients.
    /// </summary>
    public class Game {

        public GameMemento Memento { get; }

        public IPlayer Player1 => Board.Player1;
        public IPlayer Player2 => Board.Player2;
        public int BoardSize => Board.Size;
        public GameBoard Board { get; }

        public IPlayer CurrentPlayer { get; private set; }


        private Game(GameBoard board, bool isFirstPlayerPlaying) {
            Board = board;
            CurrentPlayer = isFirstPlayerPlaying ? Player1 : Player2;
            Memento = new RootMemento(this);
        }


        public static Game New(GameBoard board, bool isFirstPlayerPlaying) {
            return new Game(board, isFirstPlayerPlaying);
        }


        public static Game New(GameBoard board) {
            return New(board, new Random().Next(0, 1) == 1);
        }


        /// <summary>
        ///     Updates this game with the given player action. May change the current player as well.
        /// </summary>
        /// <param name="action">The action to be played by the current player</param>
        /// <returns>This game</returns>
        public Game Update(Action action) {
            //TODO


            return this;
        }


        /// <summary>
        ///     Represents an action the player can carry out during their turn.
        /// </summary>
        public abstract class Action {

            public abstract void Execute(Game game);


            /// <summary>
            ///     Move the ball to another piece.
            /// </summary>
            public class MoveBall : Action {

                public MoveBall(Position2D src, Position2D dst) {
                    Src = src;
                    Dst = dst;
                }


                public override void Execute(Game game) {
                    game.Board.MoveBall(Src, Dst);
                }


                public Position2D Src { get; }
                public Position2D Dst { get; }

            }


            /// <summary>
            ///     Move a piece to a new location.
            /// </summary>
            public class MovePiece : Action {

                public MovePiece(Position2D p, Position2D dst) {
                    Piece = p;
                    Dst = dst;
                }


                public override void Execute(Game game) {
                    //TODO   
                }


                public Position2D Piece { get; }
                public Position2D Dst { get; }

            }


            /// <summary>
            ///     Undo the last action of the player.
            /// </summary>
            public class Undo : Action {

                public override void Execute(Game game) {
                    //TODO   
                }

            }


            /// <summary>
            ///     End the turn and give initiative to the other player prematurely.
            /// </summary>
            public class Pass : Action {

                public override void Execute(Game game) {
                    //TODO   
                }

            }

        }

    }
}
