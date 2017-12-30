#pragma once
#include "stdafx.h"
#include "AiDecisionAlgo.hpp"
#include "NoobAiAlgo.hpp"
#include "StartingAiAlgo.hpp"

namespace Diaballik::AlgoLib {

	/// <summary>
	///		Behaves like a noob ai for the first ten turns, then like a starting ai.
	/// </summary>
	public ref class ProgressiveAiAlgo : public AiDecisionAlgo
	{
	private:
		int _nbMoves = 0;
		NoobAiAlgo^ _noob = gcnew NoobAiAlgo();
		StartingAiAlgo^ _starting = gcnew StartingAiAlgo();

	public:
		/// <inheritdoc />
		IUpdateAction^ NextMove(GameState^ board, Player^ player) override;
	};


}
