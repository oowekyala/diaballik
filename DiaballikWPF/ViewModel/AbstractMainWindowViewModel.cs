using System;
using System.Windows;
using System.Windows.Controls.Primitives;
using Diaballik.Core;
using Diaballik.Mock;
using DiaballikWPF.Util;
using DiaballikWPF.View;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using static DiaballikWPF.Util.Messages;

namespace DiaballikWPF.ViewModel {
    /// 
    /// Main window of the app, in which the various screens dock.
    /// Responsible for handling screen transitions.
    /// Mediator for components of the app, helped by the IMessenger.
    /// 
    /// This abstract class is meant to help implement alternative views
    /// with only very limited abstract functionality.
    /// 
    public abstract class AbstractMainWindowViewModel : ViewModelBase {
        #region Constructors

        protected AbstractMainWindowViewModel(Window view) {
            View = view;
            var dummyGame = Game.Init(7, MockUtil.DummyPlayerSpecPair(7));
            GameScreenViewModel = new GameScreenViewModel(MessengerInstance, dummyGame);
            GameCreationScreenViewModel = new GameCreationScreenViewModel(MessengerInstance);
            StartupScreenViewModel = new StartupScreenViewModel(MessengerInstance);
            LoadGameScreenViewModel = new LoadGameScreenViewModel(MessengerInstance);
            SaveManager = new SaveManager(MessengerInstance);

            QuitCommand = new RelayCommand(ShutdownApplication);

            RegisterMessageHandlers();
        }

        protected abstract void SwitchToGameCreationScreen();
        protected abstract void SwitchToGameScreen();
        protected abstract void SwitchToGameLoadScreen();
        protected abstract void SwitchToStartupScreen();

        protected abstract bool IsOnGameScreen();

        protected Window View { get; }


        private void RegisterMessageHandlers() {
            ShowNewGameMessage.Register(MessengerInstance, this, SwitchToGameCreationScreen);
            ShowMainMenuMessage.Register(MessengerInstance, this, SwitchToStartupScreen);
            ShowLoadMenuMessage.Register(MessengerInstance, this, () => {
                SwitchToGameLoadScreen();
                LoadGameScreenViewModel.Refresh(SaveManager.AllSaves());
            });


            void OpenGameAction(((string, GameMemento), ViewMode) payload) {
                var ((id, memento), mode) = payload;
                GameScreenViewModel.Load(id, Game.FromMemento(memento));
                GameScreenViewModel.ActiveMode = mode;

                SwitchToGameScreen();
            }


            // switch to game screen with a newly created game, assigning it an id
            OpenNewGame.Register(MessengerInstance, this, payload => {
                var (game, mode) = payload;
                OpenGameAction(((SaveManager.NextId(), game.Memento), mode));
            });

            OpenGameMessage.Register(MessengerInstance, this, OpenGameAction);

            ShowSavePopupMessage.Register(MessengerInstance, this, payload => {
                var (message, action, hasCancelButton) = payload;
                DisplaySavePopup(message, action, hasCancelButton);
            });

            AppShutdownMessage.Register(MessengerInstance, this, ShutdownApplication);
        }

        public StartupScreenViewModel StartupScreenViewModel { get; }
        public GameScreenViewModel GameScreenViewModel { get; }
        public GameCreationScreenViewModel GameCreationScreenViewModel { get; }
        public LoadGameScreenViewModel LoadGameScreenViewModel { get; }
        private SaveManager SaveManager { get; }

        #endregion

        #region Properties

        #endregion

        public RelayCommand QuitCommand { get; }

        public void ShutdownApplication() {
            if (IsOnGameScreen() && GameScreenViewModel.PrimaryNeedsSaving) {
                const string message = "Would you like to save the current game before exiting?";
                ShowSavePopupMessage.Send(MessengerInstance, (message, () => { }, true));
            }

            SaveManager.CommitSaves();
            Application.Current.Shutdown(0);
        }

        private Popup SavePopup => _savePopup ?? (_savePopup = new Popup {
            Child = new SavePopup(),
            Height = 120,
            Width = 300,
            PopupAnimation = PopupAnimation.Slide,
            PlacementTarget = View,
            Placement = PlacementMode.Center
        });

        private Popup _savePopup;

        private void DisplaySavePopup(string message, Action toConfirm, bool hasCancelButton) {
            SavePopup.DataContext = new SavePopupViewModel(message, toConfirm, hasCancelButton);
            SavePopup.IsOpen = true;

            CloseSavePopupMessage.Register(MessengerInstance, this, () => {
                SavePopup.IsOpen = false;
                CloseSavePopupMessage.Unregister(MessengerInstance, this);
            });
        }
    }
}