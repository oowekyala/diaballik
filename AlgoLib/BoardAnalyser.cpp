#include "stdafx.h"
#include "BoardAnalyser.hpp"
#include "Position2D.hpp"





namespace CppDiaballik
{

	HashSet<Position2D>^ hashset_of_collection(IEnumerable<Position2D>^ ps) {
		return ps->GetType() == HashSet<Position2D>::typeid
			? static_cast<HashSet<Position2D>^>(ps)
			: gcnew HashSet<Position2D>(ps);
	}


	BoardAnalyser::BoardAnalyser(IEnumerable<Position2D>^ p1, IEnumerable<Position2D>^ p2, Position2D ball1, Position2D ball2)
	: _player1(hashset_of_collection(p1)), _player2(hashset_of_collection(p2)), _ball1(ball1), _ball2(ball2) {}

	BoardAnalyser::~BoardAnalyser()
	{
		// looks ok
	}
	

	IEnumerable<Position2D>^ BoardAnalyser::MovesForPiece(Position2D p)
	{
		return nullptr;
	}

	IEnumerable<Position2D>^ BoardAnalyser::MovesForBall(Position2D p)
	{
		return nullptr;
	}
}
