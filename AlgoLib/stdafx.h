// stdafx.h : include file for standard system include files,
// or project specific include files that are used frequently, but
// are changed infrequently
//

#pragma once

#include "targetver.h"

#define WIN32_LEAN_AND_MEAN             // Exclude rarely-used stuff from Windows headers
// Windows Header Files:
//#include <windows.h>

#ifdef _DEBUG
#define CLIASSERT(condition, ...) System::Diagnostics::Debug::Assert(condition, ##__VA_ARGS__)
#else
#define CLIASSERT(condition, ...) // This macro will completely evaporate in Release.
#endif

//// that was an ugly hack to fix intellisense fucking up, may be useful someday
//typedef Diaballik::Core::Position2D Position2D;
//typedef Diaballik::Core::MoveAction MoveAction;
//typedef Diaballik::Core::MoveBallAction MoveBallAction;
//typedef Diaballik::Core::MovePieceAction MovePieceAction;
//typedef Diaballik::Core::PlayerAction PlayerAction;
//typedef Diaballik::Core::GameBoard GameBoard;
//typedef Diaballik::Core::IPlayer IPlayer;


// TODO: reference additional headers your program requires here
