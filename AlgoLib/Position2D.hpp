#pragma once
#include "stdafx.h"

namespace CppDiaballik {

	/**
	 * Utility class to represent a Position. Immutable struct.
	 * TODO should maybe be a class
	 */
	public value struct Position2D
	{
	private:
		int _x;
		int _y;
	public:

		property int X {
			int get()
			{
				return _x;
			}
		}

		property int Y {
			int get()
			{
				return _y;
			}
		}


		Position2D(int x, int y) : _x(x), _y(y) {
		}

		static bool operator==(Position2D lhs, Position2D rhs);
		static bool operator!=(Position2D lhs, Position2D rhs);

		virtual bool Equals(Object^ rhs) override;
		virtual bool Equals(Position2D rhs);
		virtual int GetHashCode() override;
		virtual System::String^ ToString() override;

		static Position2D New(int x, int y)
		{
			return Position2D(x, y);
		}
	};
}
