#include "stdafx.h"
#include "BoardAnalysis.hpp"

using namespace Diaballik::Core;

namespace Diaballik::AlgoLib {
	
	public ref class AiDecisionAlgo abstract
	{
	public:
		virtual PlayerAction^ NextMove(GameBoard^ board) = 0;
	};
}
