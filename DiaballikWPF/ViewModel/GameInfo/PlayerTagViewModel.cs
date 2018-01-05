﻿using Diaballik.Core;
using GalaSoft.MvvmLight;

namespace DiaballikWPF.ViewModel {
    public class PlayerTagViewModel : ViewModelBase {
        public Player Player { get; }


        public PlayerTagViewModel(Player player) {
            Player = player;
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