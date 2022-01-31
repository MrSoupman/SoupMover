using SoupMover.Commands.FileCompareCommands;
using SoupMover.Services;
using SoupMover.Stores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SoupMover.ViewModels
{
    public class FileCompareViewModel : ViewModelBase
    {
        public ICommand YesCommand { get; }

        public FileCompareViewModel(ModalNavSvc modal, DialogStore store)
        {
            YesCommand = new YesCommand(modal, store);
        }

        public override void Dispose()
        {
            base.Dispose();
        }
    }
}
