#include "stdafx.h"
#include "ProgressiveAiAlgo.hpp"

namespace Diaballik::AlgoLib {

	IUpdateAction^ ProgressiveAiAlgo::NextMove(GameState^ state, Player^ player)
	{
		return _nbMoves++ < 10 ? _noob->NextMove(state, player) : _starting->NextMove(state, player);
	}
}
