using System.Windows;
using System.Windows.Media.Animation;
using Diaballik.Core;
using Diaballik.Mock;
using DiaballikWPF.View;
using GalaSoft.MvvmLight;
using static DiaballikWPF.Util.Messages;

namespace DiaballikWPF.ViewModel {
    /// <summary>
    ///     Project intended to replace the dockwindow with something prettier.
    ///     WIP with low priority.
    /// </summary>
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
            ShowNewGameMessage.Register(MessengerInstance, this,
                                        () => BeginWindowStoryBoard(GameCreationSlideInStoryBoard));

            OpenNewGame.Register(MessengerInstance, this, payload => {
                var (game, mode) = payload;
//                GameScreenViewModel.Load(game); FIXME
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