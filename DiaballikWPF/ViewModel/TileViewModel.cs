using Diaballik.Core;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

namespace DiaballikWPF.ViewModel {
    /// <summary>
    ///     Controls a tile, which may carry a piece.
    /// </summary>
    public class TileViewModel : ViewModelBase {

        #region Fields

        private bool _isMarked;
        private bool _isSelected;
        private readonly Game _game;
        private readonly BoardViewModel _boardVM;

        #endregion

        #region Properties

        public Position2D Position { get; }

        public bool HasPiece => !_game.State.IsFree(Position);
        public IPlayer Player => _game.State.PlayerOn(Position);

        /// Marked because of available move
        public bool IsMarked {
            get => _isMarked;
            set {
                if (_isMarked != value)
                {
                    _isMarked = value;

                    RaisePropertyChanged($"Background");
                }
            }
        }

        public bool IsSelected {
            get => _isSelected;
            set {
                if (_isSelected != value)
                {
                    _isSelected = value;

                    RaisePropertyChanged($"Background");
                }
            }
        }

        public void OnLeftClick()
        {
        }

        #endregion

        #region Constructors

        public TileViewModel(Game game, BoardViewModel boardVm, Position2D position) {
            _game = game;
            _boardVM = boardVm;
            Position = position;
        }

        #endregion

        #region Commands

        private RelayCommand _onTileClicked;

        public RelayCommand OnTileClickedCommand =>
            _onTileClicked ?? (_onTileClicked = new RelayCommand(OnTileClicked));

        #endregion

        #region Behaviour

        private void OnTileClicked() {
            // TODO
        }

        #endregion
    }
}