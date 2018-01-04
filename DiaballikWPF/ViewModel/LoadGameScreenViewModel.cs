using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Diaballik.Core;
using Diaballik.Core.Util;
using Diaballik.Mock;
using DiaballikWPF.Util;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using static DiaballikWPF.Util.Messages;

namespace DiaballikWPF.ViewModel {
    using GameId = String;


    public class LoadGameScreenViewModel : ViewModelBase {
        public LoadGameScreenViewModel(IMessenger messenger) {
            MessengerInstance = messenger;

            DeleteSaveInViewMessage.Register(MessengerInstance, this, item => { Saves.Remove(item); });

            // disconnect the messenger of this one from the messenger of the game screen
            BoardViewModel = new BoardViewModel(new Messenger(), MockUtil.AnyGame(7, 0).State) {
                UnlockedPlayer = null
            };

            BackToMainMenuCommand = new RelayCommand(() => ShowMainMenuMessage.Send(MessengerInstance));
        }

        #region Properties

        public ObservableCollection<SaveItemViewModel> Saves { get; } = new ObservableCollection<SaveItemViewModel>();
        public BoardViewModel BoardViewModel { get; }

        private SaveItemViewModel _selectedItem;

        public SaveItemViewModel SelectedSaveItem {
            get => _selectedItem;
            set {
                if (value != SelectedSaveItem) {
                    if (value != null) {
                        BoardViewModel.Reset(value.Entry.Metadata.LatestState);
                    }

                    Set(ref _selectedItem, value);
                }
            }
        }

        public RelayCommand BackToMainMenuCommand { get; }

        #endregion

        public void Refresh(IEnumerable<SaveManager.SaveEntry> up2Date) {
            Saves.Clear();
            foreach (var entry in up2Date) {
                Saves.Add(new SaveItemViewModel(MessengerInstance, entry));
            }
        }
    }


    /// <summary>
    ///     Represents a saved game.
    /// 
    ///     <see cref="SaveManager.SaveEntry"/>
    /// </summary>
    public class SaveItemViewModel : ViewModelBase {
        // the messenger of that one doesn't reach the full application, only the specific boardview
        public SaveItemViewModel(IMessenger messenger, SaveManager.SaveEntry entry) {
            MessengerInstance = messenger;

            Entry = entry;

            Player1Tag = new PlayerTagViewModel(entry.Metadata.Player1);
            Player2Tag = new PlayerTagViewModel(entry.Metadata.Player2);

            if (IsVictory) {
                (Player1Tag, Player2Tag).ToLinq()
                                        .First(tag => tag.Player == Entry.Metadata.VictoriousPlayer)
                                        .IsVictorious = true;
            }


            // commands

            DeleteSaveCommand = new RelayCommand(() => {
                DeleteSaveInViewMessage.Send(MessengerInstance, this);
                DeleteSaveInManagerMessage.Send(MessengerInstance, Id);
            });

            ReplayModeCommand =
                new RelayCommand(
                    () => OpenGameMessage.Send(MessengerInstance, ((Id, Entry.Memento), ViewMode.Replay)));

            ResumeCommand = new RelayCommand(
                () => OpenGameMessage.Send(MessengerInstance, ((Id, Entry.Memento), ViewMode.Play)),
                () => !IsVictory);
        }

        public SaveManager.SaveEntry Entry { get; }

        public GameId Id => Entry.Metadata.Id;

        public DateTime SaveDate => Entry.Metadata.SaveDate;

        public bool IsVictory => Entry.Metadata.IsVictory;

        public PlayerTagViewModel Player1Tag { get; }

        public PlayerTagViewModel Player2Tag { get; }


        #region Commands

        public RelayCommand DeleteSaveCommand { get; }

        public RelayCommand ReplayModeCommand { get; }

        public RelayCommand ResumeCommand { get; }

        #endregion
    }
}