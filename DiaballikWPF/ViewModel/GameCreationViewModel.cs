using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using Diaballik.Players;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using static Diaballik.Players.GameBuilder;
using static DiaballikWPF.ViewModel.PlayerBuilderViewModel;

namespace DiaballikWPF.ViewModel {
    public class GameCreationViewModel : ViewModelBase {
        #region Constructors

        public GameCreationViewModel(GameBuilder builder) {
            Builder = builder;
            OnValidationChanged += StartGameCommand.RaiseCanExecuteChanged;

            PlayerBuilder1 = new PlayerBuilderViewModel(Builder.PlayerBuilder1, Color.RoyalBlue, OnValidationChanged);
            PlayerBuilder2 = new PlayerBuilderViewModel(Builder.PlayerBuilder2, Color.DarkRed, OnValidationChanged);
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


        public ObservableCollection<GameScenario> GameScenarios { get; }
            = new ObservableCollection<GameScenario>(Enum.GetValues(typeof(GameScenario)).Cast<GameScenario>());

        public GameScenario Scenario {
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
            var game = Builder.Build();
            Console.WriteLine(game.Memento.ToState().FullDescription());
        }

        #endregion
    }
}