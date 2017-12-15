#include "stdafx.h"
#include "BoardAnalyser.hpp"

using namespace Diaballik::Core;

namespace Diaballik::AlgoLib {
	
	public ref class AiDecisionAlgo abstract
	{
	public:
		virtual PlayerAction^ NextMove(BoardAnalyser^ analyser) = 0;
	};
}
