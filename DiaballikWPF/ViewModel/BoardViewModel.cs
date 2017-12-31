using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using Diaballik.AlgoLib;
using Diaballik.Core;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using static DiaballikWPF.ViewModel.PlayGameScreenViewModel;
using static DiaballikWPF.ViewModel.TilePresenterConstants;

namespace DiaballikWPF.ViewModel {
    public interface IBoardPresenter {
        Player UnlockedPlayer { get; set; }

        /// <summary>
        ///     Updates the view to reflect the given state.
        ///     The transition from the previous state is given
        ///     as parameter, to only update those tiles that 
        ///     have changed.
        /// </summary>
        /// <param name="state">New state</param>
        /// <param name="action">Transition to the new state</param>
        void UpdateState(GameState state, MoveAction action);
    }

    /// <summary>
    ///     Presents a board. This control is passive, it doesn't drive the game itself.
    ///     View updates must be requested by the parent view model.
    /// 
    ///     It can let one player play at a time, and fires an event when the player commits
    ///     to a MoveAction.
    /// </summary>
    public class BoardViewModel : ViewModelBase, IBoardPresenter {
        #region Constructor

        /// <summary>
        ///     Builds a board presenter, initially with no unlocked player.
        /// </summary>
        /// <param name="messenger">Messenger for this presenter and its descendants</param>
        /// <param name="state">Initial state</param>
        public BoardViewModel(IMessenger messenger, GameState state) {
            CurrentState = state;
            BoardSize = state.BoardSize;
            MessengerInstance = messenger;

            var range = Enumerable.Range(0, BoardSize).ToList();
            var tiles
                = range.SelectMany(x => range.Select(y => new TileViewModel(MessengerInstance, Position2D.New(x, y))))
                       .Cast<ITilePresenter>();

            // Register message handlers
            MessengerInstance.Register<NotificationMessage<ITilePresenter>>(this,
                                                                            SelectedTileMessageToken,
                                                                            message => SelectedTile = message.Content);

            foreach (var tile in tiles) {
                tile.Update(state.PlayerOn(tile.Position), state.HasBall(tile.Position));
                Tiles.Add(tile);
            }
        }

        #endregion

        #region Properties

        public int BoardSize { get; }


        /// State currently represented by the game
        public GameState CurrentState { get; private set; }


        #region Tiles

        /// Stores the tiles linearly. Use TileAt to retrieve one.
        public ObservableCollection<ITilePresenter> Tiles { get; } = new ObservableCollection<ITilePresenter>();

        #endregion

        #region UnlockedPlayer

        private Player _unlockedPlayer;

        /// Player currently allowed to play. Null if both players are locked.
        public Player UnlockedPlayer {
            get => _unlockedPlayer;
            set {
                if (UnlockedPlayer != value) {
                    if (value != null && UnlockedPlayer != null) {
                        // change player
                        foreach (var tile in CurrentState.PositionsForPlayer(value)) {
                            TileAt(tile).IsSelectable = true;
                        }
                        var opponent = CurrentState.GetOtherPlayer(value);
                        foreach (var tile in CurrentState.PositionsForPlayer(opponent)) {
                            TileAt(tile).IsSelectable = false;
                        }
                    } else if (value == null) {
                        // UnlockedPlayer != null
                        // then we put the board in readonly state, the other player is already locked
                        foreach (var tile in CurrentState.PositionsForPlayer(UnlockedPlayer)) {
                            TileAt(tile).IsSelectable = false;
                        }
                    } else {
                        // value != null, UnlockedPlayer == null
                        // then we unlock the value player, the other is already locked
                        foreach (var tile in CurrentState.PositionsForPlayer(value)) {
                            TileAt(tile).IsSelectable = true;
                        }
                    }
                    SelectedTile = null;
                    Set(ref _unlockedPlayer, value);
                }
            }
        }

        #endregion

        #region SelectedTile

        private ITilePresenter _selectedTile;

        public ITilePresenter SelectedTile {
            get => _selectedTile;
            set {
                Debug.WriteLine($"Selected {value?.Position}");
                if (_selectedTile != value) {
                    _selectedTile = value;
                    if (SelectedTile != null) {
                        SuggestMoves(SelectedTile);
                    } else {
                        _markedTiles.ForEach(t => t.UnMark());
                        _markedTiles.Clear();
                    }

                    RaisePropertyChanged();
                }
            }
        }

        #endregion

        #endregion

        #region Public interface

        /// <summary>
        ///     Update the visual state of the game. This also updates the
        ///     selectable tiles if need be.
        /// </summary>
        /// <param name="state">The new state to display</param>
        /// <param name="action">The transition from the previous state to the current one</param>
        public void UpdateState(GameState state, MoveAction action) {
            CurrentState = state;

            UpdateTile(action.Src, state);
            UpdateTile(action.Dst, state);

            if (action is MovePieceAction) {
                TileAt(action.Src).IsSelectable = false;
                TileAt(action.Dst).IsSelectable = true;
            }
        }

        #endregion

        #region Private members

        private ITilePresenter TileAt(Position2D p) {
            return Tiles[p.X * BoardSize + p.Y];
        }


        // Remembers the last selected tiles
        private readonly List<ITilePresenter> _markedTiles = new List<ITilePresenter>();

        private void SuggestMoves(ITilePresenter sourceTile) {
            _markedTiles.ForEach(t => t.UnMark());
            _markedTiles.Clear();

            var moves = CurrentState.AvailableMoves(sourceTile.Position);

            foreach (var move in moves) {
                var target = TileAt(move.Dst);
                target.MarkMove(UnlockedPlayer, move);
                _markedTiles.Add(target);
            }
        }


        private void UpdateTile(Position2D p, BoardLike state) {
            TileAt(p).Update(state.PlayerOn(p), state.HasBall(p));
        }

        #endregion
    }
}