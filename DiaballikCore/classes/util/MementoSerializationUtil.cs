﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Media;
using System.Linq;
using System.Xml.Linq;
using Diaballik.Core.Builders;

// ReSharper disable PossibleNullReferenceException

namespace Diaballik.Core.Util {
    /// <summary>
    ///    Serialization and deserialization utility for game memento.
    ///    Reader and writer are kept together for encapsulation and better maintainability.
    /// </summary>
    public static class MementoSerializationUtil {
        #region XML name constants

        private const string DocElement = "game";
        private const string MoveHistoryElement = "history";
        private const string MoveElement = "action";
        private const string MoveParamsElement = "params";
        private const string PlayerElement = "player";
        private const string PlayersElement = "players";
        private const string PlayerBoardSpecElement = "playerboardspec";
        private const string BoardSpecElement = "boardspec";
        private const string SpecPositionsElement = "positions";
        private const string PositionElement = "pos";
        private const string RootMementoElement = "initial-state";

        private const string MoveBallToken = "moveball";
        private const string MovePieceToken = "movepiece";
        private const string PassToken = "pass";

        #endregion

        #region Name dictionaries

        // decouples class name from xml representation
        private static readonly Dictionary<Type, string> ActionTypes = new Dictionary<Type, string> {
            {typeof(MoveBallAction), MoveBallToken},
            {typeof(MovePieceAction), MovePieceToken},
            {typeof(PassAction), PassToken},
        };

        private static readonly Dictionary<PlayerType, string> PlayerTypesToString =
            new Dictionary<PlayerType, string> {
                {PlayerType.Human, "human"},
                {PlayerType.NoobAi, "noobAi"},
                {PlayerType.StartingAi, "startingAi"},
                {PlayerType.ProgressiveAi, "progressiveAi"},
            };

        #endregion


        /// <summary>
        ///     Serializes GameMemento into an XML document.
        /// </summary>
        public class Serializer {
            #region Public interface

            public XDocument ToXml(GameMemento memento) {
                var (mementoRoot, nodes) = memento.Deconstruct();

                return new XDocument(new XElement(DocElement,
                                                  RootToElement(mementoRoot),
                                                  new XElement(MoveHistoryElement,
                                                               nodes.ZipWithIndex(NodeToElement))));
            }

            #endregion

            #region Private conversion methods

            private static XElement PlayerSpecToElement(FullPlayerBoardSpec spec, int id) {
                return new XElement(PlayerBoardSpecElement,
                                    PlayerToElement(spec.Player, id),
                                    new XAttribute("idx", id),
                                    new XElement(BoardSpecElement,
                                                 new XAttribute("ballIndex", spec.BallIndex),
                                                 new XElement(SpecPositionsElement,
                                                              spec.Positions.ZipWithIndex(PositionToElement)
                                                 )));
            }

            private static XElement PlayerToElement(Player player, int id) {
                return new XElement(PlayerElement,
                                    new XAttribute("color", new ColorConverter().ConvertToString(player.Color)),
                                    new XAttribute("name", player.Name),
                                    new XAttribute("type", PlayerTypesToString[player.Type])
                );
            }

            private static XElement PositionToElement(Position2D p, int idx) {
                return new XElement(PositionElement,
                                    new XAttribute("x", p.X),
                                    new XAttribute("y", p.Y),
                                    new XAttribute("idx", idx));
            }

            private static XElement RootToElement(RootMemento memento) {
                var specs = memento.Specs.Zip((1, 2), PlayerSpecToElement);
                return new XElement(RootMementoElement,
                                    new XAttribute("boardSize", memento.BoardSize),
                                    new XAttribute("firstPlayer", memento.IsFirstPlayerPlaying),
                                    new XElement(PlayersElement,
                                                 specs.Item1,
                                                 specs.Item2));
            }


