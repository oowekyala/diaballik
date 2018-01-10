#include "stdafx.h"
#include "BoardAnalysis.hpp"

using namespace System::Linq;


namespace Diaballik {
	namespace AlgoLib {

		IEnumerable<MoveAction^>^ BoardAnalysis::AvailableMoves(BoardLike^ Board, Position2D p)
		{
			if (Board->HasBall(p))
			{
				IEnumerable<Position2D>^ dsts = BoardAnalysis::MovesForBall(Board, p);

				List<MoveAction^>^ result = gcnew List<MoveAction^>;
				for each (auto dst in dsts)
				{
					result->Add(MoveBallAction::New(p, dst));
				}

				return result;
			}
			else
			{
				IEnumerable<Position2D>^ dsts = BoardAnalysis::MovesForPiece(Board, p);
				List<MoveAction^>^ result = gcnew List<MoveAction^>;
				for each (auto dst in dsts)
				{
					result->Add(MovePieceAction::New(p, dst));
				}

				return result;
			}

		}


		IEnumerable<Position2D>^ BoardAnalysis::MovesForPiece(BoardLike^ Board, Position2D p)
		{

			CLIASSERT(Board->IsOnBoard(p) && !Board->IsFree(p), "Invalid position");

			if (Board->HasBall(p))
			{
				return System::Linq::Enumerable::Empty<Position2D>();
			}

			List<Position2D>^ neighbours = gcnew List<Position2D>;
			neighbours->Add(Position2D(p.X - 1, p.Y));
			neighbours->Add(Position2D(p.X + 1, p.Y));
			neighbours->Add(Position2D(p.X, p.Y - 1));
			neighbours->Add(Position2D(p.X, p.Y + 1));

			List<Position2D>^ validDst = gcnew List<Position2D>;

			for each(auto pos in neighbours)
			{
				if (Board->IsOnBoard(pos) && Board->IsFree(pos))
					validDst->Add(pos);
			}

			return validDst;
		}


		IEnumerable<Position2D>^ BoardAnalysis::MovesForBall(BoardLike^ Board, Position2D p)
		{

			CLIASSERT(Board->HasBall(p), "This piece doesn't currently carry the ball");


			List<Position2D>^ validDst = gcnew List<Position2D>;
			Player^ player = Board->PlayerOn(p);
			Helper helper(validDst, p, player, Board->GetOtherPlayer(player), Board);

			TryGetFriend(helper, -1, 0);    // top
			TryGetFriend(helper, 1, 0);     // bottom
			TryGetFriend(helper, 0, 1);     // right
			TryGetFriend(helper, 0, -1);    // left

			TryGetFriend(helper, 1, 1);     // bottom right
			TryGetFriend(helper, -1, 1);    // top right
			TryGetFriend(helper, -1, -1);   // top left
			TryGetFriend(helper, 1, -1);    // bottom left

			return validDst;
		}


		void BoardAnalysis::TryGetFriend(Helper help, int xstep, int ystep)
		{
			int x = help.startX, y = help.startY;

			Position2D p(x + xstep, y + ystep);
			Player^ curPlayer = help.Board->PlayerOn(p);

			while (curPlayer != help.player)
			{
				x += xstep;
				y += ystep;
				p = Position2D(x, y);
				if (!help.Board->IsOnBoard(p)) return;
				curPlayer = help.Board->PlayerOn(p);
				if (curPlayer == help.opponent) return;
			}

			help.accumulator->Add(p);
		}

		IEnumerable<Position2D>^ BoardAnalysis::ReachablePositionsHelper(BoardLike^ Board, int nbMoves, HashSet<Position2D>^ accumulator, HashSet<Position2D>^ edge)
		{
			if (nbMoves == 0) return accumulator;
			else
			{
				auto newEdge = gcnew HashSet<Position2D>;
				for each (auto pos in edge) // edge is the next positions to develop, on the edge of the search area
				{
					for each (auto p in Neighbours(Board, pos))
					{
						if (Board->IsOnBoard(p) && Board->IsFree(p))
						{
							if (!accumulator->Contains(p))
							{
								accumulator->Add(p);
								newEdge->Add(p);
							}
						}
					}
				}
				return ReachablePositionsHelper(Board, nbMoves - 1, accumulator, newEdge);
			}
		}


