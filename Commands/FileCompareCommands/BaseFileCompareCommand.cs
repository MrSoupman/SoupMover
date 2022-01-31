using SoupMover.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoupMover.Commands.FileCompareCommands
{
    public abstract class BaseFileCompareCommand : BaseCommand
    {
        public ModalNavSvc Modal { get; set; }

        public void CloseModal()
        {
            Modal.GetModalNavStore().Close();
        }

        
    }
}
