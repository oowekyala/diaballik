namespace DiaballikWPF.ViewModel {
    /// <summary>
    ///     Tokens for messenger communication.
    /// </summary>
    public static class MessengerChannels {
        public const string ShowGameCreationScreenMessageToken = "showNewGame";
        public const string ShowGameScreenMessageToken = "showGameScreen";
        public const string ShowLoadScreenMessageToken = "showLoadScreen";
        public const string ShowMainMenuMessageToken = "showLoadScreen";

        public const string CloseVictoryPopupMessageToken = "closeVictoryPopup";
        public const string ShowVictoryPopupMessageToken = "closeVictoryPopup";


        public const string SelectedTileMessageToken = "selectedTile";
        public const string CommittedMoveMessageToken = "committedMove";
        public const string UndoMessageToken = "undo";
        public const string RedoMessageToken = "redo";
        public const string SwitchToReplayModeMessageToken = "displayMode";
        public const string ResumeGameMessageToken = "resume";
        public const string ForkGameMessageToken = "fork";
        public const string UndoTillRootMessageToken = "undoTillRoot";
        public const string RedoTillLastMessageToken = "redoTillLast";
    }
}