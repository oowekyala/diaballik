﻿using System;
using System.Collections.Generic;
using System.Drawing;

namespace CSDiaballik
{
    /// <summary>
    /// Builds player instances.
    /// </summary>
    public class PlayerBuilder
    {
        private static bool _isFirstPlayer;
        private AiPlayer.AiLevel _aiLevel;
        private List<Position2D> _piecesList;

        public PlayerBuilder()
        {
            _isFirstPlayer = !_isFirstPlayer;
            Color = _isFirstPlayer ? Color.Blue : Color.Red;
        }

        public PlayerBuilder SetIsAi(AiPlayer.AiLevel level)
        {
            _aiLevel = level;
            return this;
        }

        public PlayerBuilder Pieces(List<Position2D> pos)
        {
            _piecesList = pos;
            return this;
        }

        public Color Color { set; get; }

        public string Name { set; get; }

        /// <summary>
        /// Builds a player with the specified configuration.
        /// </summary>
        /// <returns>A new player</returns>
        public IPlayer Build()
        {
            switch (_aiLevel)
            {
                case AiPlayer.AiLevel.Noob:
                    return new NoobAiPlayer(Color, Name, _piecesList);
                case AiPlayer.AiLevel.Starting:
                    return new StartingAiPlayer(Color, Name, _piecesList);
                case AiPlayer.AiLevel.Progressive:
                    return new ProgressiveAiPlayer(Color, Name, _piecesList);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}