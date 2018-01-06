using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Diaballik.Core;
using DiaballikWPF.ViewModel;
using GalaSoft.MvvmLight.Messaging;

namespace DiaballikWPF.Util {
    using GameId = String;


    /// <summary>
    ///     Enumerates messages available for messenger communication.
    /// </summary>
    public static class Messages {
        public static Message ShowMainMenuMessage = new Message("showMainMenu");
        public static Message ShowLoadMenuMessage = new Message("showLoadMenu");
        public static Message<(Game, ViewMode)> OpenNewGame = new Message<(Game, ViewMode)>("showGameScreen");
        public static Message ShowNewGameMessage = new Message("showNewGame");
        public static Message AppShutdownMessage = new Message("appShutdown");

        // save popup

        public static Message<(string, Action, bool)> ShowSavePopupMessage =
            new Message<(string, Action, bool)>("showSavePopup");

        public static Message CloseSavePopupMessage = new Message("closeSavePopup");

        // load / save

        /// request the game screen to send its primary game to the save manager for saving
        public static Message RequestSaveToGameScreenMessage = new Message("saveRequest");

        public static Message<((GameId, GameMemento), ViewMode)> OpenGameMessage 
            = new Message<((GameId, GameMemento), ViewMode)>("loadGameFromSave");

        public static Message<GameId> DeleteSaveInManagerMessage = new Message<GameId>("deleteSaveForReal");

        // sent by a list item to the listview vm, followed by a DeleteSaveInManagerMessage
        public static Message<SaveItemViewModel> DeleteSaveInViewMessage = new Message<SaveItemViewModel>("deleteSave");

        public static Message<(GameId, GameMemento)> SaveGameMessage = new Message<(GameId, GameMemento)>("saveGame");

        // board and game actions

        public static Message<ITilePresenter> SetSelectedTileMessage = new Message<ITilePresenter>("selectedTile");
        public static Message<IUpdateAction> CommitMoveMessage = new Message<IUpdateAction>("committedMove");

        public static Message UndoMessage = new Message("undo");
        public static Message RedoMessage = new Message("redo");
        public static Message UndoTillRootMessage = new Message("undoTillRoot");
        public static Message RedoTillLastMessage = new Message("redoTillLast");
        public static Message ResumeGameMessage = new Message("resume");
        public static Message ForkGameMessage = new Message("fork");
        public static Message<ViewMode> SwitchGameViewModeMessage = new Message<ViewMode>("switchViewMode");
        public static Message PauseGameMessage = new Message("closeGame");
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

        private readonly ConditionalWeakTable<object, List<Action<NotificationMessage<T>>>> _subscribers =
            new ConditionalWeakTable<object, List<Action<NotificationMessage<T>>>>();


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
                Debug.WriteLine($"{message.Notification} ({message.Content})");
                action(message.Content);
            };
            var ok = _subscribers.TryGetValue(recipient, out var actions);
            if (ok) {
                actions.Add(notifAction);
            } else {
                _subscribers.Add(recipient, new List<Action<NotificationMessage<T>>> {notifAction});
            }
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

        private readonly ConditionalWeakTable<object, List<Action<NotificationMessage>>> _subscribers =
            new ConditionalWeakTable<object, List<Action<NotificationMessage>>>();

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

            var ok = _subscribers.TryGetValue(recipient, out var actions);
            if (ok) {
                actions.Add(notifAction);
            } else {
                _subscribers.Add(recipient, new List<Action<NotificationMessage>> {notifAction});
            }


            messenger.Register(recipient: recipient,
                               token: Token,
                               action: notifAction);
        }
    }
}