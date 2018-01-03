using System;
using System.Collections.ObjectModel;
using Diaballik.Core;
using DiaballikWPF.Util;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using static DiaballikWPF.ViewModel.Messages;

namespace DiaballikWPF.ViewModel {
    using GameId = String;
    using DecoratedMemento = ValueTuple<GameMetadataBundle, GameMemento>;

    public class LoadGameScreenViewModel : ViewModelBase {
        public LoadGameScreenViewModel(IMessenger messenger) {
            MessengerInstance = messenger;
        }


        public ObservableCollection<SaveItemViewModel> Saves { get; }


        public void Refresh() {
            Saves.Clear();
            foreach (var decoratedMemento in SaveManager.Instance.AllSaves()) {
                Saves.Add(new SaveItemViewModel(MessengerInstance, decoratedMemento));
            }
        }
    }

    public class SaveItemViewModel : ViewModelBase {
        public SaveItemViewModel(IMessenger messenger, DecoratedMemento decoratedMemento) {
            MessengerInstance = messenger;

            DecoratedMemento = decoratedMemento;

            var (root, _) = decoratedMemento.Item2.Deconstruct();
            Player1Tag = new PlayerTagViewModel(root.Player1);
            Player2Tag = new PlayerTagViewModel(root.Player2);
        }

        private DecoratedMemento DecoratedMemento { get; }

        public GameId Id => DecoratedMemento.Item1.Id;

        public DateTime SaveDate => DecoratedMemento.Item1.SaveDate;


        public PlayerTagViewModel Player1Tag { get; }

        public PlayerTagViewModel Player2Tag { get; }


        #region Delete

        private RelayCommand _deleteSaveCommand;

        public RelayCommand DeleteSaveCommand =>
            _deleteSaveCommand ?? (_deleteSaveCommand =
                new RelayCommand(DeleteSaveCommandExecute, DeleteSaveCommandCanExecute));

        private bool DeleteSaveCommandCanExecute() => true;

        private void DeleteSaveCommandExecute() {
            DeleteSaveMessage.Send(MessengerInstance, this);
        }

        #endregion


        #region Replay

        private RelayCommand _replayModeCommand;

        public RelayCommand ReplayModeCommand =>
            _replayModeCommand ?? (_replayModeCommand =
                new RelayCommand(ReplayModeCommandExecute, ReplayModeCommandCanExecute));

        private bool ReplayModeCommandCanExecute() => true;

        private void ReplayModeCommandExecute() {
            LoadGameFromSaveMessage.Send(MessengerInstance, (DecoratedMemento, ViewMode.Replay));
        }

        #endregion


        #region Resume

        private RelayCommand _resumeCommand;

        public RelayCommand ResumeCommand =>
            _resumeCommand ?? (_resumeCommand = new RelayCommand(ResumeCommandExecute, ResumeCommandCanExecute));

        private bool ResumeCommandCanExecute() => true;

        private void ResumeCommandExecute() {
            LoadGameFromSaveMessage.Send(MessengerInstance, (DecoratedMemento, ViewMode.Play));
        }

        #endregion
    }
}