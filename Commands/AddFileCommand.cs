using Microsoft.Win32;
using SoupMover.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace SoupMover.Commands
{
    public class AddFileCommand : BaseCommand
    {
        private readonly HomeViewModel HVM;
        public override void Execute(object parameter)
        {
            OpenFileDialog open = new OpenFileDialog();
            open.Multiselect = true;
            if (open.ShowDialog() == true)
            {
                foreach (string filename in open.FileNames)
                {
                    if (!HVM.SourceFilesContains(filename))
                        HVM.AddToSourceFiles(filename);
                }
            }
        }

        public AddFileCommand(HomeViewModel HVM)
        {
            this.HVM = HVM;
        }
    }
}
