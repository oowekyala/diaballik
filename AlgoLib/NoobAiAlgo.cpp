#include "stdafx.h"
#include "NoobAiAlgo.hpp"

using namespace System::Linq;

namespace Diaballik::AlgoLib {


	
	
	/// Tries to get a valid MovePiece.
	IUpdateAction^ NoobAiAlgo::TryGetAMovePiece(GameState^ state, IEnumerable<Position2D>^ ps, Position2D ballCarrier) {
		auto ls = Enumerable::ToList(ps);
		ls->Remove(ballCarrier);
		for each(auto p in ls) {
			auto movePieces = Enumerable::ToList(BoardAnalysis::AvailableMoves(state, p));
			if (movePieces->Count > 0) return movePieces[0];
		}
		// We make the assumption that this line is never reached,
		// which is confirmed by coverage analysis.
		return gcnew PassAction();
	}



	/// Tries to get a valid MoveBall, falling back on a MovePiece if none is available
	IUpdateAction^ NoobAiAlgo::TryGetAMoveBall(GameState^ state, IEnumerable<Position2D>^ ps, Position2D ballCarrier) {
		auto moveBalls = Enumerable::ToList(BoardAnalysis::AvailableMoves(state, ballCarrier));
		return moveBalls->Count > 0 ? moveBalls[0] : TryGetAMovePiece(state, ps, ballCarrier);
	}


	IUpdateAction^ NoobAiAlgo::NextMove(GameState^ state, IPlayer^ player)
	{
		auto ps = state->PositionsForPlayer(player);
		auto ballCarrier = state->BallCarrierForPlayer(player);

		const int movePieceProportion = 40; 
		const int moveBallProportion = 40;
		auto random = NoobAiAlgo::Rng->Next(100);

		if (random < movePieceProportion) {
			return TryGetAMovePiece(state, ps, ballCarrier);
		}
		if (random < movePieceProportion + moveBallProportion) {
			return TryGetAMoveBall(state, ps, ballCarrier);
		}
		auto pass = gcnew PassAction();
		return pass->IsValidOn(state) ? pass : TryGetAMovePiece(state, ps, ballCarrier);
	}
}
