using Diaballik.Core;
using GalaSoft.MvvmLight;

namespace DiaballikWPF.ViewModel {
    public class PlayerTagViewModel : ViewModelBase {
        public Player Player { get; }


        public PlayerTagViewModel(Player player) {
            Player = player;
        }

        private bool _isActive;

        public bool IsActive {
            get => _isActive;
            set => Set(ref _isActive, value);
        }
    }
}