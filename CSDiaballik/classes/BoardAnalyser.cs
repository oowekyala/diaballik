using System;
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


        [DllImport("CppLib.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern void del_board_analyser(IntPtr ba);



        private BoardAnalyser(GameBoard board)
        {
            _underlying = new_board_analyser(board.Size);
            foreach (var pos in board.Player1Positions)
            {
                ba_set_status(_underlying, pos.X, pos.Y, 1);
            }

            foreach (var pos in board.Player2Positions)
            {
                ba_set_status(_underlying, pos.X, pos.Y, 2);
            }
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
    }
}