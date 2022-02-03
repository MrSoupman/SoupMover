using SoupMover.Services;
using SoupMover.Stores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoupMover.Commands.FileCompareCommands
{
    public class NoCommand : BaseFileCompareCommand
    {
        public override void Execute(object parameter)
        {
            Store.GetResult(2);
            CloseModal();
        }

        public NoCommand(ModalNavSvc modal, DialogStore store)
        {
            Modal = modal;
            Store = store;
        }
    }
}
