using SoupMover.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoupMover.Commands
{
    public class ResetCommand : BaseCommand
    {
        public readonly HomeViewModel HVM;
        public override void Execute(object parameter)
        {
            HVM.Reset();
        }

        public ResetCommand(HomeViewModel HVM)
        {
            this.HVM = HVM;
        }
    }
}
