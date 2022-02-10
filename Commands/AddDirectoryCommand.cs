using SoupMover.Models;
using SoupMover.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoupMover.Commands
{
    public class AddDirectoryCommand : BaseCommand
    {
        private readonly ObservableCollection<DestinationPathViewModel> Directories;
        public override void Execute(object parameter)
        {
            using var dialog = new System.Windows.Forms.FolderBrowserDialog();
            System.Windows.Forms.DialogResult result = dialog.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK && dialog.SelectedPath.Length > 0)
            {
                DestinationPathViewModel DestPathVM = new DestinationPathViewModel(new DestinationPath(dialog.SelectedPath));
                if (!Directories.Contains(DestPathVM))
                { 
                    Directories.Add(DestPathVM);
                    
                }
            }
        }

        public AddDirectoryCommand(ObservableCollection<DestinationPathViewModel> Directories)
        {
            this.Directories = Directories;
        }
    }
}