		IEnumerable<Position2D>^ BoardAnalysis::ReachablePositions(BoardLike^ Board, Position2D src, int nbMoves) {
			auto acc = gcnew HashSet<Position2D>();
			acc->Add(src);
			auto edge = gcnew HashSet<Position2D>();
			edge->Add(src);

			return ReachablePositionsHelper(Board, nbMoves, acc, edge);
		}


		IEnumerable<Position2D>^ BoardAnalysis::Neighbours(BoardLike^ Board, Position2D src) {
			List<Position2D>^ res = gcnew List<Position2D>;
			Position2D haut = Position2D::New(src.X - 1, src.Y);
			if (Board->IsOnBoard(haut)) res->Add(haut);
			Position2D bas = Position2D::New(src.X + 1, src.Y);
			if (Board->IsOnBoard(bas)) res->Add(bas);
			Position2D droite = Position2D::New(src.X, src.Y + 1);
			if (Board->IsOnBoard(droite)) res->Add(droite);
			Position2D gauche = Position2D::New(src.X, src.Y - 1);
			if (Board->IsOnBoard(gauche)) res->Add(gauche);
			return res;
		}

		bool BoardAnalysis::EnemyNeighbours(BoardLike^ Board, Player^ player, Position2D src) {
			List<Position2D>^ res = gcnew List<Position2D>;
			Player^ adversary = Board->GetOtherPlayer(player);
			for each (auto pos in BoardAnalysis::Neighbours(Board, src))
			{
				if (Enumerable::Contains<Position2D>(Board->PositionsForPlayer(adversary), pos)) return true;
			}
			return false;
		}

		IEnumerable<Position2D>^ BoardAnalysis::GetTilesBetween(BoardLike^ Board, Position2D src, Position2D dest) {
			List<Position2D>^ res = gcnew List<Position2D>;
			if (src.Y == dest.Y) {
				if (src.X < dest.X) {
					return GetTiles(Board, src, dest, 1, 0);
				}
				else if (src.X > dest.X) {
					return GetTiles(Board, src, dest, -1, 0);
				}
			}
			else if (src.X == dest.X) {
				if (src.Y < dest.Y) {
					return GetTiles(Board, src, dest, 0, 1);
				}
				else if (src.Y > dest.Y) {
					return GetTiles(Board, src, dest, 0, -1);
				}
			}
			else if (src.X < dest.X && src.Y < dest.Y) {
				return GetTiles(Board, src, dest, 1, 1);
			}
			else if (src.X < dest.X && src.Y > dest.Y) {
				return GetTiles(Board, src, dest, 1, -1);
			}
			else if (src.X > dest.X && src.Y > dest.Y) {
				return GetTiles(Board, src, dest, -1, -1);
			}
			else if (src.X > dest.X && src.Y < dest.Y) {
				return GetTiles(Board, src, dest, -1, 1);
			}

		}

		IEnumerable<Position2D>^ BoardAnalysis::GetTiles(BoardLike^ Board, Position2D src, Position2D dest, int xstep, int ystep)
		{
			int x = src.X + xstep, y = src.Y + ystep;
			List<Position2D>^ res = gcnew List<Position2D>;
			Position2D p(x + xstep, y + ystep);
			while (x != dest.X && y != dest.Y)
			{
				p = Position2D(x, y);
				if (Board->IsOnBoard(p)) res->Add(p);
				// CLIASSERT(Board->IsOnBoard(p), "Out of the board (GetTiles function)");
				x += xstep;
				y += ystep;
			}
			return res;
		}
	}
}

