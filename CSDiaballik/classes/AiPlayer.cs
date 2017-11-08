﻿using System.Drawing;

namespace CSDiaballik
{
    public abstract class AiPlayer : IPlayer
    {
        public abstract Color Color { get; }
        public abstract string Name { get; }
        public abstract Piece BallBearer { get; set; }
        public abstract PlayerAction GetNextMove();
    }
}