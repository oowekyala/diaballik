﻿#pragma once
#include "stdafx.h"
#include "AiDecisionAlgo.hpp"

namespace Diaballik::AlgoLib {

	public ref class NoobAiAlgo : public AiDecisionAlgo
	{
	public:
		PlayerAction^ NextMove(GameBoard^ board, IPlayer^ player) override;
	};

}
