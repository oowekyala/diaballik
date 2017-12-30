#pragma once
#include "stdafx.h"
#include "AiDecisionAlgo.hpp"

namespace Diaballik::AlgoLib {

	public ref class NoobAiAlgo : public AiDecisionAlgo
	{
	private:
		/// <summary>Random number generator.</summary>
		static Random^ Rng = gcnew Random();

		/// <summary>Tries to get a movepiece.</summary>
		static IUpdateAction^ TryGetAMovePiece(GameState^ state, IEnumerable<Position2D>^ ps, Position2D ballCarrier);
		
		/// <summary>Tries to get a moveball, falling back on movePiece if not possible.</summary>
		static IUpdateAction^ TryGetAMoveBall(GameState^ state, IEnumerable<Position2D>^ ps, Position2D ballCarrier);

	public:
		/// <inheritdoc />
		/// <summary>
        ///	    Returns a randomly chosen move.
		///
        ///     The moves returned are approximately
		///		distributed around 50% MovePiece, 
		///		30% MoveBall, and 20% Pass.
        /// </summary>
		IUpdateAction^ NextMove(GameState^ board, Player^ player) override;
	};

}
