using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using Diaballik.AlgoLib;
using Diaballik.Core;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
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
        void UpdateState(BoardLike state, MoveAction action);
    }

    /// <summary>
    ///     Presents a board. This control is passive, it doesn't drive the game itself.
    ///     View updates must be requested by the parent view model.
    /// 
    ///     It can let one player play at a time, and fires an event when the player commits
    ///     to a MoveAction.
    /// </summary>
    public class BoardViewModel : ViewModelBase, IBoardPresenter {
        #region Fields

        private readonly Dictionary<Player, HashSet<Position2D>> _positionsByPlayer
            = new Dictionary<Player, HashSet<Position2D>>();

        private readonly Player _player1;
        private readonly Player _player2;

        #endregion


        #region Constructor

        /// <summary>
        ///     Builds a board presenter, initially with no unlocked player.
        /// </summary>
        /// <param name="state">Initial state</param>
        public BoardViewModel(BoardLike state) {
            CurrentState = state;
            BoardSize = state.BoardSize;

            var range = Enumerable.Range(0, BoardSize).ToList();
            var tiles = range.SelectMany(x => range.Select(y => new TileViewModel(Position2D.New(x, y))))
                             .Cast<ITilePresenter>();

            _player1 = state.Player1;
            _player2 = state.Player2;

            _positionsByPlayer.Add(state.Player1, new HashSet<Position2D>(state.Player1Positions));
            _positionsByPlayer.Add(state.Player2, new HashSet<Position2D>(state.Player2Positions));

            // Register message handlers
            MessengerInstance.Register<NotificationMessage<ITilePresenter>>(this, token: SelectedTileMessageToken,
                                                       action: message => SelectedTile = message.Content);

            foreach (var tile in tiles) {
                tile.Update(state.PlayerOn(tile.Position), state.HasBall(tile.Position));
                Tiles.Add(tile);
            }
        }

        #endregion

        #region Properties

        public int BoardSize { get; }


        public BoardLike CurrentState { get; private set; }


        #region Tiles

        /// Stores the tiles linearly. Use TileAt to retrieve one.
        public ObservableCollection<ITilePresenter> Tiles { get; } = new ObservableCollection<ITilePresenter>();

        #endregion

        #region MessengerInstance

//        protected new IMessenger MessengerInstance => Messenger.Default;

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
                        foreach (var tile in _positionsByPlayer[value]) {
                            TileAt(tile).IsSelectable = true;
                        }
                        var opponent = value == _player1 ? _player2 : _player1;
                        foreach (var tile in _positionsByPlayer[opponent]) {
                            TileAt(tile).IsSelectable = false;
                        }
                    } else if (value == null) {
                        // then we put the board in readonly state, the other player is already locked
                        foreach (var tile in _positionsByPlayer[UnlockedPlayer]) {
                            TileAt(tile).IsSelectable = false;
                        }
                    } else if (value != null) {
                        // then we unlock the value player, the other is already locked
                        foreach (var tile in _positionsByPlayer[value]) {
                            TileAt(tile).IsSelectable = true;
                        }
                    }
                    SelectedTile = null;
                    Set(ref _unlockedPlayer, value, "UnlockedPlayer");
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
                    }

                    RaisePropertyChanged("SelectedTile");
                }
            }
        }

        #endregion

        #endregion

        #region Methods

        public ITilePresenter TileAt(Position2D p) {
            Debug.WriteLine($"{p} : {Tiles[p.X * BoardSize + p.Y].Position}");
            return Tiles[p.X * BoardSize + p.Y];
        }

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

        public void UpdateState(BoardLike state, MoveAction action) {
            CurrentState = state;
            UpdateTile(action.Src, state);
            UpdateTile(action.Dst, state);
        }


        private void UpdateTile(Position2D p, BoardLike state) {
            TileAt(p).Update(state.PlayerOn(p), state.HasBall(p));
        }

        #endregion
    }
}