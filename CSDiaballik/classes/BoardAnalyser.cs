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

        /*
        public List<Position2D> GetPossibleMoves(Position2D pos)
        {
            var possibleMoves = new List<Position2D>();
            int[][] moves = new int[4][];
            moves = ba_get_possible_moves(_underlying, pos.X, pos.Y);
            if(moves[0][0]!=-1 && moves[1][0] != -1)
            {
                possibleMoves.Add(new Position2D(moves[0][0], moves[1][0]));
            }
            if (moves[0][1] != -1 && moves[1][1] != -1)
            {
                possibleMoves.Add(new Position2D(moves[0][0], moves[1][0]));
            }
            if (moves[0][2] != -1 && moves[1][2] != -1)
            {
                possibleMoves.Add(new Position2D(moves[0][0], moves[1][0]));
            }
            if (moves[0][3] != -1 && moves[1][3] != -1)
            {
                possibleMoves.Add(new Position2D(moves[0][0], moves[1][0]));
            }
            return possibleMoves;
        }
        */
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
            Console.WriteLine(moves[0]);
            for(int i=0; i < tabSize; i=i+2)
            {
                if (moves[i] != -1 && moves[i+1] != -1)
                {
                    possibleMoves.Add(new Position2D(moves[i], moves[i+1]));
                }
            }/*
            if (moves[0] != -1 && moves[1] != -1)
            {
                possibleMoves.Add(new Position2D(moves[0], moves[1]));
            }
            if (moves[2] != -1 && moves[3] != -1)
            {
                possibleMoves.Add(new Position2D(moves[2], moves[3]));
            }
            if (moves[5] != -1 && moves[5] != -1)
            {
                possibleMoves.Add(new Position2D(moves[4], moves[5]));
            }
            if (moves[6] != -1 && moves[7] != -1)
            {
                possibleMoves.Add(new Position2D(moves[6], moves[7]));
            }*/
            return possibleMoves;
        }
    }
}