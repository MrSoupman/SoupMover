using SoupMover.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoupMover.Stores
{
    public class NavStore
    {
        private ViewModelBase _CurrentVM;
        public ViewModelBase CurrentVM
        {
            get => _CurrentVM;
            set
            {
                _CurrentVM = value;
                OnCurrentViewModelChanged();
            }

        }

        private void OnCurrentViewModelChanged()
        {
            CurrentViewModelChanged?.Invoke();
        }

        public event Action CurrentViewModelChanged;
    }
}
