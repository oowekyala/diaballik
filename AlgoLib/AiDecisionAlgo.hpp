#include "stdafx.h"
#include "BoardAnalysis.hpp"
#include <cstdlib>

using namespace Diaballik::Core;

namespace Diaballik::AlgoLib {
	
	public ref class AiDecisionAlgo abstract
	{
	public:
		/// <summary>
		///     Gets a valid update action to perform on the given state.
		/// </summary>
		/// <param name="state">The state</param>
		/// <param name="player">The player which should perform the action</param>
		/// <return>A valid move</return>
		virtual IUpdateAction^ NextMove(GameState^ state, IPlayer^ player) = 0;
	};
}
