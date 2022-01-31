using SoupMover.Models;
using SoupMover.Services;
using SoupMover.Stores;
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
    public class MoveFilesCommand : BaseCommand
    {
        private readonly HomeViewModel HVM;
        private readonly ObservableCollection<DestinationPathViewModel> Directories;
        private readonly ModalNavSvc modal;
        private readonly DialogStore dialog;
        private TaskCompletionSource<int> result;
        private enum CompareResult {Yes, YesToAll, No, NoToAll, KeepBoth, Cancel }

        public override async void Execute(object parameter)
        {
            /*
            var res = MessageBox.Show("Move files?", "Confirm", MessageBoxButton.OKCancel, MessageBoxImage.Question);
            if (res == MessageBoxResult.OK)
            {
                //Prep work
                foreach (DestinationPathViewModel path in Directories)
                {
                    List<ModFile> files = path.GetFiles();
                    foreach (ModFile file in files)
                    { 
                        
                    }
                }
            }
            */
            result = new TaskCompletionSource<int>();
            modal.Navigate();
            await result.Task;
            
            
            
        }

        public override bool CanExecute(object parameter)
        {
            //return HVM.TotalCount > 0;
            return true;
        }

        public MoveFilesCommand(HomeViewModel HVM, ObservableCollection<DestinationPathViewModel> Directories, ModalNavSvc modal, DialogStore dialog)
        {
            this.HVM = HVM;
            this.HVM.PropertyChanged += HVM_PropertyChanged;
            this.Directories = Directories;
            this.modal = modal;
            this.dialog = dialog;
            this.dialog.FileCompareResult += dialog_FileCompareResult;
        }

        private void dialog_FileCompareResult(int obj)
        {
            result?.TrySetResult(obj);
        }

        private void HVM_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(HomeViewModel.TotalCount))
                OnCanExecutedChanged();
        }
    }
}
