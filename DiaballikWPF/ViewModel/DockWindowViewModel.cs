using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media.Animation;
using Diaballik.Core;
using Diaballik.Mock;
using DiaballikWPF.View;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using static DiaballikWPF.ViewModel.MessengerChannels;

namespace DiaballikWPF.ViewModel {
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

            RegisterScreenSwitchHandlers();
        }


        private void RegisterScreenSwitchHandlers() {
            // switch to game creation mode
            MessengerInstance.Register<NotificationMessage>(
                recipient: this,
                token: ShowGameCreationScreenMessageToken,
                action: message => ContentViewModel = GameCreationScreenViewModel);

            // switch to main menu
            MessengerInstance.Register<NotificationMessage>(
                recipient: this,
                token: ShowMainMenuMessageToken,
                action: message => ContentViewModel = StartupScreenViewModel);

            // switch to game mode
            MessengerInstance.Register<NotificationMessage<(Game, ViewMode)>>(
                recipient: this,
                token: ShowGameScreenMessageToken,
                action: message => {
                    var (game, mode) = message.Content;
                    GameScreenViewModel.Reset(game);
                    GameScreenViewModel.ActiveMode = mode;

                    ContentViewModel = GameScreenViewModel;
                });

            // show victory popup
            MessengerInstance.Register<NotificationMessage<Player>>(
                recipient: this,
                token: ShowVictoryPopupMessageToken,
                action: message => HandleVictory(message.Content));
        }

        public StartupScreenViewModel StartupScreenViewModel { get; }
        public GameScreenViewModel GameScreenViewModel { get; }
        public GameCreationScreenViewModel GameCreationScreenViewModel { get; }

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

            MessengerInstance.Register<NotificationMessage>(
                this,
                token: CloseVictoryPopupMessageToken,
                action: message => {
                    VictoryPopup.IsOpen = false;
                    MessengerInstance.Unregister<NotificationMessage>(recipient: this,
                                                                      token: CloseVictoryPopupMessageToken);
                });
        }
    }
}