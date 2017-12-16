﻿#include "stdafx.h"
#include "BoardAnalysis.hpp"
#include <cstdlib>

using namespace Diaballik::Core;

namespace Diaballik::AlgoLib {
	
	public ref class AiDecisionAlgo abstract
	{
	public:
		virtual PlayerAction^ NextMove(GameBoard^ board, IPlayer^ player) = 0;
	};
}
