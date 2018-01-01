using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Media;
using Diaballik.Core;
using Diaballik.Core.Builders;
using Diaballik.Core.Util;
using DiaballikWPF.View;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using static DiaballikWPF.ViewModel.PlayerBuilderViewModel;

namespace DiaballikWPF.ViewModel {
    public class GameCreationScreenViewModel : ViewModelBase {
        #region Fields

        private readonly DockWindowViewModel _dock;

        #endregion

        #region Constructors

        public GameCreationScreenViewModel(DockWindowViewModel dock) {
            _dock = dock;
            Builder = new GameBuilder();
            OnValidationChanged += StartGameCommand.RaiseCanExecuteChanged;

            PlayerBuilder1 = new PlayerBuilderViewModel(Builder.PlayerBuilder1, Colors.RoyalBlue, OnValidationChanged);
            PlayerBuilder2 = new PlayerBuilderViewModel(Builder.PlayerBuilder2, Colors.DarkRed, OnValidationChanged);
        }

        #endregion


        #region Properties

        private OnValidationChangedDelegate OnValidationChanged { get; } = () => { };


        public GameBuilder Builder { get; }

        public PlayerBuilderViewModel PlayerBuilder1 { get; }
        public PlayerBuilderViewModel PlayerBuilder2 { get; }


        public string ErrorMessage => Builder.ErrorMessage;

        public int Size {
            get => Builder.BoardSize;
            set {
                if (value != Size) {
                    Builder.BoardSize = value;
                    RaisePropertyChanged("Size");
                    OnValidationChanged();
                }
            }
        }


        public ObservableCollection<GameBuilder.GameScenario> GameScenarios { get; }
            = new ObservableCollection<GameBuilder.GameScenario>(
                Enum.GetValues(typeof(GameBuilder.GameScenario)).Cast<GameBuilder.GameScenario>());

        public GameBuilder.GameScenario Scenario {
            get => Builder.Scenario;
            set {
                if (value != Scenario) {
                    Builder.Scenario = value;
                    RaisePropertyChanged("Scenario");
                    OnValidationChanged();
                }
            }
        }

        #endregion

        #region Commands

        private RelayCommand _startGameCommand;

        public RelayCommand StartGameCommand {
            get => _startGameCommand ?? (_startGameCommand = new RelayCommand(StartGame, CanStart));
            set => _startGameCommand = value;
        }

        #endregion

        #region Methods

        private static readonly List<string> _defaultPlayerNames = new List<string> {
            "Didier",
            "Jacques",
            "Foobar",
            "Gorgonzola"
        };

        private static string GetDefaultPlayerName(Random rng) {
            var name = _defaultPlayerNames[rng.Next(_defaultPlayerNames.Count)];
            _defaultPlayerNames.Remove(name);
            return name;
        }


        public bool CanStart() => Builder.CanBuild;

        public void StartGame() {
            var rng = new Random();
            (Builder.PlayerBuilder1, Builder.PlayerBuilder2).Foreach(b => {
                if (string.IsNullOrWhiteSpace(b.Name)) b.Name = GetDefaultPlayerName(rng);
            });

            var playGameVm = new GameScreenViewModel(Builder.Build(), ViewMode.Play);
            var screen = new PlayGameScreen {
                DataContext = playGameVm
            };

            _dock.ContentViewModel = playGameVm;
            playGameVm.StartGameLoop();
        }

        #endregion
    }
}