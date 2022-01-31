using SoupMover.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoupMover.Commands.FileCompareCommands
{
    public class YesCommand : BaseFileCompareCommand
    {
        public override void Execute(object parameter)
        {
            CloseModal();
        }

        public YesCommand(ModalNavSvc modal)
        {
            Modal = modal;
        }
    }
}
