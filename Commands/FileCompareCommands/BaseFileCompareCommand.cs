using SoupMover.Services;
using SoupMover.Stores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoupMover.Commands.FileCompareCommands
{
    public abstract class BaseFileCompareCommand : BaseCommand
    {
        public ModalNavSvc Modal { get; init; }
        public DialogStore Store { get; init; }

        public void CloseModal()
        {
            Modal.GetModalNavStore().Close();
        }

        
    }
}
