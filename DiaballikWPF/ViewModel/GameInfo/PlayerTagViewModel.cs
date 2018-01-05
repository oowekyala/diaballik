using Diaballik.Core;
using GalaSoft.MvvmLight;

namespace DiaballikWPF.ViewModel {
    public class PlayerTagViewModel : ViewModelBase {
        private Player _player;

        public Player Player {
            get => _player;
            set => Set(ref _player, value);
        }


        public PlayerTagViewModel(Player player) {
            Player = player;
        }

        public PlayerTagViewModel(Player player, bool isVictorious) : this(player) {
            IsVictorious = isVictorious;
        }

        private bool _isVictorious;

        public bool IsVictorious {
            get => _isVictorious;
            set => Set(ref _isVictorious, value);
        }

        /// True if the tag represents the current player.
        public bool IsActive => NumMovesLeft > 0;

        private int _numMovesLeft;

        public int NumMovesLeft {
            get => _numMovesLeft;
            set {
                Set(ref _numMovesLeft, value);
                RaisePropertyChanged("IsActive");
            }
        }
    }
}