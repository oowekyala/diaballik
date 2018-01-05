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




		IEnumerable<Position2D>^ BoardAnalysis::DangerousPieces(BoardLike^ Board, Player^ player)
		{
			List<Position2D>^ threats = gcnew List<Position2D>;
			for each(auto pos in Board->PositionsForPlayer(Board->GetOtherPlayer(player)))
			{
				if (abs(pos.X - Board->GetRowIndexOfInitialLine(player)) <= 2) threats->Add(pos);
			}

			return threats;
		}

		IEnumerable<Position2D>^ BoardAnalysis::ReachablePositions(BoardLike^ Board, Position2D src, int nbMoves) {
			List<Position2D>^ res = gcnew List<Position2D>;
			if (nbMoves == 1) {
				for each(auto pos in Neighbours(src)) {
					if (IsReachable(Board, pos, src, nbMoves)) res->Add(pos);
				}
			}
			else if (nbMoves == 2) {
				for each(auto pos in Neighbours(src)) {
					for each (auto pos2 in Neighbours(pos)) {
						if (!src.Equals(pos2) && IsReachable(Board, pos2, src, nbMoves)) res->Add(pos2);
					}
				}
				Enumerable::Distinct(res);
			}
			else if (nbMoves == 3) {
				for each(auto pos in Neighbours(src)) {
					for each(auto pos2 in Neighbours(pos)) {
						for each (auto pos3 in Neighbours(pos2))
						{
							if (!Enumerable::Contains(Neighbours(src), pos3) && IsReachable(Board, pos3, src, nbMoves))res->Add(pos3);
						}
					}
				}
				Enumerable::Distinct(res);
			}
			return res;
		}

		bool BoardAnalysis::IsReachable(BoardLike^ Board, Position2D dest, Position2D src, int nbMoves) {
			if (nbMoves == 1) {
				if (Enumerable::Contains<Position2D>(Neighbours(src), dest) && Board->IsOnBoard(dest) && Board->IsFree(dest)) return true;
			}
			else if (nbMoves > 1) {
				for each(auto pos in Neighbours(dest)) {
					if (Board->IsOnBoard(pos) && Board->IsFree(pos)) {
						int i = 1;
						while (nbMoves - i != 0) {
							if (!IsReachable(Board, pos, dest, nbMoves - i)) break;
							else i++;
						}
						if (nbMoves - i == 0) return true;
					}
				}
			}
		}

		IEnumerable<Position2D>^ BoardAnalysis::Neighbours(Position2D src) {
			List<Position2D>^ res = gcnew List<Position2D>;
			Position2D haut = Position2D::New(src.X - 1, src.Y);
			Position2D bas = Position2D::New(src.X + 1, src.Y);
			Position2D droite = Position2D::New(src.X, src.Y + 1);
			Position2D gauche = Position2D::New(src.X, src.Y - 1);
			return res;
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
			while (p != dest)
			{
				p = Position2D(x, y);
				if (Board->IsOnBoard(p)) res->Add(p);
				CLIASSERT(!Board->IsOnBoard(p), "Out of the board (GetTiles function)");
				x += xstep;
				y += ystep;
			}
			return res;
		}
	}
}

