using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using Diaballik.Core;
using GalaSoft.MvvmLight;

namespace DiaballikWPF.ViewModel {
    public class BoardViewModel : ViewModelBase {
        #region Constructor

        public BoardViewModel(Game game) {
            Game = game;

            var range = Enumerable.Range(0, BoardSize).ToList();
            var tiles = range.Select(x => range.Select(y => new TileViewModel(this, Position2D.New(x, y))))
                             .SelectMany(r => r);

            foreach (var tile in tiles) {
                tile.Update(Game.State);
                Tiles.Add(tile);
            }
        }

        #endregion

        #region Properties

        public Game Game { get; }

        /// Stores the tiles linearly. Use TileAt to retrieve one.
        public ObservableCollection<TileViewModel> Tiles { get; } = new ObservableCollection<TileViewModel>();


        private TileViewModel _selectedTile;

        public TileViewModel SelectedTile {
            get => _selectedTile;
            set {
                if (_selectedTile != value) {
                    _selectedTile = value;
                    RaisePropertyChanged("SelectedTile");
                }
            }
        }


        public GameMemento Memento => Game.Memento;
        public int BoardSize => Game.State.BoardSize;

        #endregion

        #region Methods

        public TileViewModel TileAt(Position2D p) {
            return Tiles[p.X * BoardSize + p.Y];
        }

        // Updates the underlying game and the tiles.
        private void Update(IUpdateAction action) {
            Game.Update(action);
            if (action is MoveAction m) {
                TileAt(m.Dst).Update(Game.State);
                TileAt(m.Src).Update(Game.State);
            }
        }

        #endregion
    }
}