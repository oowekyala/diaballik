using System.Collections.Generic;
using System.Linq;
using Diaballik.Core;
using GalaSoft.MvvmLight;

namespace DiaballikWPF.ViewModel {
    public class BoardViewModel : ViewModelBase {
        #region Private attributes

        private readonly Game _game;

        #endregion

        #region Properties

        public IList<TileViewModel> Tiles { get; }

        private TileViewModel SelectedTile { get; set; }

        #endregion

        #region Constructor

        public BoardViewModel(Game game) {
            _game = game;

            var ps = from x in Enumerable.Range(0, game.State.BoardSize)
                from y in Enumerable.Range(0, game.State.BoardSize)
                select new Position2D(x, y);

            Tiles = ps.Select(p => new TileViewModel(game, this, p)).ToList();
        }

        #endregion
    }
}