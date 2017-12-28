using Diaballik.Core;
using GalaSoft.MvvmLight;

namespace DiaballikWPF.ViewModel {
    public class PlayGameScreenViewModel : ViewModelBase {
        #region Constructor

        public PlayGameScreenViewModel(Game game) {
            BoardViewModel = new BoardViewModel(game);
        }

        #endregion

        #region Properties

        public BoardViewModel BoardViewModel { get; set; }

        #endregion
    }
}