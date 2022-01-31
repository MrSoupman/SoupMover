using SoupMover.Stores;
using SoupMover.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoupMover.Services
{
    public class ModalNavSvc
    {
        private readonly ModalNavStore modal;
        private readonly Func<ViewModelBase> createViewModel;
        public void Navigate()
        {
            modal.CurrentVM = createViewModel();
        }
        public ModalNavSvc(ModalNavStore modal, Func<ViewModelBase> createViewModel)
        {
            this.modal = modal;
            this.createViewModel = createViewModel;
        }

        public ModalNavStore GetModalNavStore()
        {
            return modal;
        }
    }
}
