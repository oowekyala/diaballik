using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Media;
using Diaballik.Core;
using Diaballik.Core.Builders;
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

        public bool CanStart() => Builder.CanBuild;

        public void StartGame() {

            var playGameVm = new PlayGameScreenViewModel(Builder);
            var screen = new PlayGameScreen {
                DataContext = playGameVm
            };

            _dock.ContentViewModel = playGameVm;

            playGameVm.StartGameLoop();
        }

        #endregion
    }
}