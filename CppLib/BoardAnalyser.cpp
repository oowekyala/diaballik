﻿#include "BoardAnalyser.hpp"
#include <iostream>
#include <algorithm>
#include <time.h>
#include <math.h>
using namespace std;
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

//Sets the status of the tiles of the board
void board_analyser::set_status(const int x, const int y, int status) 
{
	_board[x][y] = static_cast<tile_status>(status);
	if (status == player_1) {
		_p1_pieces.push_back(x);
		_p1_pieces.push_back(y);
	}
	else if (status == player_2) {
		_p2_pieces.push_back(x);
		_p2_pieces.push_back(y);
	}
	_p1_pieces.push_back(x);
}

//Beta version
//Gets the possible moves for a piece at a given position
//(a move is composed of 2 integers : x destination and y destination)
int* board_analyser::get_possible_moves(int x, int y) const {
	//PLayer 1 moveBall suggestions
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

	//PLayer 2 moveBall suggestions
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
	//MovePiece suggestions
	else {
		int* possible_moves = new int[8];
		//Move piece forward
		if ((x + 1) < _board_size && _board[x + 1][y] == 0) {
			possible_moves[0] = x + 1;
			possible_moves[1] = y;
		}
		else {
			possible_moves[0] = -1;
			possible_moves[1] = -1;
		}
		//Move piece backward
		if ((x - 1) >= 0 && _board[x - 1][y] == 0) {
			possible_moves[2] = x - 1;
			possible_moves[3] = y;
		}
		else {
			possible_moves[2] = -1;
			possible_moves[3] = -1;
		}
		//Move piece rightward
		if ((y + 1) < _board_size &&_board[x][y + 1] == 0) {
			possible_moves[4] = x;
			possible_moves[5] = y + 1;
		}
		else {
			possible_moves[4] = -1;
			possible_moves[5] = -1;
		}
		//Move piece leftward
		if ((y - 1) >= 0 && _board[x][y - 1] == 0) {
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

//Beta version
//Gets the moves of a noob AI for a turn (moves are selected randomly)
//(a move is composed of 4 integers : x source, y source, x destination and y destination)
int* board_analyser::noob_IA_moves(int playerNumber) const {
	int nb_moves = rand() % 3 ;
	int* moves = new int[12];
	for (int i = 0; i < 12; i++) {
		moves[i] = -1;
	}
	int moveCount = 0;
	if (playerNumber == 1) {
		for (int i = 0; i < nb_moves; i++) {
			int PieceX = rand() % (_board_size - 2);
			if (PieceX % 2 == 0) {
				PieceX = (PieceX + 1) % _board_size;
			}
			int xPiece = _p1_pieces[PieceX];
			int PieceY = PieceX + 1;
			int* possibleMoves = this->get_possible_moves(PieceX, PieceY);
			int MoveX = rand() % 6;
			if (MoveX % 2 == 0) {
				MoveX = (MoveX + 1) % 8;
			}
			int xDest = possibleMoves[MoveX];
			int yDest = possibleMoves[MoveX + 1];
			moves[moveCount] = PieceX;
			moveCount++;
			moves[moveCount] = PieceY;
			moveCount++;
			moves[moveCount] = xDest;
			moveCount++;
			moves[moveCount] = yDest;
			moveCount++;
		}
	}
	else if (playerNumber == 2) {
		for (int i = 0; i < nb_moves; i++) {
			int PieceX = rand() % (_board_size - 2);
			if (PieceX % 2 == 0) {
				PieceX = (PieceX + 1) % _board_size;
			}
			int PieceY = PieceX + 1;
			int* possibleMoves = this->get_possible_moves(PieceX, PieceY);
			int MoveX = rand() % 6;
			if (MoveX % 2 == 0) {
				MoveX = (MoveX + 1) % 8;
			}
			int xDest = possibleMoves[MoveX];
			int yDest = possibleMoves[MoveX + 1];
			moves[moveCount] = PieceX;
			moveCount++;
			moves[moveCount] = PieceY;
			moveCount++;
			moves[moveCount] = xDest;
			moveCount++;
			moves[moveCount] = yDest;
			moveCount++;
		}
	}
	return moves;
}

//Beta version
//Gets the moves of a starting AI for a turn
//(a move is composed of 4 integers : x source, y source, x destination and y destination)
int* board_analyser::starting_IA_moves(int playerNumber) const {
	int dangerous_piece_index = dangerous_piece(playerNumber);
	int* moves = new int[12];
	for (int i = 0; i < 12; i++) {
		moves[i] = -1;
	}
	if (dangerous_piece_index != -1) {
		if (playerNumber == 1) {

			int x = _p2_pieces[dangerous_piece_index];
			int y = _p2_pieces[dangerous_piece_index + 1];
			int index = -1;
			int temp = y - _p1_pieces[1];
			for (int i = 0; i < _board_size; i = i + 2) {
				if (_p1_pieces[i] > x && abs(y - _p1_pieces[i + 1]) < temp) {
					temp = y - _p1_pieces[i + 1];
					index = i + 1;
				}
			}
			moves[0] = _p1_pieces[index - 1];
			moves[1] = _p1_pieces[index];
			int* possible_moves = get_possible_moves(moves[0], moves[1]);
			if (index < y) {
				for (int i = 1; i < 8; i = i + 2) {
					if (possible_moves[i] == moves[1] + 1)
						moves[3] = moves[1] + 1;
				}
			}
			else if (index > y) {
				for (int i = 1; i < 8; i = i + 2) {
					if (possible_moves[i] == moves[1] - 1)
						moves[3] = moves[1] - 1;
				}
			}
		}

		else if (playerNumber == 2) {
			int x = _p1_pieces[dangerous_piece_index];
			int y = _p1_pieces[dangerous_piece_index + 1];
			int index = -1;
			int temp = y - _p2_pieces[1];
			for (int i = 0; i < _board_size; i = i + 2) {
				if (_p2_pieces[i] > x && abs(y - _p2_pieces[i + 1]) < temp) {
					temp = y - _p2_pieces[i + 1];
					index = i + 1;
				}
			}
			moves[0] = _p2_pieces[index - 1];
			moves[1] = _p2_pieces[index];
			int* possible_moves = get_possible_moves(moves[0], moves[1]);
			if (index < y) {
				for (int i = 1; i < 8; i = i + 2) {
					if (possible_moves[i] == moves[1] + 1)
						moves[3] = moves[1] + 1;
				}
			}
			else if (index > y) {
				for (int i = 1; i < 8; i = i + 2) {
					if (possible_moves[i] == moves[1] - 1)
						moves[3] = moves[1] - 1;
				}
			}
		}
	}	
	else return noob_IA_moves(playerNumber);

}

//Beta version
//Returns the index of the piece that might be dangerous for the player
//(a piece is considered dangerous for a player if it is 2 or less turns away from being on this player's starting line)
int board_analyser::dangerous_piece(int playerNumber) const {
	if (playerNumber == 1) {
		for (int i = _board_size-1; i >= 0; i = i - 2) {
			if ((_board_size - 1) - _p2_pieces[i] < 3) return i;
		}
	}
	else if (playerNumber == 2) {
		for (int i = 0; i < _board_size; i = i + 2) {
			if (_p1_pieces[i] < 3) return i;
		}
	}
	return -1;
}