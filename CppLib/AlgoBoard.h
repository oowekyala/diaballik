#pragma once



class AlgoBoard {

public:
	AlgoBoard() {}
	~AlgoBoard() {}

	// You can change the return type and the parameters according to your needs.
	void fillMap(int size);
};


#define EXPORTCDECL extern "C" __declspec(dllexport)
//
// export all C++ class/methods as friendly C functions to be consumed by external component in a portable way
///

EXPORTCDECL void AlgoBoard_fillMap(AlgoBoard* algo, int size) {
	return algo->fillMap(size);
}

EXPORTCDECL AlgoBoard* AlgoBoard_new() {
	return new AlgoBoard();
}

EXPORTCDECL void AlgoBoard_delete(AlgoBoard* algo) {
	delete algo;
}
