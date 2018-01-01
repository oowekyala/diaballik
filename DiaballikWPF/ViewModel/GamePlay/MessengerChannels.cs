namespace DiaballikWPF.ViewModel {
    /// <summary>
    ///     Tokens for messenger communication.
    /// </summary>
    public static class MessengerChannels {
        public const string SelectedTileMessageToken = "selectedTile";
        public const string CommittedMoveMessageToken = "committedMove";
        public const string UndoMessageToken = "undo";
        public const string RedoMessageToken = "redo";
        public const string PassActionMessageToken = "pass";
        public const string SwitchToReplayModeMessageToken = "displayMode";
    }
}