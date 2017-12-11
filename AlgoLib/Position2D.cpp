#include "stdafx.h"
#include "Position2D.hpp"

using namespace CppDiaballik;

bool Position2D::Equals(Object^ that)
{
	if (ReferenceEquals(nullptr, that)) return false;
	if (that->GetType() == Position2D::typeid) {
		return this->Equals(static_cast<Position2D>(that));
	}
	return false;
}

bool Position2D::Equals(Position2D that)
{
	return this->X == that.X && this->Y == that.Y;
}

bool Position2D::operator==(Position2D lhs, Position2D rhs)
{
	return lhs.Equals(rhs);
}

bool Position2D::operator!=(Position2D lhs, Position2D rhs)
{
	return !(lhs == rhs);
}

System::String^ Position2D::ToString()
{
	return  "(" + X + ", " + Y + ")";
}

int Position2D::GetHashCode()
{
	return (X * 397) ^ Y;
}