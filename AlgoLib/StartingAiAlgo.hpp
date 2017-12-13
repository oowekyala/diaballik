#pragma once
#include "stdafx.h"

#include "Position2D.hpp"
#include "BoardAnalyser.hpp"
#include "AiDecisionAlgo.hpp"

namespace CppDiaballik {
	
	public ref class StartingAiAlgo : public AiDecisionAlgo
	{
	public:
		PlayerActions::ActionDescriptor^ NextMove(BoardAnalyser^ analyser) override;
	};

}
