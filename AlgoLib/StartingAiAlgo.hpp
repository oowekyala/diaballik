﻿#pragma once
#include "stdafx.h"
#include "AiDecisionAlgo.hpp"

namespace Diaballik::AlgoLib {
	
	public ref class StartingAiAlgo : public AiDecisionAlgo
	{
	public:
		IPlayerAction^ NextMove(GameBoard^ board, IPlayer^ player) override;
	};

}
