using SoupMover.Services;
using SoupMover.Stores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoupMover.Commands.FileCompareCommands
{
    public class CancelCommand : BaseFileCompareCommand
    {
        public override void Execute(object parameter)
        {
            Store.GetResult(5);
            CloseModal();
        }

        public CancelCommand(ModalNavSvc modal, DialogStore store)
        {
            Modal = modal;
            Store = store;
        }
    }
}
