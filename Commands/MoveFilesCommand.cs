using SoupMover.Models;
using SoupMover.Services;
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
        public override void Execute(object parameter)
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
            modal.Navigate();
            
        }

        public override bool CanExecute(object parameter)
        {
            //return HVM.TotalCount > 0;
            return true;
        }

        public MoveFilesCommand(HomeViewModel HVM, ObservableCollection<DestinationPathViewModel> Directories, ModalNavSvc modal)
        {
            this.HVM = HVM;
            this.HVM.PropertyChanged += HVM_PropertyChanged;
            this.Directories = Directories;
            this.modal = modal;
        }

        private void HVM_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(HomeViewModel.TotalCount))
                OnCanExecutedChanged();
        }
    }
}
