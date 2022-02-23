using SoupMover.Models;
using SoupMover.ViewModels;

namespace SoupMover.Commands
{
    public class AddDirectoryCommand : BaseCommand
    {
        private readonly HomeViewModel HVM;
        public override void Execute(object parameter)
        {
            using var dialog = new System.Windows.Forms.FolderBrowserDialog();
            System.Windows.Forms.DialogResult result = dialog.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK && dialog.SelectedPath.Length > 0)
            {
                DestinationPathViewModel DestPathVM = new DestinationPathViewModel(new DestinationPath(dialog.SelectedPath));
                HVM.AddToDirectories(DestPathVM);
            }
        }

        public AddDirectoryCommand(HomeViewModel HVM)
        {
            this.HVM = HVM;
        }
    }
}
