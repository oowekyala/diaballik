using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Media;
using Diaballik.Core.Builders;
using Diaballik.Core.Util;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Messaging;
using static DiaballikWPF.ViewModel.PlayerBuilderViewModel;

namespace DiaballikWPF.ViewModel {
    public class GameCreationScreenViewModel : ViewModelBase {
        #region Constructors

        public GameCreationScreenViewModel(IMessenger messenger) {
            MessengerInstance = messenger;
            Builder = new GameBuilder();
            OnValidationChanged += StartGameCommand.RaiseCanExecuteChanged;

            PlayerBuilder1 = new PlayerBuilderViewModel(Builder.PlayerBuilder1, Colors.RoyalBlue, OnValidationChanged);
            PlayerBuilder2 = new PlayerBuilderViewModel(Builder.PlayerBuilder2, Colors.DarkRed, OnValidationChanged);

            var rng = new Random();
            (Builder.PlayerBuilder1, Builder.PlayerBuilder2).Foreach(b => b.Name = GetDefaultPlayerName(rng));
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
            get => _startGameCommand ??
                   (_startGameCommand = new RelayCommand(StartGameCommandExecute, StartGameCommandCanExecute));
            set => _startGameCommand = value;
        }


        public bool StartGameCommandCanExecute() => Builder.CanBuild;

        public void StartGameCommandExecute() {
            Messages.ShowGameScreenMessage.Send(MessengerInstance, (Builder.Build(), ViewMode.Play));
        }

        #endregion

        #region Methods

        private static readonly List<string> DefaultPlayerNames = new List<string> {
            "Didier",
            "Jacques",
            "Foobar",
            "Tatiana"
        };

        private static string GetDefaultPlayerName(Random rng) {
            var name = DefaultPlayerNames[rng.Next(DefaultPlayerNames.Count)];
            DefaultPlayerNames.Remove(name);
            return name;
        }

        #endregion
    }
}