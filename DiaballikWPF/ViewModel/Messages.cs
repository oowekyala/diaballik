using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Diaballik.Core;
using DiaballikWPF.Util;
using GalaSoft.MvvmLight.Messaging;

namespace DiaballikWPF.ViewModel {
    using GameId = String;
    using DecoratedMemento = ValueTuple<GameMetadataBundle, GameMemento>;

    /// <summary>
    ///     Enumerates messages available for messenger communication.
    /// </summary>
    public static class Messages {
        public static Message ShowMainMenuMessage = new Message("showMainMenu");
        public static Message ShowLoadMenuMessage = new Message("showLoadMenu");
        public static Message<(Game, ViewMode)> ShowGameScreenMessage = new Message<(Game, ViewMode)>("showGameScreen");
        public static Message ShowNewGameMessage = new Message("showNewGame");

        // victory popup

        public static Message<Player> ShowVictoryPopupMessage = new Message<Player>("showVictoryPopup");
        public static Message CloseVictoryPopupMessage = new Message("closeVictoryPopup");

        // loading

        public static Message<(DecoratedMemento, ViewMode)> LoadGameFromSaveMessage =
            new Message<(DecoratedMemento, ViewMode)>("loadGameFromSave");

        public static Message<SaveItemViewModel> DeleteSaveMessage = new Message<SaveItemViewModel>("deleteSave");

        public static Message<DecoratedMemento> SaveGameMessage =
            new Message<(GameMetadataBundle, GameMemento)>("saveGame");

        // board and game actions

        public static Message<ITilePresenter> SetSelectedTileMessage = new Message<ITilePresenter>("selectedTile");
        public static Message<IUpdateAction> CommitMoveMessage = new Message<IUpdateAction>("committedMove");

        public static Message UndoMessage = new Message("undo");
        public static Message RedoMessage = new Message("redo");
        public static Message UndoTillRootMessage = new Message("undoTillRoot");
        public static Message RedoTillLastMessage = new Message("redoTillLast");
        public static Message ResumeGameMessage = new Message("resume");
        public static Message ForkGameMessage = new Message("fork");
        public static Message<ViewMode> SwitchGameViewMode = new Message<ViewMode>("switchViewMode");
    }


    /// <summary>
    ///     Encapsulates a message that can be sent or registered to on an IMessenger.
    ///     Greatly reduces the amount of boilerplate needed to register handlers.
    ///     Prevents messing up argument types or token, which cause silently lost messages.
    /// </summary>
    /// <typeparam name="T">Type of argument the message takes</typeparam>
    public class Message<T> {
        private string Token { get; }

        public Message(string token) {
            Token = token;
        }

        private readonly ConditionalWeakTable<object, Action<NotificationMessage<T>>> _subscribers
            = new ConditionalWeakTable<object, Action<NotificationMessage<T>>>();


        public void Send(IMessenger messenger, T param) {
            Send(messenger, param, Token);
        }

        public void Send(IMessenger messenger, T param, string message) {
            messenger.Send(message: new NotificationMessage<T>(content: param, notification: message),
                           token: Token);
        }

        public void Unregister(IMessenger messenger, object recipient) {
            messenger.Unregister<NotificationMessage<T>>(recipient: recipient, token: Token);
            _subscribers.Remove(recipient);
        }

        public void Register(IMessenger messenger, object recipient, Action<T> action) {
            // we have to keep a hard reference to the action passed to Register,
            // otherwise it's garbage collected
            // see https://stackoverflow.com/questions/25730530/bug-in-weakaction-in-case-of-closure-action
            Action<NotificationMessage<T>> notifAction = message => {
                Debug.WriteLine(message.Notification);
                action(message.Content);
            };

            _subscribers.Add(recipient, notifAction);

            messenger.Register(recipient: recipient,
                               token: Token,
                               action: notifAction);
        }
    }

    public class Message {
        public string Token { get; }

        public Message(string token) {
            Token = token;
        }

        private readonly ConditionalWeakTable<object, Action<NotificationMessage>> _subscribers =
            new ConditionalWeakTable<object, Action<NotificationMessage>>();

        public void Send(IMessenger messenger) {
            Send(messenger, Token);
        }

        public void Send(IMessenger messenger, string message) {
            messenger.Send(message: new NotificationMessage(Token), token: message);
        }

        public void Unregister(IMessenger messenger, object recipient) {
            messenger.Unregister<NotificationMessage>(recipient: recipient, token: Token);
        }


        public void Register(IMessenger messenger, object recipient, Action action) {
            Action<NotificationMessage> notifAction = message => {
                Debug.WriteLine(message.Notification);
                action();
            };

            _subscribers.Add(recipient, notifAction);

            messenger.Register(recipient: recipient,
                               token: Token,
                               action: notifAction);
        }
    }
}