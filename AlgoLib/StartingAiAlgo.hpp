#pragma once
#include "stdafx.h"
#include "AiDecisionAlgo.hpp"
#include "NoobAiAlgo.hpp"
#include "BoardAnalysis.hpp"

namespace Diaballik{
	namespace AlgoLib {


		public ref class StartingAiAlgo : public AiDecisionAlgo
		{
		private:
			NoobAiAlgo ^ _noob = gcnew NoobAiAlgo();

			/// Looks for the most dangerous adversary piece (i.e. the nearest for the player's starting line) among the dangerous pieces of the adversary
			Position2D MostDangerousPiece(GameState^ board, Player^ player);

			/// Returns the nearest piece from an adversary piece
			Position2D NearestPieceFrom(GameState^ board, Player^ player, Position2D pos);

			/// Returns a position to move from the src piece to the dest tile
			Position2D moveTo(GameState ^ board, Position2D src, Position2D dest);

			/// Gets the set of dangerous pieces (i.e. 2 lines away or less from the player starting line) for the player
			static IEnumerable<Position2D>^ DangerousPieces(BoardLike^ Board, Player^ player);

		public:
			IUpdateAction ^ NextMove(GameState^ board, Player^ player) override;
		};
	}
}
