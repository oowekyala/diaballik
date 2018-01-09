#include "stdafx.h"
#include "NoobAiAlgo.hpp"

using namespace System::Linq;
using namespace Diaballik::Core::Util;



namespace Diaballik {
	namespace AlgoLib {

		generic<class T>
		IEnumerable<T>^ Shuffle(IEnumerable<T>^ l)
		{
			return ExtensionUtil::Shuffle(Enumerable::ToList(l));
		}


		IUpdateAction^ NoobAiAlgo::TryGetAMovePiece(GameState^ state, IEnumerable<Position2D>^ ps, Position2D ballCarrier)
		{
			auto shuffledPs = Shuffle(ps);
			for each(auto p in shuffledPs)
			{
				if (p == ballCarrier) continue;

				auto movePieces = Shuffle(BoardAnalysis::AvailableMoves(state, p));
				if (Enumerable::Any(movePieces)) return Enumerable::First(movePieces);
			}
			// We make the assumption that this line is never reached,
			// which is confirmed by coverage analysis.
			return gcnew PassAction();
		}


		IUpdateAction^ NoobAiAlgo::TryGetAMoveBall(GameState^ state, IEnumerable<Position2D>^ ps, Position2D ballCarrier)
		{
			auto moveBalls = Shuffle(BoardAnalysis::AvailableMoves(state, ballCarrier));
			return Enumerable::Any(moveBalls) ? Enumerable::First(moveBalls) : TryGetAMovePiece(state, ps, ballCarrier);
		}


		IUpdateAction^ NoobAiAlgo::NextMove(GameState^ state, Player^ player)
		{
			auto ps = state->PositionsForPlayer(player);
			auto ballCarrier = state->BallCarrierForPlayer(player);

			const int movePieceProportion = 50;
			const int moveBallProportion = 50;
			auto random = NoobAiAlgo::Rng->Next(100);

			if (random < movePieceProportion)
			{
				return TryGetAMovePiece(state, ps, ballCarrier);
			}
			if (random < movePieceProportion + moveBallProportion)
			{
				return TryGetAMoveBall(state, ps, ballCarrier);
			}
			auto pass = gcnew PassAction();
			return pass->IsValidOn(state) ? pass : TryGetAMovePiece(state, ps, ballCarrier);
		}
	}
}