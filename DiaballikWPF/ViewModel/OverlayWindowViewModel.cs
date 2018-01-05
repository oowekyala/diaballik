using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media.Animation;
using DiaballikWPF.View;

namespace DiaballikWPF.ViewModel {
    /// <summary>
    ///     Project intended to replace the dockwindow with something prettier.
    ///     WIP with low priority.
    /// </summary>
    public class OverlayWindowViewModel : AbstractMainWindowViewModel {
        protected override void SwitchToGameCreationScreen() {
            _helper.ActiveScreen = _creationScreen;
        }

        protected override void SwitchToGameScreen() {
            _helper.ActiveScreen = _gameScreen;
        }

        protected override void SwitchToGameLoadScreen() {
            _helper.ActiveScreen = _loadScreen;
        }

        protected override void SwitchToStartupScreen() {
            _helper.ActiveScreen = _startupScreen;
        }

        protected override bool IsOnGameScreen() {
            return _helper.ActiveScreen == _gameScreen;
        }

        public new ScreenOverlayWindow View { get; }

        private readonly DiaporamaHelper _helper;


        public double WindowWidth {
            get => _windowWidth;
            set => Set(ref _windowWidth, value);
        }

        #region Screen visibilities

        private Visibility _creationScreenVisibility = Visibility.Collapsed;
        private Visibility _loadScreenVisibility = Visibility.Collapsed;
        private Visibility _startupScreenVisibility = Visibility.Visible;


        public Visibility StartupScreenVisibility {
            get => _startupScreenVisibility;
            set => Set(ref _startupScreenVisibility, value);
        }

        public Visibility LoadScreenVisibility {
            get => _loadScreenVisibility;
            set => Set(ref _loadScreenVisibility, value);
        }

        public Visibility CreationScreenVisibility {
            get => _creationScreenVisibility;
            set => Set(ref _creationScreenVisibility, value);
        }

        #endregion

        private readonly DiaporamaHelper.Screen _gameScreen;
        private readonly DiaporamaHelper.Screen _startupScreen;
        private readonly DiaporamaHelper.Screen _loadScreen;
        private readonly DiaporamaHelper.Screen _creationScreen;
        private double _windowWidth;

        public OverlayWindowViewModel(ScreenOverlayWindow view) : base(view) {
            View = view;

            _gameScreen =
                new DiaporamaHelper.Screen(View, "", "", v => { }); // it's the bottom, should never be slid out
            _startupScreen = new DiaporamaHelper.Screen(View, "mainScreenSlideIn", "mainScreenSlideOut",
                                                        v => StartupScreenVisibility = v);
            _loadScreen = new DiaporamaHelper.Screen(View, "loadScreenSlideIn", "loadScreenSlideOut",
                                                     v => LoadScreenVisibility = v);
            _creationScreen = new DiaporamaHelper.Screen(View, "gameCreationSlideIn", "gameCreationSlideOut",
                                                         v => CreationScreenVisibility = v);

            _helper = new DiaporamaHelper(new List<DiaporamaHelper.Screen> {
                _gameScreen,
                _startupScreen,
                _loadScreen,
                _creationScreen
            });
        }

        private void BeginWindowStoryBoard(string id) {
            var storyboard = View.FindResource(id) as Storyboard;
            storyboard.Begin();
        }

        private class DiaporamaHelper {
            public class Screen {
                private readonly Window _view;
                private readonly string _slideInTransitionId;
                private readonly string _slideOutTransitionId;
                private readonly Action<Visibility> _visibilitySetter;

                public Screen(Window view, string slideInTransitionId, string slideOutTransitionId,
                              Action<Visibility> visibilitySetter) {
                    _view = view;
                    _slideInTransitionId = slideInTransitionId;
                    _slideOutTransitionId = slideOutTransitionId;
                    _visibilitySetter = visibilitySetter;
                }

                public void SlideIn() {
                    var storyboard = (Storyboard) _view.FindResource(_slideInTransitionId);
                    _visibilitySetter(Visibility.Visible);
                    storyboard.Begin();
                }


                public void SlideOut() {
                    var storyboard = (Storyboard) _view.FindResource(_slideOutTransitionId);
                    storyboard.Completed += (s, a) => _visibilitySetter(Visibility.Collapsed);
                    storyboard.Begin();
                }
            }

            private readonly List<Screen> _screens;
            private Screen _activeScreen;
            private int _activeScreenIndex;

            public Screen ActiveScreen {
                get => _activeScreen;
                set {
                    if (_activeScreen != value) {
                        var idx = _screens.IndexOf(value);
                        MakeTransition(_activeScreenIndex, idx);
                        _activeScreenIndex = idx;
                        _activeScreen = value;
                    }
                }
            }

            private void MakeTransition(int curIndex, int newIndex) {
                if (curIndex < newIndex) {
                    for (int i = curIndex + 1; i <= newIndex; i++) {
                        _screens[i].SlideIn();
                    }
                } else {
                    for (int i = curIndex; i > newIndex; i--) {
                        _screens[i].SlideOut();
                    }
                }
            }


            /// <param name="screens">Ordered collection of screens, leftmost is the deepest</param>
            public DiaporamaHelper(IEnumerable<Screen> screens) {
                _screens = screens.ToList(); // copy
            }
        }
    }
}