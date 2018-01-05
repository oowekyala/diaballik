#include "stdafx.h"
#include "StartingAiAlgo.hpp"

using namespace System::Linq;
using namespace Diaballik::Core::Util;

namespace Diaballik::AlgoLib {

	//Looks for the most dangerous adversary piece (i.e. the nearest for the player's starting line) among the dangerous pieces of the adversary
	Position2D StartingAiAlgo::MostDangerousPiece(GameState^ board, Player^ player) 
	{
		IEnumerable<Position2D>^ threats = BoardAnalysis::DangerousPieces(board, player);
		Position2D tmp = Enumerable::First<Position2D>(threats);
		for each (auto pos in threats) {
			if (pos.X < tmp.X) tmp = pos;
		}
		return tmp;
	}

	//Returns the nearest piece from an adversary piece
	Position2D StartingAiAlgo::NearestPieceFrom(GameState^ board, Player^ player, Position2D piece) 
	{
		int playerBase = board->GetRowIndexOfInitialLine(player);
		Position2D nearest;
		int temp = board->BoardSize;
		for each (auto pos in board->PositionsForPlayer(player))
		{
			if (abs(pos.X - playerBase) < abs(piece.X - playerBase) && abs(pos.Y - piece.Y) < temp) {
				temp = abs(pos.Y - piece.Y);
				nearest = pos;
			}				
		}
		return nearest;
	}

	//Returns a position to move from the src piece to the dest tile
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
		if (Enumerable::Any(BoardAnalysis::DangerousPieces(board, player))) // verify if there is any dangerous piece for the player
		{
			Position2D threat = MostDangerousPiece(board, player);
			if (threat.X == board->GetRowIndexOfInitialLine(player)) { //the dangerous piece is on the player's starting line
				if (Enumerable::Contains<Position2D>(BoardAnalysis::MovesForBall(board, board->BallCarrierForPlayer(board->GetOtherPlayer(player))), threat)) {
					for each (auto pos in board->PositionsForPlayer(player))
					{
						if (pos != board->BallCarrierForPlayer(player)) {
							auto targets = Enumerable::Union(BoardAnalysis::ReachablePositions(board, pos, 3), BoardAnalysis::GetTilesBetween(board, board->BallCarrierForPlayer(board->GetOtherPlayer(player)), threat));
							if (Enumerable::Any(targets)) {
								Position2D p = moveTo(board, pos, Enumerable::ElementAt(targets, 0));
								return MovePieceAction::New(pos, p);
							}
						}
					}
				}		
			}
			else {
				Position2D pieceToMove = NearestPieceFrom(board, player, threat);
				if (pieceToMove.Y == threat.Y) {
					return gcnew PassAction();
				}
				else if (pieceToMove.Y < threat.Y) {
					Position2D dest = Position2D::New(pieceToMove.X, pieceToMove.Y + 1);
					return MovePieceAction::New(pieceToMove, dest);
				}
				else {
					Position2D dest = Position2D::New(pieceToMove.X, pieceToMove.Y - 1);
					return MovePieceAction::New(pieceToMove, dest);
				}
			}
		}
		else return _noob->NextMove(board, player);
	}
}