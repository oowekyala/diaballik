using System;
using System.Diagnostics;
using System.Windows.Media;
using Diaballik.Core;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

namespace DiaballikWPF.ViewModel {
    /// <summary>
    ///     Controls a tile, which may carry a piece.
    /// </summary>
    public class TileViewModel : ViewModelBase {
        #region Events

//        public event EventHandler SelectEvent;

        #endregion

        #region Properties

        public BoardViewModel ParentBoard { get; }

        public Position2D Position { get; }

        #region HasPiece

        private bool _hasPiece;

        public bool HasPiece {
            get => _hasPiece;
            set => Set(ref _hasPiece, value, "HasPiece");
        }

        #endregion

        #region HasBall

        private bool _hasBall;

        public bool HasBall {
            get => _hasBall;
            set => Set(ref _hasBall, value, "HasBall");
        }

        #endregion

        #region PieceColor

        private Color _color;

        public Color PieceColor {
            get => _color;
            set => Set(ref _color, value, propertyName: "PieceColor");
        }

        #endregion

        #region IsMarked

        private bool _isMarked;

        /// Marked because of available move
        public bool IsMarked {
            get => _isMarked;
            set => Set(ref _isMarked, value, propertyName: "IsMarked");
        }

        #endregion

        #region MarkingColor

        private Color _markingColor;

        /// Color with which the tile should be marked
        public Color MarkingColor {
            get => _markingColor;
            set => Set(ref _markingColor, value, "MarkingColor");
        }

        #endregion

        #region IsSelected

        private bool _isSelected;

        public bool IsSelected {
            get => _isSelected;
            set {
                if (_isSelected != value) {
                    _isSelected = value;
                    if (IsSelected) {
                        ;
                        ParentBoard.SelectedTile = this;
                    }

                    RaisePropertyChanged("IsSelected");
                }
            }
        }

        #endregion

        #region IsSelectable

        private bool _isSelectable;

        public bool IsSelectable {
            get => _isSelectable;
            set => Set(ref _isSelectable, value, "IsSelectable");
        }

        #endregion

        #endregion

        #region Constructors

        public TileViewModel(BoardViewModel parentBoard, Position2D position) {
            ParentBoard = parentBoard;
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
            PieceColor = player.Color;

            if (board.BallCarrierForPlayer(player) == Position) {
                HasBall = true;
            }
        }

        #endregion

//        protected virtual void OnSelectEvent() {
//            SelectEvent?.Invoke(this, new EventArgs());
//        }
    }
}