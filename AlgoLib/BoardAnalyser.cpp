#include "stdafx.h"
#include "BoardAnalyser.hpp"



namespace Diaballik::AlgoLib
{

	HashSet<Position2D>^ hashset_of_collection(IEnumerable<Position2D>^ ps) {
		return ps->GetType() == HashSet<Position2D>::typeid
			? static_cast<HashSet<Position2D>^>(ps)
			: gcnew HashSet<Position2D>(ps);
	}

	Dictionary<Position2D, TileStatus>^ lookup_of_positions(HashSet<Position2D>^ p1, HashSet<Position2D>^ p2)
	{
		auto dic = gcnew Dictionary<Position2D, TileStatus>();
		for each(auto p in p1)
		{
			dic->Add(p, TileStatus::PLAYER_1);
		}

		for each(auto p in p2)
		{
			dic->Add(p, TileStatus::PLAYER_2);
		}
		return dic;
	}


	BoardAnalyser::BoardAnalyser(int size, IEnumerable<Position2D>^ p1, IEnumerable<Position2D>^ p2, Position2D ball1, Position2D ball2)
		: Size(size), _player1(hashset_of_collection(p1)), _player2(hashset_of_collection(p2)), _ball1(ball1), _ball2(ball2), _lookup(lookup_of_positions(_player1, _player2)) {	}

	BoardAnalyser::~BoardAnalyser()
	{
		// looks ok
	}


	bool BoardAnalyser::is_free(Position2D p)
	{
		return get_status(p) == TileStatus::EMPTY;
	}

	bool BoardAnalyser::is_on_board(Position2D p)
	{
		return p.X >= 0 && p.X < Size
			&& p.Y >= 0 && p.Y < Size;
	}


	TileStatus BoardAnalyser::get_status(Position2D p)
	{
		TileStatus val;
		_lookup->TryGetValue(p, val); // this puts EMPTY into val if there is no key
		return val;
	}


	IEnumerable<Position2D>^ BoardAnalyser::MovesForPiece(Position2D p)
	{
		if (!is_on_board(p) || is_free(p))
		{
			throw gcnew ArgumentException("Invalid position");
		}


		List<Position2D>^ validDst = gcnew List<Position2D>;

		for (int x = p.X - 1; x <= p.X + 1; x++)
		{
			for (int y = p.Y - 1; y <= p.Y + 1; y++)
			{
				Position2D pos(x, y);
				if (is_on_board(pos) && is_free(pos))
				{
					validDst->Add(pos);
				}
			}
		}

		return validDst;
	}


	// Gets the opponent of the player if the status is a player, otherwise returns EMPTY
	TileStatus get_opponent(TileStatus stat)
	{
		if (stat == TileStatus::PLAYER_1) return TileStatus::PLAYER_2;
		else if (stat == TileStatus::PLAYER_2) return TileStatus::PLAYER_1;
		else return TileStatus::EMPTY; // signal value, must be handled
	}



	// iterates in one direction until either the target status or a stop status is reached
	void BoardAnalyser::try_get_friend(Helper help, int xstep, int ystep)
	{
		int x = help.startX, y = help.startY;
		TileStatus target = help.player, stop = help.opponent;
		TileStatus curstat;

		while (curstat != target)
		{
			x += xstep;
			y += ystep;
			Position2D p(x, y);
			curstat = get_status(p);
			if (curstat == stop || !is_on_board(p)) return;
		}

		help.accumulator->Add(Position2D(x, y));
	}



	IEnumerable<Position2D>^ BoardAnalyser::MovesForBall(Position2D p)
	{
		if (_ball1 != p && _ball2 != p)
		{
			throw gcnew ArgumentException("This piece doesn't currently carry the ball");
		}

		List<Position2D>^ validDst = gcnew List<Position2D>;
		auto player = get_status(p);

		Helper helper(validDst, p, player, get_opponent(player));

		try_get_friend(helper, -1, 0);	// left
		try_get_friend(helper, 1, 0);	// right
		try_get_friend(helper, 0, 1);	// top
		try_get_friend(helper, 0, -1);	// bottom

		try_get_friend(helper, 1, 1);	// top right
		try_get_friend(helper, -1, 1);	// top left
		try_get_friend(helper, -1, -1);	// bottom left
		try_get_friend(helper, 1, -1);	// bottom right

		return validDst;
	}
}
