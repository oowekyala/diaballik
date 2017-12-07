//#include "stdafx.h"


//#include "stdafx.h"
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

/*
int** board_analyser::get_possible_moves(int x, int y) const {
	int** possible_moves = new int*[4];
	if ((x+1) < _board_size &&_board[x + 1][y] == empty) {
		possible_moves[0][0] = x + 1;
		possible_moves[1][0] = y;
	}else {
		possible_moves[0][0] = -1;
		possible_moves[1][0] = -1;
	}
	if ((x - 1) >= 0 && _board[x - 1][y] == empty) {
		possible_moves[0][1] = x - 1;
		possible_moves[1][1] = y;
	}else {
		possible_moves[0][1] = -1;
		possible_moves[1][1] = -1;
	}
	if ((y + 1) < _board_size &&_board[x][y+1] == empty) {
		possible_moves[0][2] = x;
		possible_moves[1][2] = y+1;
	}else {
		possible_moves[0][2] = -1;
		possible_moves[1][2] = -1;
	}
	if ((y - 1) >= 0 && _board[x][y-1] == empty) {
		possible_moves[0][3] = x;
		possible_moves[1][3] = y-1;
	}else {
		possible_moves[0][3] = -1;
		possible_moves[1][3] = -1;
	}
	return possible_moves;
}*/

