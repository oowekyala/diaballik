#pragma once
#include <memory>
#include <vector>
#include <deque>

enum tile_status
{
	empty,
	player_1,
	player_2,
	ball_player_1,
	ball_player_2
};

/**
 *  Provides insight into board configurations, e.g. available moves and best move.
 *  Bound to a single board.
 */
class board_analyser
{
	int _board_size;
	tile_status** _board;
	std::vector<int> _p1_pieces;
	std::vector<int> _p2_pieces;

public:
	explicit board_analyser(int size);
	~board_analyser();

	/**
	 * Sets the status of one tile on the internal model of the board. Used to initialise the model.
	 * 
	 * These status are represented by the enum tile_status.
	 */
	void set_status(int x, int y, int status);

	int* get_possible_moves(int x, int y) const;

	int* noob_ai_moves(int player_number) const;

	int* starting_ai_moves(int player_number) const;

	int dangerous_piece(int player_number) const;
};

// TODO move ai decision logic into other classes

extern "C"
{
	__declspec(dllexport) board_analyser* new_board_analyser(int size)
	{
		return new board_analyser(size);
	}

	__declspec(dllexport) void del_board_analyser(board_analyser* ba)
	{
		delete ba;
	}

	__declspec(dllexport) void ba_set_status(board_analyser* ba, int x, int y, int status)
	{
		ba->set_status(x, y, status);
	}

	__declspec(dllexport) int* ba_get_possible_moves(board_analyser* ba, int x, int y)
	{
		return ba->get_possible_moves(x, y);
	}

	__declspec(dllexport) int* ba_noob_ai_moves(board_analyser* ba, int player_number)
	{
		return ba->noob_ai_moves(player_number);
	}

	__declspec(dllexport) int* ba_starting_ai_moves(board_analyser* ba, int player_number)
	{
		return ba->starting_ai_moves(player_number);
	}
}
