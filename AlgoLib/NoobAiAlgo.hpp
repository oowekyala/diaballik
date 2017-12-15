#pragma once
#include "stdafx.h"
#include "BoardAnalyser.hpp"
#include "AiDecisionAlgo.hpp"

namespace Diaballik::AlgoLib {

	public ref class NoobAiAlgo : public AiDecisionAlgo
	{
	public:
		PlayerAction^ NextMove(BoardAnalyser^ analyser) override;
	};

}
