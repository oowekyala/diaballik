using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using CppDiaballik;


namespace CSDiaballik {
    /// <summary>
    ///     Wrapper around the c++ class board_analyser.
    /// </summary>
    public class BoardAnalyser {
        private readonly IntPtr _underlying;
        private readonly GameBoard _analysedBoard;
        private bool _disposed = false;

        // mirrors the enum in BoardAnalyser.hpp
        private enum TileStatus {
            Empty,
            Player1,
            Player2,
            BallPlayer1,
            BallPlayer2
        }


        [DllImport("CppLib.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr new_board_analyser(int size);

        [DllImport("CppLib.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern void ba_set_status(IntPtr ba, int x, int y, TileStatus status);

        [DllImport("CppLib.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr ba_get_possible_moves_for_piece(IntPtr ba, int x, int y);

        [DllImport("CppLib.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr ba_get_possible_moves_for_ball(IntPtr ba, int x, int y);

        [DllImport("CppLib.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr ba_noob_ai_moves(IntPtr ba, int playerNumber);

        [DllImport("CppLib.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr ba_starting_ai_moves(IntPtr ba, int playerNumber);

        [DllImport("CppLib.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern void del_board_analyser(IntPtr ba);


        private BoardAnalyser(GameBoard board) {
            _analysedBoard = board;
            _underlying = new_board_analyser(board.Size);
            for (var i = 0; i < board.Size; i++) {
                for (var j = 0; j < board.Size; j++) {
                    ba_set_status(_underlying, i, j, TileStatus.Empty);
                }
            }

            foreach (var pos in board.Player1Positions) {
                ba_set_status(_underlying, pos.X, pos.Y, TileStatus.Player1);
            }

            foreach (var pos in board.Player2Positions) {
                ba_set_status(_underlying, pos.X, pos.Y, TileStatus.Player2);
            }

            ba_set_status(_underlying, board.BallBearer1.X, board.BallBearer1.Y, TileStatus.BallPlayer1);
            ba_set_status(_underlying, board.BallBearer2.X, board.BallBearer2.Y, TileStatus.BallPlayer2);
        }

        /// <summary>
        ///     Returns a new analyser bound to the given game board.
        /// </summary>
        /// <param name="board">The board to analyse.</param>
        /// <returns>A new board analyser</returns>
        public static BoardAnalyser New(GameBoard board) {
            return new BoardAnalyser(board);
        }


        ~BoardAnalyser() {
            Dispose(false);
            del_board_analyser(_underlying);
        }


        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing) {
            if (_disposed)
                return;
            if (disposing) {
                del_board_analyser(_underlying);
            }
            _disposed = true;
        }

        /// <summary>
        ///     Returns the possible moves of the piece at the position pos.
        /// </summary>
        /// <param name="pos">The position from which the possible moves are calculated</param>
        public List<Position2D> GetPossibleMoves(Position2D pos) {
            if (_analysedBoard.IsFree(pos)) {
                throw new ArgumentException("No piece to move");
            }

            if (pos.Equals(_analysedBoard.BallBearer1)
                || pos.Equals(_analysedBoard.BallBearer2)) {
                return GetPossibleMovesForBall(pos);
            } else {
                return GetPossibleMovesForPiece(pos);
            }
        }


        private List<Position2D> GetPossibleMovesForPiece(Position2D pos) {
            var intptr = ba_get_possible_moves_for_piece(_underlying, pos.X, pos.Y);
            return ConvertArrayToPositionList(intptr, 8);
        }


        private List<Position2D> GetPossibleMovesForBall(Position2D pos) {
            var intptr = ba_get_possible_moves_for_ball(_underlying, pos.X, pos.Y);
            return ConvertArrayToPositionList(intptr, _analysedBoard.Size * 2);
        }

        // TODO these have nothing to do here, should be moved out and encapsulated into the players' implementation.

        /// <summary>
        ///     Returns the moves of a Noob AI for a turn. A move is composed of two 
        ///     positions: the source and the destination.
        /// </summary>
        /// <param name="player">The player making the moves</param>
        public List<Position2D> NoobAiMoves(IPlayer player) {
            var playerIndex = PlayerIndex(player);
            var intptr = ba_noob_ai_moves(_underlying, playerIndex);
            return ConvertArrayToPositionList(intptr, 12);
        }


        /// <summary>
        ///     Returns the moves of a Starting AI for a turn. A move is composed of two 
        ///     positions: the source and the destination.
        /// </summary>
        /// <param name="player">The player making the moves</param>
        public List<Position2D> StartingAiMoves(IPlayer player) {
            var playerIndex = PlayerIndex(player);
            var intptr = ba_starting_ai_moves(_underlying, playerIndex);
            return ConvertArrayToPositionList(intptr, 12);
        }


        private static List<Position2D> ConvertArrayToPositionList(IntPtr arr, int size) {
            var result = new List<Position2D>();
            var startingMoves = new int[size];

            Marshal.Copy(arr, startingMoves, 0, startingMoves.Length);

            for (var i = 0; i < size; i = i + 2) {
                if (startingMoves[i] != -1 && startingMoves[i + 1] != -1) {
                    result.Add(new Position2D(startingMoves[i], startingMoves[i + 1]));
                }
            }
            return result;
        }


        private int PlayerIndex(IPlayer player) {
            return _analysedBoard.Player1.Equals(player)
                ? 1
                : _analysedBoard.Player2.Equals(player)
                    ? 2
                    : throw new ArgumentException("Unknown player");
        }
    }
}