using SoupMover.Models;
using SoupMover.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SoupMover.Commands
{
    public class RemoveDirectoryCommand : BaseCommand
    {
        private readonly HomeViewModel HVM;
        private readonly ObservableCollection<DestinationPathViewModel> DestVMCollection;
        public override void Execute(object parameter)
        {
            DestinationPath path = DestVMCollection[HVM.SelectedDirectoryIndex].GetDestinationPath();
            if (path.GetFiles().Count > 0)
            {
                var result = MessageBox.Show("There are files queued to this location. Do you still want to remove this directory? Selecting yes will return all files to the source files.", "Confirm",
                    MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    DestVMCollection.Remove(new DestinationPathViewModel(path));
                }
            }
            else
                DestVMCollection.Remove(new DestinationPathViewModel(path));
        }

        public override bool CanExecute(object parameter)
        {
            return HVM.SelectedDirectoryIndex > -1;
        }

        public RemoveDirectoryCommand(HomeViewModel HVM, ObservableCollection<DestinationPathViewModel> DestVMCollection)
        {
            this.HVM = HVM;
            this.DestVMCollection = DestVMCollection;
            this.HVM.PropertyChanged += HVM_PropertyChanged;
        }

        private void HVM_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(HomeViewModel.SelectedDirectoryIndex))
                OnCanExecutedChanged();
        }
    }
}
