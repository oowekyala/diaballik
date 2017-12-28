using System.Windows.Media;
using Diaballik.Core;
using DiaballikWPF.Converters;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

namespace DiaballikWPF.ViewModel {
    /// <summary>
    ///     Controls a tile, which may carry a piece.
    /// </summary>
    public class TileViewModel : ViewModelBase {
        #region Properties

        private BoardViewModel BoardVm { get; }
        public Position2D Position { get; }

        private bool _hasPiece;


        public bool HasPiece {
            get => _hasPiece;
            set {
                if (value != HasPiece) {
                    _hasPiece = value;
                    RaisePropertyChanged("HasPiece");
                }
            }
        }

        private bool _hasBall;


        public bool HasBall {
            get => _hasBall;
            set {
                if (value != HasBall) {
                    _hasPiece = value;
                    RaisePropertyChanged("HasBall");
                }
            }
        }

        private Color _color;

        public Color PieceColor {
            get => _color;
            set {
                if (value != PieceColor) {
                    _color = value;
                    RaisePropertyChanged("PieceColor");
                }
            }
        }


        private bool _isMarked;

        /// Marked because of available move
        public bool IsMarked {
            get => _isMarked;
            set {
                if (_isMarked != value) {
                    _isMarked = value;

                    RaisePropertyChanged($"Background");
                }
            }
        }

        private bool _isSelected;


        public bool IsSelected {
            get => _isSelected;
            set {
                if (_isSelected != value) {
                    _isSelected = value;

                    RaisePropertyChanged($"Background");
                }
            }
        }

        public void OnLeftClick() {
        }

        public bool IsEven => Position.X % 2 == Position.Y % 2;

        #endregion

        #region Constructors

        public TileViewModel(BoardViewModel boardVm, Position2D position) {
            BoardVm = boardVm;
            Position = position;
        }

        #endregion

        #region Commands

        private RelayCommand _onTileClicked;

        public RelayCommand OnTileClickedCommand =>
            _onTileClicked ?? (_onTileClicked = new RelayCommand(OnTileClicked));

        #endregion

        #region Methods

        private void OnTileClicked() {
            // TODO
        }

        public void Update(BoardLike board) {
            var player = board.PlayerOn(Position);

            if (player == null) {
                HasPiece = false;
                HasBall = false;
                return;
            }

            HasPiece = true;
            PieceColor = MediaColorToDrawingColorConverter.DrawingToMedia(player.Color);

            if (board.BallCarrierForPlayer(player) == Position) {
                HasBall = true;
            }
        }

        #endregion
    }
}