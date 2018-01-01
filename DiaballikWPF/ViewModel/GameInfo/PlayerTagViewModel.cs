using Diaballik.Core;
using GalaSoft.MvvmLight;

namespace DiaballikWPF.ViewModel {
    public class PlayerTagViewModel : ViewModelBase {

        public Player Player { get; }


        public PlayerTagViewModel(Player player) {
            Player = player;
        }

        private bool _isActive;

        /// True if the tag represents the current player.
        public bool IsActive {
            get => _isActive;
            set => Set(ref _isActive, value);
        }
    }
}