using SoupMover.Stores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoupMover.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private readonly NavStore nav;
        private readonly ModalNavStore modal;
        public ViewModelBase CurrentVM => nav.CurrentVM;
        public ViewModelBase CurrentModal => modal.CurrentVM;

        public bool IsOpen => modal.IsOpen;

        public MainViewModel(NavStore nav, ModalNavStore modal)
        {
            this.nav = nav;
            nav.CurrentViewModelChanged += Nav_CurrentViewModelChanged;
            this.modal = modal;
            modal.CurrentViewModelChanged += Modal_CurrentViewModelChanged;
        }

        private void Modal_CurrentViewModelChanged()
        {
            OnPropertyChanged(nameof(CurrentModal));
            OnPropertyChanged(nameof(IsOpen));
        }

        private void Nav_CurrentViewModelChanged()
        {
            OnPropertyChanged(nameof(CurrentVM));
        }
    }
}