            private static XElement NodeToElement(MementoNode node, int idx) {
                var elem = new XElement(MoveElement,
                                        new XAttribute("type", ActionTypes[node.Action.GetType()]),
                                        new XAttribute("idx", idx)
                );

                if (node.Action is MoveAction move) {
                    elem.Add(new XElement(MoveParamsElement,
                                          PositionToElement(move.Src, 0),
                                          PositionToElement(move.Dst, 1)
                             ));
                }
                return elem;
            }

            #endregion
        }

        /// <summary>
        ///     Deserializes a document obtained from a <see cref="Serializer"/>
        ///     into an equivalent GameMemento.
        /// </summary>
        public class Deserializer {
            #region Public interface

            public GameMemento FromDocument(XDocument doc) {
                var root = RootFromElement(doc.Element(DocElement).Element(RootMementoElement));
                return doc.Element(DocElement)
                          .Element(MoveHistoryElement)
                          .Elements(MoveElement)
                          .OrderBy(elt => int.Parse(elt.Attribute("idx").Value))
                          .Aggregate((GameMemento) root, NodeFromElement);
            }

            #endregion

            #region Private conversion methods

            private static MementoNode NodeFromElement(GameMemento previous, XElement element) {
                var type = element.Attribute("type").Value;

                IUpdateAction action;
                switch (type) {
                    case MoveBallToken: {
                        var (src, dst) = MoveActionParamsFromElement(element.Element(MoveParamsElement));
                        action = MoveBallAction.New(src, dst);
                        break;
                    }
                    case MovePieceToken: {
                        var (src, dst) = MoveActionParamsFromElement(element.Element(MoveParamsElement));
                        action = MovePieceAction.New(src, dst);
                        break;
                    }
                    case PassToken:
                        action = new PassAction();
                        break;
                    default: throw new ArgumentOutOfRangeException();
                }
                return previous.Update(action);
            }

            private static (Position2D, Position2D) MoveActionParamsFromElement(XElement elements) {
                return elements.Elements(PositionElement)
                               .Select(PositionFromElement)
                               .SortAndUnzip()
                               .ToTuple();
            }

            private static RootMemento RootFromElement(XElement rootElem) {
                var isFirstPlayerPlaying = bool.Parse(rootElem.Attribute("firstPlayer").Value);
                var boardSize = int.Parse(rootElem.Attribute("boardSize").Value);
                var boardSpecs = rootElem.Element(PlayersElement)
                                         .Elements(PlayerBoardSpecElement)
                                         .Select(PlayerSpecFromElement)
                                         .SortAndUnzip().ToTuple();
                return new RootMemento(boardSpecs, boardSize, isFirstPlayerPlaying);
            }

            private static (FullPlayerBoardSpec, int) PlayerSpecFromElement(XElement element) {
                var player = PlayerFromElement(element.Element(PlayerElement));

                var ballIndex = int.Parse(element.Element(BoardSpecElement).Attribute("ballIndex").Value);
                var positions = element.Element(BoardSpecElement)
                                       .Element(SpecPositionsElement)
                                       .Elements(PositionElement)
                                       .Select(PositionFromElement)
                                       .SortAndUnzip();
                var idx = int.Parse(element.Attribute("idx").Value);

                return (new FullPlayerBoardSpec(player, positions, ballIndex), idx);
            }

            private static Player PlayerFromElement(XElement element) {
                var name = element.Attribute("name").Value;
                var color =
                    (Color) (new ColorConverter() as TypeConverter).ConvertFromString(element.Attribute("color").Value);
                var type = element.Attribute("type").Value;

                var playerType = PlayerTypesToString.First(x => x.Value == type).Key;

                return new PlayerBuilder().SetColor(color).SetName(name).SetPlayerType(playerType).Build();
            }

            private static (Position2D, int) PositionFromElement(XElement element) {
                var x = int.Parse(element.Attribute("x").Value);
                var y = int.Parse(element.Attribute("y").Value);
                var idx = int.Parse(element.Attribute("idx").Value);
                return (new Position2D(x, y), idx);
            }

            #endregion
        }
    }
}