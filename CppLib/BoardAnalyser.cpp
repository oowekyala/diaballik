#include "BoardAnalyser.hpp"
#include <iostream>
#include <algorithm>
#include <time.h>
#include <math.h>


board_analyser::board_analyser(const int size) : _board_size(size)
{
	this->_board = new tile_status*[size];

	for (int x = 0; x < size; x++)
	{
		this->_board[x] = new tile_status[size];
	}
}

board_analyser::~board_analyser()
{
	for (int x = 0; x < _board_size; x++)
	{
		delete _board[x];
	}

	delete _board;
}

void board_analyser::set_status(const int x, const int y, int status) const
{
	_board[x][y] = static_cast<tile_status>(status);
}

void board_analyser::print_model() const
{
	for (int x = 0; x < _board_size; x++)
	{
		for (int y = 0; y < _board_size; y++)
		{
			std::cout << _board[x][y];
		}
		std::cout << std::endl;
	}
}
