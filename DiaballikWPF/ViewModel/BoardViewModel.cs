using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using Diaballik.AlgoLib;
using Diaballik.Core;
using GalaSoft.MvvmLight;

namespace DiaballikWPF.ViewModel {
    public class BoardViewModel : ViewModelBase {
        #region Constructor

        public BoardViewModel(Game game) {
            Game = game;
            UnlockedPlayer = Game.State.CurrentPlayer; // FIXME


            var range = Enumerable.Range(0, BoardSize).ToList();
            var tiles = range.Select(x => range.Select(y => new TileViewModel(this, Position2D.New(x, y))))
                             .SelectMany(r => r);

            foreach (var tile in tiles) {
                tile.Update(Game.State);
                tile.IsSelectable = tile.PieceColor == Game.State.CurrentPlayer.Color;
                Tiles.Add(tile);
            }
        }

        #endregion

        #region Properties

        public Game Game { get; }
        public GameMemento Memento => Game.Memento;
        public int BoardSize => Game.State.BoardSize;

        /// Stores the tiles linearly. Use TileAt to retrieve one.
        public ObservableCollection<TileViewModel> Tiles { get; } = new ObservableCollection<TileViewModel>();


        #region UnlockedPlayer

        private Player _unlockedPlayer;

        /// Player currently allowed to play. Null if both players are locked.
        public Player UnlockedPlayer {
            get => _unlockedPlayer;
            set => Set(ref _unlockedPlayer, value, "UnlockedPlayer");
        }
        

        #endregion

        #region SelectedTile

        private TileViewModel _selectedTile;

        public TileViewModel SelectedTile {
            get => _selectedTile;
            set {
                if (_selectedTile != value) {
                    _selectedTile = value;
                    Debug.WriteLine($"{_selectedTile.Position} isSelected");
                    if (SelectedTile != null) {
                        SuggestMoves(SelectedTile);
                    }

                    RaisePropertyChanged("SelectedTile");
                }
            }
        }

        #endregion

        #endregion

        #region Methods

        public TileViewModel TileAt(Position2D p) {
            return Tiles[p.X * BoardSize + p.Y];
        }

        private readonly IList<TileViewModel> _markedTiles = new List<TileViewModel>();

        public void SuggestMoves(TileViewModel sourceTile) {
            foreach (var tile in _markedTiles) {
                tile.IsMarked = false;
            }

            _markedTiles.Clear();

            var moves = Game.State.AvailableMoves(sourceTile.Position);

            foreach (var move in moves) {
                var target = TileAt(move.Dst);
                target.IsMarked = true;
                _markedTiles.Add(target);
            }
        }

        // Updates the underlying game and the tiles.
        private void Update(IUpdateAction action) {
            Game.Update(action);
            if (action is MoveAction m) {
                TileAt(m.Dst).Update(Game.State);
                TileAt(m.Src).Update(Game.State);
            }

            // player has changed
            if (Game.State.CurrentPlayer != Game.Memento.Parent.ToState().CurrentPlayer) {
                foreach (var tile in Tiles) {
                    if (tile.HasPiece) {
                        tile.IsSelectable = !tile.IsSelectable;
                    }
                }
            }
        }

        #endregion
    }
}