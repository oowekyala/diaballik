using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Resources;
using Diaballik.AlgoLib;
using Diaballik.Core;
using Diaballik.Core.Util;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using static DiaballikWPF.ViewModel.GameScreenViewModel;
using static DiaballikWPF.ViewModel.Messages;

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
        public BoardViewModel(IMessenger messenger, Game game) {
            MessengerInstance = messenger;
            SetSelectedTileMessage.Register(MessengerInstance, this, tile => SelectedTile = tile);
            Reset(game);
        }

        public void Reset(Game game) {
            if (game.BoardSize == BoardSize) {
                ResetDifferential(game);
                return;
            }

            Tiles.Clear();
            CurrentState = game.State;
            BoardSize = game.BoardSize;
            var range = Enumerable.Range(0, BoardSize).ToList();
            var tiles
                = range.SelectMany(x => range.Select(y => new TileViewModel(MessengerInstance, Position2D.New(x, y))))
                       .Cast<ITilePresenter>();

            foreach (var tile in tiles) {
                tile.Update(game.State.PlayerOn(tile.Position), game.State.HasBall(tile.Position));
                Tiles.Add(tile);
            }
        }

        private void ResetDifferential(Game game) {
            DiaballikUtil.Assert(game.BoardSize == BoardSize, "Cannot reset differentially when changing board size");


            var modifiedPs = new HashSet<Position2D>();
            CurrentState.PositionsPair.Foreach(ps => modifiedPs.UnionWith(ps));
            game.State.PositionsPair.Foreach(ps => modifiedPs.UnionWith(ps));
            foreach (var p in modifiedPs) {
                UpdateTile(p, game.State);
            }
            CurrentState = game.State;
        }

        #endregion

        #region Properties

        private int _boardSize;

        public int BoardSize {
            get => _boardSize;
            private set => Set(ref _boardSize, value);
        }


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
                Debug.WriteLine($"unlocked player {value} ({UnlockedPlayer})");
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
                if (_selectedTile != value) {
                    if (value != null) {
                        SuggestMoves(value);
                    } else {
                        SelectedTile.IsSelected = false;
                        _markedTiles.ForEach(t => t.UnMark());
                        _markedTiles.Clear();
                    }
                    _selectedTile = value;
                    RaisePropertyChanged();
                }
            }
        }

        #endregion

        #endregion

        #region Public interface

        /// <summary>
        ///     Update the visual state of the game. This also updates the
        ///     selectable tiles if need be, and resets piece selection.
        /// </summary>
        /// <param name="state">The new state to display</param>
        /// <param name="action">The transition from the previous state to the current one</param>
        public void UpdateState(GameState state, MoveAction action) {
            CurrentState = state;

            UpdateTile(action.Src, state);
            UpdateTile(action.Dst, state);
        }

        #endregion

        #region Private members

        public ITilePresenter TileAt(Position2D p) {
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
                target.MarkMove(CurrentState.CurrentPlayer, move);
                _markedTiles.Add(target);
            }
        }


        private void UpdateTile(Position2D p, BoardLike state) {
            TileAt(p).Update(state.PlayerOn(p), state.HasBall(p));
        }

        #endregion
    }
}