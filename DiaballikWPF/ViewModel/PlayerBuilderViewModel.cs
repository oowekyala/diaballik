using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows.Media;
using System.Linq;
using Diaballik.Players;
using GalaSoft.MvvmLight;
using static Diaballik.Players.PlayerBuilder;

namespace DiaballikWPF.ViewModel {
    public class PlayerBuilderViewModel : ViewModelBase {
        #region Constructors

        public PlayerBuilderViewModel(PlayerBuilder builder, Color baseColor,
            OnValidationChangedDelegate onValidationChanged) {
            Builder = builder;
            Color = baseColor;
            OnValidationChanged += onValidationChanged;
            // OnValidationChanged += () => Debug.WriteLine("Validation changed");
        }

        #endregion

        #region Properties

        public delegate void OnValidationChangedDelegate();

        public OnValidationChangedDelegate OnValidationChanged { get; } = () => { };

        private PlayerBuilder Builder { get; }

        public string Name {
            get => Builder.Name;
            set {
                if (value != Name) {
                    Builder.Name = value;
                    RaisePropertyChanged("Name");
                    OnValidationChanged();
                }
            }
        }

        public Color Color {
            get => Builder.Color;
            set {
                if (value != Color) {
                    Builder.Color = value;
                    RaisePropertyChanged("Color");
                    OnValidationChanged();
                }
            }
        }

        public ObservableCollection<PlayerType> PlayerTypes { get; }
            = new ObservableCollection<PlayerType>(Enum.GetValues(typeof(PlayerType)).Cast<PlayerType>());


        public PlayerType SelectedPlayerType {
            get => Builder.SelectedPlayerType;
            set {
                if (value != SelectedPlayerType) {
                    Builder.SelectedPlayerType = value;
                    RaisePropertyChanged("SelectedPlayerType");
                    OnValidationChanged();
                }
            }
        }

        #endregion
    }
}