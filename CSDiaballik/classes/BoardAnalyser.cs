using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace CSDiaballik
{
    /// <summary>
    /// Wrapper around the c++ class board_analyser.
    /// </summary>
    public class BoardAnalyser
    {
        private readonly IntPtr _underlying;
        private bool disposed = false;

        [DllImport("CppLib.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr new_board_analyser(int size);

        [DllImport("CppLib.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern void ba_set_status(IntPtr ba, int x, int y, int status);

        [DllImport("..\\..\\..\\Debug\\CppLib.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr ba_get_possible_moves(IntPtr ba, int x, int y);

        [DllImport("..\\..\\..\\Debug\\CppLib.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr ba_noob_IA_moves(IntPtr ba, int playerNumber);

        [DllImport("..\\..\\..\\Debug\\CppLib.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr ba_starting_IA_moves(IntPtr ba, int playerNumber);

        [DllImport("CppLib.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern void del_board_analyser(IntPtr ba);



        private BoardAnalyser(GameBoard board)
        {
            _underlying = new_board_analyser(board.Size);
            for(int i=0; i<board.Size; i++)
            {
                for(int j=0; j<board.Size; j++)
                {
                    ba_set_status(_underlying, i, j, 0);
                }
            }
            foreach (var pos in board.Player1Positions)
            {
                ba_set_status(_underlying, pos.X, pos.Y, 1);
            }

            ba_set_status(_underlying, board.BallBearer1.X, board.BallBearer1.Y, 3);

            foreach (var pos in board.Player2Positions)
            {
                ba_set_status(_underlying, pos.X, pos.Y, 2);
            }

            ba_set_status(_underlying, board.BallBearer2.X, board.BallBearer2.Y, 4);
        }

        public static BoardAnalyser New(GameBoard board)
        {
            return new BoardAnalyser(board);
        }


        ~BoardAnalyser()
        {
            Dispose(false);
            del_board_analyser(_underlying);
        }


        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
                return;
            if (disposing)
            {
                del_board_analyser(_underlying);
            }
            disposed = true;
        }

        /// <summary>
        /// Returns the possible moves of the piece at the position pos
        /// </summary>
        /// <param name="board">The board of the game</param>
        /// <param name="pos">The position from which the possible moves are calculated</param>
        public List<Position2D> GetPossibleMoves(GameBoard board, Position2D pos)
        {
            int tabSize = 0;
            if (pos.Equals(board.BallBearer1) || pos.Equals(board.BallBearer2)){
                tabSize = board.Size * 2;
            } else tabSize = 8;
            var possibleMoves = new List<Position2D>();
            int[] moves = new int[tabSize];
            IntPtr intptr = ba_get_possible_moves(_underlying, pos.X, pos.Y);
            Marshal.Copy(intptr, moves, 0, moves.Length);
            for(int i=0; i < tabSize; i=i+2)
            {
                if (moves[i] != -1 && moves[i+1] != -1)
                {
                    possibleMoves.Add(new Position2D(moves[i], moves[i+1]));
                }
            }
            return possibleMoves;
        }

        /// <summary>
        /// Returns the moves of an Noob IA for a turn 
        /// (a move is composed of 2 Position2D : the source and the destination)
        /// </summary>
        /// <param name="board">The board of the game</param>
        /// <param name="player">The player making the moves</param>
        public List<Position2D> NoobIAMoves(GameBoard board, IPlayer player)
        {
            var moves = new List<Position2D>();
            int[] noob_moves = new int[12];
            if (player.Equals(board.Player1))
            {
                IntPtr intptr = ba_noob_IA_moves(_underlying, 1);
                Marshal.Copy(intptr, noob_moves, 0, noob_moves.Length);
            }else if (player.Equals(board.Player2))
            {
                IntPtr intptr = ba_noob_IA_moves(_underlying, 2);
                Marshal.Copy(intptr, noob_moves, 0, noob_moves.Length);
            }
            for (int i = 0; i < 12; i = i + 2)
            {
                
                if (noob_moves[i] != -1 && noob_moves[i + 1] != -1)
                {
                    moves.Add(new Position2D(noob_moves[i], noob_moves[i + 1]));
                }
            }
            return moves;
        }

        /// <summary>
        /// Returns the moves of an Starting IA for a turn
        /// (a move is composed of 2 Position2D : the source and the destination)
        /// </summary>
        /// <param name="board">The board of the game</param>
        /// <param name="player">The player making the moves</param>
        public List<Position2D> StartingIAMoves(GameBoard board, IPlayer player)
        {
            var moves = new List<Position2D>();
            int[] starting_moves = new int[12];
            if (player.Equals(board.Player1))
            {
                IntPtr intptr = ba_starting_IA_moves(_underlying, 1);
                Marshal.Copy(intptr, starting_moves, 0, starting_moves.Length);
            }
            else if (player.Equals(board.Player2))
            {
                IntPtr intptr = ba_starting_IA_moves(_underlying, 2);
                Marshal.Copy(intptr, starting_moves, 0, starting_moves.Length);
            }
            for (int i = 0; i < 12; i = i + 2)
            {

                if (starting_moves[i] != -1 && starting_moves[i + 1] != -1)
                {
                    moves.Add(new Position2D(starting_moves[i], starting_moves[i + 1]));
                }
            }
            return moves;
        }
    }
}