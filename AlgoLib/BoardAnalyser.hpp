#pragma once
#include "stdafx.h"
#include "Position2D.hpp"

using namespace System::Collections::Generic;
using namespace System;

namespace CppDiaballik {

	/** Analyses one board.
	  */
	public ref class BoardAnalyser
	{
	private:
		HashSet<Position2D>^ _player1;
		HashSet<Position2D>^ _player2;

		Position2D _ball1;
		Position2D _ball2;
		

	public:
		BoardAnalyser(IEnumerable<Position2D>^ p1, IEnumerable<Position2D>^ p2, 
					  Position2D ball1, Position2D ball2);

		~BoardAnalyser();

		IEnumerable<Position2D>^ MovesForPiece(Position2D p);

		IEnumerable<Position2D>^ MovesForBall(Position2D p);
	};

}
