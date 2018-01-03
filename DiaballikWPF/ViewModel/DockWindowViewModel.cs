using System;
using System.Diagnostics;
using System.Windows.Controls.Primitives;
using Diaballik.Core;
using Diaballik.Mock;
using DiaballikWPF.Util;
using DiaballikWPF.View;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using static DiaballikWPF.ViewModel.Messages;

namespace DiaballikWPF.ViewModel {
    using GameId = String;
    using DecoratedMemento = ValueTuple<GameMetadataBundle, GameMemento>;


    /// <summary>
    ///     Main window of the app, in which the various screens dock.
    /// </summary>
    public class DockWindowViewModel : ViewModelBase {
        #region Constructors

        public DockWindowViewModel(DockWindow view) {
            View = view;
            var dummyGame = Game.Init(7, MockUtil.DummyPlayerSpecPair(7));
            GameScreenViewModel = new GameScreenViewModel(MessengerInstance, dummyGame);
            GameCreationScreenViewModel = new GameCreationScreenViewModel(MessengerInstance);
            StartupScreenViewModel = new StartupScreenViewModel(MessengerInstance);
            LoadGameScreenViewModel = new LoadGameScreenViewModel(MessengerInstance);

            RegisterScreenSwitchHandlers();
        }


        private void RegisterScreenSwitchHandlers() {
            ShowNewGameMessage.Register(MessengerInstance, this, () => ContentViewModel = GameCreationScreenViewModel);
            ShowMainMenuMessage.Register(MessengerInstance, this, () => ContentViewModel = StartupScreenViewModel);
            ShowLoadMenuMessage.Register(MessengerInstance, this, () => ContentViewModel = LoadGameScreenViewModel);

            // switch to game screen with a newly created game
            ShowGameScreenMessage.Register(MessengerInstance, this, payload => {
                var (game, mode) = payload;
                GameScreenViewModel.Load(game);
                GameScreenViewModel.ActiveMode = mode;

                ContentViewModel = GameScreenViewModel;
            });


            // switch to game with game loaded from a save, which already has an id
            LoadGameFromSaveMessage.Register(MessengerInstance, this, payload => {
                var ((metadata, memento), mode) = payload;
                GameScreenViewModel.LoadWithId(metadata.Id, Game.FromMemento(memento));
                GameScreenViewModel.ActiveMode = mode;

                ContentViewModel = GameScreenViewModel;
            });


            ShowVictoryPopupMessage.Register(MessengerInstance, this, HandleVictory);
        }

        public StartupScreenViewModel StartupScreenViewModel { get; }
        public GameScreenViewModel GameScreenViewModel { get; }
        public GameCreationScreenViewModel GameCreationScreenViewModel { get; }
        public LoadGameScreenViewModel LoadGameScreenViewModel { get; }

        #endregion

        #region Properties

        public DockWindow View { get; }

        private ViewModelBase _contentViewModel;

        public ViewModelBase ContentViewModel {
            get => _contentViewModel;
            set => Set(ref _contentViewModel, value);
        }

        #endregion

        private Popup VictoryPopup => _victoryPopup ?? (_victoryPopup = new Popup {
            Child = new VictoryPopup(),
            Height = 70,
            Width = 300,
            PopupAnimation = PopupAnimation.Slide,
            PlacementTarget = View,
            Placement = PlacementMode.Center
        });

        private Popup _victoryPopup;

        public void HandleVictory(Player player) {
            VictoryPopup.DataContext = new VictoryPopupViewModel(MessengerInstance, player);
            VictoryPopup.IsOpen = true;

            CloseVictoryPopupMessage.Register(MessengerInstance, this, () => {
                VictoryPopup.IsOpen = false;
                CloseVictoryPopupMessage.Unregister(MessengerInstance, this);
            });
        }


        private Popup SavePopup => _savePopup ?? (_savePopup = new Popup {
            Child = new VictoryPopup(),
            Height = 70,
            Width = 300,
            PopupAnimation = PopupAnimation.Slide,
            PlacementTarget = View,
            Placement = PlacementMode.Center
        });

        private Popup _savePopup;

        public void DisplaySavePopup(Player player) {
            // TODODODODODO
            SavePopup.DataContext = new VictoryPopupViewModel(MessengerInstance, player);
            SavePopup.IsOpen = true;

            CloseVictoryPopupMessage.Register(MessengerInstance, this, () => {
                SavePopup.IsOpen = false;
                CloseVictoryPopupMessage.Unregister(MessengerInstance, this);
            });
        }
    }
}