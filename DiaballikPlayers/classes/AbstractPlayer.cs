﻿using System.Drawing;
using Diaballik.Core;

namespace Diaballik.Players {
    public abstract class AbstractPlayer : IPlayer {
        protected AbstractPlayer(Color color, string name) {
            Color = color;
            Name = name;
        }


        public Color Color { get; }

        public string Name { get; }

        public abstract bool IsAi();

        public abstract PlayerAction GetNextMove(GameBoard board);


        public virtual PlayerBuilder ToBuilder() => new PlayerBuilder {
            Color = Color,
            Name = Name
        };
    }
}