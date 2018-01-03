using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;
using Diaballik.Core;
using Diaballik.Mock;
using DiaballikWPF.View;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using static DiaballikWPF.ViewModel.MessengerChannels;

namespace DiaballikWPF.ViewModel {
    public class OverlayWindowViewModel : ViewModelBase {
        private readonly ScreenOverlayWindow _view;


        private const string GameCreationSlideInStoryBoard = "gameCreationSlideIn";
        private const string GameCreationSlideOutStoryBoard = "gameCreationSlideOut";
        private const string MainMenuSlideInStoryBoard = "mainScreenSlideIn";
        private const string MainMenuSlideOutStoryBoard = "mainScreenSlideOut";


        public OverlayWindowViewModel(ScreenOverlayWindow view) {
            _view = view;
            var dummyGame = Game.Init(7, MockUtil.DummyPlayerSpecPair(7));
            GameScreenViewModel = new GameScreenViewModel(MessengerInstance, dummyGame);
            GameCreationScreenViewModel = new GameCreationScreenViewModel(MessengerInstance);
            StartupScreenViewModel = new StartupScreenViewModel(MessengerInstance);

            RegisterScreenSwitchHandlers();
        }

        private void BeginWindowStoryBoard(string id) {
            var storyboard = Application.Current.MainWindow.FindResource(id) as Storyboard;
            storyboard.Begin();
        }


        private void RegisterScreenSwitchHandlers() {
            MessengerInstance.Register<NotificationMessage>(
                recipient: this,
                token: ShowGameCreationScreenMessageToken,
                action: message => { BeginWindowStoryBoard(GameCreationSlideInStoryBoard); });

            MessengerInstance.Register<NotificationMessage<(Game, ViewMode)>>(
                recipient: this,
                token: ShowGameScreenMessageToken,
                action: message => {
                    var (game, mode) = message.Content;
                    GameScreenViewModel.Reset(game);
                    GameScreenViewModel.ActiveMode = mode;

                    BeginWindowStoryBoard(GameCreationSlideOutStoryBoard);
                    BeginWindowStoryBoard(MainMenuSlideOutStoryBoard);
                });
        }

        public StartupScreenViewModel StartupScreenViewModel { get; }
        public GameScreenViewModel GameScreenViewModel { get; }
        public GameCreationScreenViewModel GameCreationScreenViewModel { get; }

        private Screen _activeScreen;

        public Screen ActiveScreen {
            get => _activeScreen;
            set { Set(ref _activeScreen, value); }
        }


        public enum Screen {
            MainMenu,
            GameScreen,
            LoadMenu,
            CreationMenu
        }


        private void OpenGame(Game game, ViewMode mode) {
        }
    }
}