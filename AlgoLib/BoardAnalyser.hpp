#pragma once
#include "stdafx.h"
#include "Position2D.hpp"
#include <vector>
#include <optional>

using namespace System::Collections::Generic;
using namespace System;

namespace CppDiaballik {

	private enum class TileStatus
	{
		EMPTY,		// default value
		PLAYER_1,
		PLAYER_2
	};


	/** Analyses one board. The internal state of a BoardAnalyser is 
	 *  nearly identical to that of the GameBoard, in fact, it would
	 *  be much simpler to not duplicate all state and make the analyser
	 *  inspect the GameBoard itself.
	 *  
	 *  TODO reorganise dependencies. Everything could live in the 
	 */
	public ref class BoardAnalyser
	{
	private:
		const int Size;

		HashSet<Position2D>^ _player1;
		HashSet<Position2D>^ _player2;

		Dictionary<Position2D, TileStatus>^ _lookup;

		Position2D _ball1;
		Position2D _ball2;

		/* Helper for the try_get_friend algorithm. */
		value struct Helper
		{
			int startX;
			int startY;
			TileStatus player;
			TileStatus opponent;
			List<Position2D>^ accumulator;

			Helper(List<Position2D>^ acc, Position2D start, TileStatus p, TileStatus o)
				: accumulator(acc), startX(start.X), startY(start.Y), player(p), opponent(o) {}
		};


		bool is_free(Position2D p);
		bool is_on_board(Position2D p);
		TileStatus get_status(Position2D p);
		void try_get_friend(Helper p, int xstep, int ystep);

	public:
		BoardAnalyser(int size, IEnumerable<Position2D>^ p1, IEnumerable<Position2D>^ p2,
			Position2D ball1, Position2D ball2);

		~BoardAnalyser();

		/** Gets the set of positions on which this piece can be moved. */
		IEnumerable<Position2D>^ MovesForPiece(Position2D p);

		/** Gets the set of positions on which this ball can be moved. */
		IEnumerable<Position2D>^ MovesForBall(Position2D p);

	};

}
