#pragma once
#include "stdafx.h"
#include "AiDecisionAlgo.hpp"

namespace Diaballik::AlgoLib {
	
	public ref class StartingAiAlgo : public AiDecisionAlgo
	{
	public:
		PlayerAction^ NextMove(GameBoard^ board) override;
	};

}
