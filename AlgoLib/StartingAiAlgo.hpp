﻿#pragma once
#include "stdafx.h"
#include "AiDecisionAlgo.hpp"
#include "NoobAiAlgo.hpp"
#include "BoardAnalysis.hpp"

namespace Diaballik{
	namespace AlgoLib {

		///
		/// TODO this implementation is really limited, and could be more efficient.
		/// We could compute travel costs from every position to its neighbours and
		/// store that in a single array, then explore each dangerous position's
		/// lines of views looking for an enemy piece, and recurse until we hit
		/// MaxMovesPerTurn - (cost of original tile) depth. This could give us all harmful
		/// MoveBall / MovePiece combinations for the next player's turn, which we
		/// could then try to hinder in as few moves as possible.
		///
		/// We could change AiDecisionAlgo to give back not only the next move, but
		/// all the moves for the turn, which would allow us to do the computation
		/// only once.
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
