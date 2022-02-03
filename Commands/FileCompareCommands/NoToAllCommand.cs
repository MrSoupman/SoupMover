using SoupMover.Services;
using SoupMover.Stores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoupMover.Commands.FileCompareCommands
{
    public class NoToAllCommand : BaseFileCompareCommand
    {
        public override void Execute(object parameter)
        {
            Store.GetResult(3);
            CloseModal();
        }

        public NoToAllCommand(ModalNavSvc modal, DialogStore store)
        {
            Modal = modal;
            Store = store;
        }
    }
}
