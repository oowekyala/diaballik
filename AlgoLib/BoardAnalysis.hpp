﻿#pragma once
#include "stdafx.h"

using namespace System::Collections::Generic;
using namespace System;
using namespace Diaballik::Core;
using namespace System::Runtime::CompilerServices;

namespace Diaballik{
	namespace AlgoLib {

		/// Extension methods for GameBoard providing insight into the moves available to
		/// a player.
		///
		/// This is our implementation of the required algorithm library.
		[ExtensionAttribute]
		public ref class BoardAnalysis abstract sealed {
		public:
			/// Gets the set of positions on which this piece can be moved. 
			[ExtensionAttribute]
			static IEnumerable<Position2D>^ MovesForPiece(BoardLike^ Board, Position2D p);

			/// Gets the set of positions on which this ball can be moved. 
			[ExtensionAttribute]
			static IEnumerable<Position2D>^ MovesForBall(BoardLike^ Board, Position2D p);

			/// Gets the moves this piece can legally perform (either ball moves or piece moves). 
			[ExtensionAttribute]
			static IEnumerable<MoveAction^>^ AvailableMoves(BoardLike^ Board, Position2D p);

			/// Gets the set of positions reachable from one position in a maximum number of moves
			static IEnumerable<Position2D>^ ReachablePositions(BoardLike^ Board, Position2D src, int nbMoves);

			/// Gets the set of the tiles between src and dest
			static IEnumerable<Position2D>^ GetTilesBetween(BoardLike ^ Board, Position2D src, Position2D dest);

			/// Gets the set of adversary pieces among src neighbours
			static bool EnemyNeighbours(BoardLike^ Board, Player^ player, Position2D src);

		private:
			/// Helper class for the TryGetFriend algorithm. 
			value struct Helper
			{
				List<Position2D>^ accumulator;
				int startX;
				int startY;
				Player^ player;
				Player^ opponent;
				BoardLike^ Board;

				Helper(List<Position2D>^ acc, Position2D start, Player^ p, Player^ o, BoardLike^ board)
					: accumulator(acc), startX(start.X), startY(start.Y), player(p), opponent(o), Board(board) {}
			};

			///  Looks for a friendly piece on a piece-free line or diagonal,
			///  iterating with parameters xstep and ystep.
			///
			///  The result is put into the accumulator of the helper if it is found.
			static void TryGetFriend(Helper help, int xstep, int ystep);

			/// Gets the neighbours of a position
			static IEnumerable<Position2D>^ Neighbours(BoardLike^ Board, Position2D src);

			/// Recursive helper 
			static IEnumerable<Position2D>^ ReachablePositionsHelper(BoardLike^ Board, int nbMoves, HashSet<Position2D>^ accumulator, HashSet<Position2D>^ edge);

			/// Gets the set of the tiles between src and dest according to the direction given by xstep and ystep
			static IEnumerable<Position2D>^ GetTiles(BoardLike ^ Board, Position2D src, Position2D dest, int xstep, int ystep);
		};
	}
}
