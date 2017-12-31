using System;
using System.Diagnostics;
using System.Windows.Media;
using Diaballik.Core;
using DiaballikWPF.Converters;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using static DiaballikWPF.ViewModel.TilePresenterConstants;

namespace DiaballikWPF.ViewModel {
    public static class TilePresenterConstants {
        public const string SelectedTileMessageToken = "selectedTile";
        public const string CommittedMoveMessageToken = "committedMove";
    }


    // Model-side interface of the ViewModel
    public interface ITilePresenter {
        /// <summary>
        ///     Position of the tile on the board.
        /// </summary>
        Position2D Position { get; }

        bool IsSelectable { get; set; }

        Tuple<Player, MoveAction> MarkedMove { get; }

        /// <summary>
        ///     Updates the view for this tile.
        /// </summary>
        /// <param name="player">The player owning the piece. Null if the tile is empty</param>
        /// <param name="hasBall">Whether the piece has the ball or not. Ignored if the tile is empty</param>
        void Update(Player player, bool hasBall);

        /// <summary>
        ///     Marks an available move on this tile.
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
                    Set(ref _markedMove, value, propertyName: "MarkedMove");
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
                Set(ref _isSelectable, value, "IsSelectable");
                SelectPieceCommand.RaiseCanExecuteChanged();
            }
        }

        #endregion

        #region IsSelected

        private bool _isSelected;

        public bool IsSelected {
            get => _isSelected;
            set => Set(ref _isSelected, value, "IsSelected");
        }

        #endregion


        #region MessengerInstance

//        protected new IMessenger MessengerInstance => Messenger.Default;

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
            Debug.WriteLine("Send piece selected");
            MessengerInstance.Send(new NotificationMessage<ITilePresenter>(this, "New selected tile"),
                                   token: SelectedTileMessageToken);
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
            Debug.WriteLine("Send move committed");
            MessengerInstance.Send(new NotificationMessage<IUpdateAction>(MarkedMove.Item2,
                                                                          "Move was committed by the player"),
                                   token: CommittedMoveMessageToken);
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