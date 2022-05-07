using SoupMover.Models;
using SoupMover.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoupMover.Commands
{
    public class RecursiveAddDirCommand : BaseCommand
    {
        private readonly HomeViewModel HVM;
        public void RecursiveAdd(string dir)
        {
            try
            {
                List<string> dirs = new List<string>(Directory.EnumerateDirectories(dir));
                if (dirs.Count == 0)
                    return;
                foreach (var directory in dirs)
                {
                    HVM.AddToDirectories(new DestinationPathViewModel(new DestinationPath(directory)));
                    RecursiveAdd(directory);
                }
            }
            catch (UnauthorizedAccessException e)
            {
                throw;
            }
            catch (PathTooLongException e)
            {
                throw;
            }
        }

        public override void Execute(object parameter)
        {
            using var dialog = new System.Windows.Forms.FolderBrowserDialog();
            System.Windows.Forms.DialogResult result = dialog.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK && dialog.SelectedPath.Length > 0)
            {
                RecursiveAdd(dialog.SelectedPath);
            }
        }

        public RecursiveAddDirCommand(HomeViewModel HVM)
        {
            this.HVM = HVM;
        }
    }
}
