using SoupMover.Models;
using SoupMover.ViewModels;
using System.Windows;

namespace SoupMover.Commands
{
    public class RemoveDirectoryCommand : BaseCommand
    {
        private readonly HomeViewModel HVM;
        public override void Execute(object parameter)
        {
            DestinationPathViewModel Path = HVM.GetDirectory(HVM.SelectedDirectoryIndex);
            if (Path.GetFiles().Count > 0)
            {
                var result = MessageBox.Show("There are files queued to this location. Do you still want to remove this directory? Selecting yes will return all files to the source files.", "Confirm",
                    MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    foreach (ModFile file in Path.GetFiles())
                        HVM.AddToSourceFiles(file.FileName);
                    HVM.RemoveFromDirectories(HVM.SelectedDirectoryIndex);
                }
            }
            else
                HVM.RemoveFromDirectories(HVM.SelectedDirectoryIndex);
        }

        public override bool CanExecute(object parameter)
        {
            return HVM.SelectedDirectoryIndex > -1;
        }

        public RemoveDirectoryCommand(HomeViewModel HVM)
        {
            this.HVM = HVM;

            this.HVM.PropertyChanged += HVM_PropertyChanged;
        }

        private void HVM_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(HomeViewModel.SelectedDirectoryIndex))
                OnCanExecutedChanged();
        }
    }
}
