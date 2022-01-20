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
        public ViewModelBase CurrentVM => nav.CurrentVM;

        public MainViewModel(NavStore nav)
        {
            this.nav = nav;
            nav.CurrentViewModelChanged += Nav_CurrentViewModelChanged;
        }

        private void Nav_CurrentViewModelChanged()
        {
            OnPropertyChanged(nameof(CurrentVM));
        }
    }
}
