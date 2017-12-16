#include "stdafx.h"
#include "NoobAiAlgo.hpp"

using namespace System::Linq;

namespace Diaballik::AlgoLib {

	

	PlayerAction^ NoobAiAlgo::NextMove(GameBoard^ board, IPlayer^ player)
	{
		List<Position2D>^ ps = Enumerable::ToList(board->PositionsForPlayer(player));
		int i = 0;

		IEnumerable<MoveAction^>^ actions = BoardAnalysis::AvailableMoves(board, ps[i]);

		while (i < ps->Count && !Enumerable::Any(actions))
		{
			actions = BoardAnalysis::AvailableMoves(board, ps[++i]);
		}

		if (!Enumerable::Any(actions)) // cannot do anything
		{
			return gcnew PassAction();
		}

		return Enumerable::ElementAt(actions, 0);
	}
}
