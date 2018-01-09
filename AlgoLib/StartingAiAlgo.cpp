#include "stdafx.h"
#include "StartingAiAlgo.hpp"

using namespace System::Linq;
using namespace Diaballik::Core::Util;

namespace Diaballik {
	namespace AlgoLib {

		IEnumerable<Position2D>^ StartingAiAlgo::DangerousPieces(BoardLike^ Board, Player^ player)
		{
			List<Position2D>^ threats = gcnew List<Position2D>;
			for each(auto pos in Board->PositionsForPlayer(Board->GetOtherPlayer(player)))
			{
				if (Math::Abs(pos.X - Board->GetRowIndexOfInitialLine(player)) <= 2) threats->Add(pos);
			}

			return threats;
		}


		// Looks for the most dangerous adversary piece (i.e. the nearest from the player's starting line) among the dangerous pieces of the adversary
		Position2D StartingAiAlgo::MostDangerousPiece(GameState^ board, Player^ player)
		{
			IEnumerable<Position2D>^ threats = DangerousPieces(board, player);
			Position2D tmp = Enumerable::First<Position2D>(threats);
			for each (auto pos in threats) {
				if (Math::Abs(board->GetRowIndexOfInitialLine(player)-pos.X) < Math::Abs(board->GetRowIndexOfInitialLine(player) - tmp.X)) tmp = pos;
			}
			return tmp;
		}

		// Returns the nearest piece from an adversary piece
		Position2D StartingAiAlgo::NearestPieceFrom(GameState^ board, Player^ player, Position2D piece)
		{
			int playerBase = board->GetRowIndexOfInitialLine(player);
			Position2D nearest;
			int temp = board->BoardSize;
			for each (auto pos in board->PositionsForPlayer(player))
			{
				if (Math::Abs(pos.X - playerBase) < Math::Abs(piece.X - playerBase) && Math::Abs(pos.Y - piece.Y) < temp && !board->HasBall(pos)) {
					temp = Math::Abs(pos.Y - piece.Y);
					nearest = pos;
				}
			}
			return nearest;
		}

		// Returns a position to move from the src piece to the dest tile
		Position2D StartingAiAlgo::moveTo(GameState^ board, Position2D src, Position2D dest) {
			if (src.X < dest.X) {
				Position2D pos(src.X + 1, src.Y);
				if (board->IsFree(pos) && board->IsOnBoard(pos)) {
					return pos;
				}
			}
			else if (src.X > dest.X) {
				Position2D pos(src.X - 1, src.Y);
				if (board->IsFree(pos) && board->IsOnBoard(pos)) {
					return pos;
				}
			}
			else if (src.X == dest.X && src.Y < dest.Y) {
				Position2D pos(src.X, src.Y + 1);
				if (board->IsFree(pos) && board->IsOnBoard(pos)) {
					return pos;
				}
			}
			else if (src.X == dest.X && src.Y > dest.Y) {
				Position2D pos(src.X, src.Y - 1);
				if (board->IsFree(pos) && board->IsOnBoard(pos)) {
					return pos;
				}
			}
		}

		IUpdateAction^ StartingAiAlgo::NextMove(GameState^ board, Player^ player)
		{
			if (Enumerable::Any(DangerousPieces(board, player))) // verify if there is any dangerous piece for the player
			{
				Position2D threat = MostDangerousPiece(board, player);
				if (threat.X == board->GetRowIndexOfInitialLine(player)) { //the dangerous piece is on the player's starting line
					if (Enumerable::Contains<Position2D>(BoardAnalysis::MovesForBall(board, board->BallCarrierForPlayer(board->GetOtherPlayer(player))), threat)) { //verify if the player can pass the ball to the dangerous piece
						for each (auto pos in board->PositionsForPlayer(player))
						{
							if (pos != board->BallCarrierForPlayer(player)) {
								auto targets = Enumerable::Intersect(BoardAnalysis::ReachablePositions(board, pos, 1), BoardAnalysis::GetTilesBetween(board, board->BallCarrierForPlayer(board->GetOtherPlayer(player)), threat));
								if (Enumerable::Any(targets)) {
									Position2D p = moveTo(board, pos, Enumerable::ElementAt(targets, 0));
									if(!pos.Equals(p)) return MovePieceAction::New(pos, p);
								}
							}
						}
						return _noob->NextMove(board, player);
					}
					else return _noob->NextMove(board, player);
				}
				else {
					Position2D pieceToMove = NearestPieceFrom(board, player, threat);
					if (pieceToMove.Y == threat.Y) {
						if(board->NumMovesLeft != 3) return PassAction::New();
						else return _noob->NextMove(board, player);
					}
					else if (pieceToMove.Y < threat.Y) {
						Position2D dest = Position2D::New(pieceToMove.X, pieceToMove.Y + 1);
						if(board->IsFree(dest) && board->IsOnBoard(dest)) return MovePieceAction::New(pieceToMove, dest);
						else return _noob->NextMove(board, player);
					}
					else {
						Position2D dest = Position2D::New(pieceToMove.X, pieceToMove.Y - 1);
						if (board->IsFree(dest) && board->IsOnBoard(dest)) return MovePieceAction::New(pieceToMove, dest);
						else return _noob->NextMove(board, player);
					}
				}
			}
			else return _noob->NextMove(board, player);
		}
	}
}
