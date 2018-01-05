using System;
using System.Windows.Media;
using Diaballik.Core;
using DiaballikWPF.Converters;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using static DiaballikWPF.Util.Messages;

namespace DiaballikWPF.ViewModel {
    // Model-side interface of the ViewModel
    public interface ITilePresenter {
        /// <summary>
        ///     Position of the tile on the board.
        /// </summary>
        Position2D Position { get; }

        bool IsSelectable { get; set; }

        bool IsSelected { get; set; }

        /// <summary>
        ///     Updates the view for this tile.
        /// </summary>
        /// <param name="player">The player owning the piece. Null if the tile is empty</param>
        /// <param name="hasBall">Whether the piece has the ball or not. Ignored if the tile is empty</param>
        void Update(Player player, bool hasBall);

        /// <summary>
        ///     Marks an available move on this tile. After doing so,
        ///     selection of the move will become possible for the 
        ///     unlocked player. That action will send a message
        ///     using <see cref="CommitMoveMessage"/>.
        /// </summary>
        /// <param name="actor">The player who can play the move</param>
        /// <param name="action">The move itself</param>
        void MarkMove(Player actor, MoveAction action);

        /// <summary>
        ///     Unmark the move.
        /// </summary>
        void UnMark();
    }


    /// <summary>
    ///     Controls a tile, which may carry a piece.
    /// </summary>
    public class TileViewModel : ViewModelBase, ITilePresenter {
        #region Properties

        #region Position

        public Position2D Position { get; }

        #endregion

        #region HasBall

        private bool _hasBall;

        public bool HasBall {
            get => _hasBall;
            set => Set(ref _hasBall, value, "HasBall");
        }

        #endregion

        #region HasPiece

        public bool HasPiece => PieceOwner != null;

        #endregion

        #region PieceOwner

        private Player _player;

        public Player PieceOwner {
            get => _player;
            set {
                Set(ref _player, value, "PieceOwner");
                RaisePropertyChanged("HasPiece");
                SelectPieceCommand.RaiseCanExecuteChanged();
            }
        }

        #endregion

        #region IsMarked

        public bool IsMarked => MarkedMove != null;

        #endregion

        #region MarkingColor

        public Color MarkingColor => (MarkedMove?.Item1.Color ?? Colors.Transparent).GetComplement();

        #endregion


        #region MarkedMove

        private Tuple<Player, MoveAction> _markedMove;

        /// Move currently represented by the marking.
        public Tuple<Player, MoveAction> MarkedMove {
            get => _markedMove;
            set {
                if (!Equals(value, MarkedMove)) {
                    Set(ref _markedMove, value);
                    SelectMarkedMoveCommand.RaiseCanExecuteChanged();
                    RaisePropertyChanged("MarkingColor");
                    RaisePropertyChanged("IsMarked");
                }
            }
        }

        #endregion

        #region IsSelectable

        private bool _isSelectable;

        public bool IsSelectable {
            get => _isSelectable;
            set {
                Set(ref _isSelectable, value);
                SelectPieceCommand.RaiseCanExecuteChanged();
            }
        }

        #endregion

        #region IsActive

        private bool _isSelected;

        public bool IsSelected {
            get => _isSelected;
            set => Set(ref _isSelected, value);
        }

        #endregion

        #endregion

        #region Constructors

        public TileViewModel(IMessenger messenger, Position2D position) {
            Position = position;
            MessengerInstance = messenger;
        }

        #endregion

        #region Commands

        #region SelectPieceCommand

        private RelayCommand _selectPiece;

        public RelayCommand SelectPieceCommand =>
            _selectPiece ?? (_selectPiece = new RelayCommand(SelectPieceExecute, SelectPieceCanExecute));

        private bool SelectPieceCanExecute() {
            return PieceOwner != null && IsSelectable;
        }

        private void SelectPieceExecute() {
            SetSelectedTileMessage.Send(MessengerInstance, this);
        }

        #endregion

        #region SelectMarkedMoveCommand

        private RelayCommand _selectMarkedMove;

        public RelayCommand SelectMarkedMoveCommand =>
            _selectMarkedMove ??
            (_selectMarkedMove = new RelayCommand(SelectMarkedMoveExecute, SelectMarkedMoveCanExecute));

        private bool SelectMarkedMoveCanExecute() {
            return MarkedMove != null;
        }

        private void SelectMarkedMoveExecute() {
            CommitMoveMessage.Send(MessengerInstance, MarkedMove.Item2);
        }

        #endregion

        #endregion

        #region Methods

        public void Update(Player player, bool hasBall) {
            PieceOwner = player;
            HasBall = hasBall;
        }

        public void MarkMove(Player actor, MoveAction action) {
            MarkedMove = new Tuple<Player, MoveAction>(actor, action);
        }

        public void UnMark() {
            MarkedMove = null;
        }

        #endregion
    }
}