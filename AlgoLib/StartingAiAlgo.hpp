#pragma once
#include "stdafx.h"

#include "BoardAnalyser.hpp"
#include "AiDecisionAlgo.hpp"

namespace Diaballik::AlgoLib {
	
	public ref class StartingAiAlgo : public AiDecisionAlgo
	{
	public:
		PlayerAction^ NextMove(BoardAnalyser^ analyser) override;
	};

}
