#include "stdafx.h"
#include "BoardAnalyser.hpp"
#include "Position2D.hpp"

namespace CppDiaballik {
	namespace PlayerActions {
		// Substitutes for IUpdateAction, as it's defined in C# code
		public ref class ActionDescriptor {};
		public ref class MovePieceDescriptor : ActionDescriptor
		{
		public:
			property Position2D src;
			property Position2D dst;
		};
		public ref class MoveBallDescriptor : ActionDescriptor
		{
		public:
			property Position2D src;
			property Position2D dst;
		};
		public ref class PassDescriptor : ActionDescriptor {};
	}

	public ref class AiDecisionAlgo abstract
	{
	public:
		virtual PlayerActions::ActionDescriptor^ NextMove(BoardAnalyser^ analyser) = 0;
	};
}