int* board_analyser::get_possible_moves(int x, int y) const {
	if (_board[x][y] == ball_player_1) {
		int* possible_moves = new int[_board_size*2];
		int counter = 0;
		for (int i = x + 1; i < _board_size; i++) {
			if (_board[i][y] == player_1) {
				possible_moves[counter] = i;
				possible_moves[counter + 1] = y;
				counter += 2;
			}
			else if (_board[i][y] == player_2) break;
		}
		for (int i = x-1; i >=0 ; i--) {
			if (_board[i][y] == player_1) {
				possible_moves[counter] = i;
				possible_moves[counter+1] = y;
				counter += 2;
			}
			else if (_board[i][y] == player_2) break;
		}
		for (int i = y - 1; i >= 0; i--) {
			if (_board[x][i] == player_1) {
				possible_moves[counter] = x;
				possible_moves[counter + 1] = i;
				counter += 2;
			}
			else if (_board[x][i] == player_2) break;
		}
		for (int i = y + 1; i < _board_size; i++) {
			if (_board[x][i] == player_1) {
				possible_moves[counter] = x;
				possible_moves[counter + 1] = i;
				counter += 2;
			}
			else if (_board[i][y] == player_2) break;
		}

		int j = 1;
		for (int i = x + 1; i < _board_size; i++) {
			if ((y+j)<_board_size && _board[i][y+j] == player_1) {
				possible_moves[counter] = i;
				possible_moves[counter + 1] = y+j;
				counter += 2;
				j++;
			}
			else if (_board[i][y+j] == player_2) break;
		}

		j = 1;
		for (int i = x + 1; i < _board_size; i++) {
			if ((y-j)>=0 && _board[i][y - j] == player_1) {
				possible_moves[counter] = i;
				possible_moves[counter + 1] = y-j;
				counter += 2;
				j++;
			}
			else if ((y - j) >= 0 && _board[i][y-j] == player_2) break;
		}

		j = 1;
		for (int i = x - 1; i >= 0; i--) {
			if ((y + j)<_board_size && _board[i][y+j] == player_1) {
				possible_moves[counter] = i;
				possible_moves[counter + 1] = y+j;
				counter += 2;
				j++;
			}
			else if (_board[i][y+j] == player_2) break;
		}

		j = 1;
		for (int i = x - 1; i >= 0; i--) {
			if ((y - j) >= 0 && _board[i][y-j] == player_1) {
				possible_moves[counter] = i;
				possible_moves[counter + 1] = y-j;
				counter += 2;
				j++;
			}
			else if (_board[i][y-j] == player_2) break;
		}
		for (int i = counter; i < _board_size * 2; i++)
			possible_moves[i] = -1;
		return possible_moves;
	}
	else if (_board[x][y] == ball_player_2) {
		int* possible_moves = new int[_board_size * 2];
		int counter = 0;
		for (int i = x + 1; i < _board_size; i++) {
			if (_board[i][y] == player_2) {
				possible_moves[counter] = i;
				possible_moves[counter + 1] = y;
				counter += 2;
			}
			else if (_board[i][y] == player_1) break;
		}
		for (int i = x - 1; i >= 0; i--) {
			if (_board[i][y] == player_2) {
				possible_moves[counter] = i;
				possible_moves[counter + 1] = y;
				counter += 2;
			}
			else if (_board[i][y] == player_1) break;
		}
		for (int i = y - 1; i >= 0; i--) {
			if (_board[x][i] == player_2) {
				possible_moves[counter] = x;
				possible_moves[counter + 1] = i;
				counter += 2;
			}
			else if (_board[x][i] == player_1) break;
		}
		for (int i = y + 1; i < _board_size; i++) {
			if (_board[x][i] == player_2) {
				possible_moves[counter] = x;
				possible_moves[counter + 1] = i;
				counter += 2;
			}
			else if (_board[i][y] == player_1) break;
		}
		int j = 1;
		for (int i = x + 1; i < _board_size; i++) {
			if ((y + j)<_board_size && _board[i][y + j] == player_2) {
				possible_moves[counter] = i;
				possible_moves[counter + 1] = y + j;
				counter += 2;
				j++;
			}
			else if (_board[i][y + j] == player_1) break;
		}

		j = 1;
		for (int i = x + 1; i < _board_size; i++) {
			if ((y - j) >= 0 && _board[i][y - j] == player_2) {
				possible_moves[counter] = i;
				possible_moves[counter + 1] = y - j;
				counter += 2;
				j++;
			}
			else if ((y - j) >= 0 && _board[i][y - j] == player_1) break;
		}

		j = 1;
		for (int i = x - 1; i >= 0; i--) {
			if ((y + j)<_board_size && _board[i][y + j] == player_2) {
				possible_moves[counter] = i;
				possible_moves[counter + 1] = y + j;
				counter += 2;
				j++;
			}
			else if (_board[i][y + j] == player_1) break;
		}

		j = 1;
		for (int i = x - 1; i >= 0; i--) {
			if ((y - j) >= 0 && _board[i][y - j] == player_2) {
				possible_moves[counter] = i;
				possible_moves[counter + 1] = y - j;
				counter += 2;
				j++;
			}
			else if (_board[i][y - j] == player_1) break;
		}
		for (int i = counter; i < _board_size * 2; i++)
			possible_moves[i] = -1;
		return possible_moves;
	}
	else {
		int* possible_moves = new int[8];
		if ((x + 1) < _board_size && _board[x + 1][y] == empty) {
			possible_moves[0] = x + 1;
			possible_moves[1] = y;
		}
		else {
			possible_moves[0] = -1;
			possible_moves[1] = -1;
		}
		if ((x - 1) >= 0 && _board[x - 1][y] == empty) {
			possible_moves[2] = x - 1;
			possible_moves[3] = y;
		}
		else {
			possible_moves[2] = -1;
			possible_moves[3] = -1;
		}
		if ((y + 1) < _board_size &&_board[x][y + 1] == empty) {
			possible_moves[4] = x;
			possible_moves[5] = y + 1;
		}
		else {
			possible_moves[4] = -1;
			possible_moves[5] = -1;
		}
		if ((y - 1) >= 0 && _board[x][y - 1] == empty) {
			possible_moves[6] = x;
			possible_moves[7] = y - 1;
		}
		else {
			possible_moves[6] = -1;
			possible_moves[7] = -1;
		}
		return possible_moves;
	}
	
}

