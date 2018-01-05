#pragma once
#include "stdafx.h"
#include "AiDecisionAlgo.hpp"

namespace Diaballik {
	namespace AlgoLib {

		public ref class StartingAiAlgo : public AiDecisionAlgo
		{
		public:
			IUpdateAction^ NextMove(GameState^ board, Player^ player) override;
		};

	}
}
