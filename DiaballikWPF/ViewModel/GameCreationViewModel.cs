using System;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using Diaballik.Core;
using Diaballik.Players;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using static Diaballik.Players.GameBuilder;

namespace DiaballikWPF.ViewModel {
    public class GameCreationViewModel : ViewModelBase {
        #region Constructors

        public GameCreationViewModel(GameBuilder builder) {
            Builder = builder;
            PlayerBuilder1 = new PlayerBuilderViewModel(Builder.PlayerBuilder1, Color.RoyalBlue);
            PlayerBuilder2 = new PlayerBuilderViewModel(Builder.PlayerBuilder2, Color.DarkRed);
        }

        #endregion


        #region Properties

        public GameBuilder Builder { get; }

        public PlayerBuilderViewModel PlayerBuilder1 { get; }
        public PlayerBuilderViewModel PlayerBuilder2 { get; }

        public int Size {
            get => Builder.BoardSize;
            set {
                if (value != Size) {
                    Builder.BoardSize = value;
                    RaisePropertyChanged("Size");
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
                }
            }
        }

        #endregion

        #region Commands

        private RelayCommand _startGameCommand;

        public RelayCommand StartGameCommand => _startGameCommand ?? (_startGameCommand = new RelayCommand(StartGame));

        #endregion

        #region Methods

        public void StartGame() {
            var game = Builder.Build();
            Console.WriteLine(game.Memento.ToState().FullDescription());
        }

        #endregion
    }
}