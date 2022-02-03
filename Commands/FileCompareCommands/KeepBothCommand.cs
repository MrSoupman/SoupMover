using SoupMover.Services;
using SoupMover.Stores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoupMover.Commands.FileCompareCommands
{
    public class KeepBothCommand : BaseFileCompareCommand
    {
        public override void Execute(object parameter)
        {
            Store.GetResult(4);
            CloseModal();
        }

        public KeepBothCommand(ModalNavSvc modal, DialogStore store)
        {
            Modal = modal;
            Store = store;
        }
    }
}
