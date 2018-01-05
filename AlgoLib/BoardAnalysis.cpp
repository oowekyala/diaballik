#include "stdafx.h"
#include "BoardAnalysis.hpp"


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

	}
}
