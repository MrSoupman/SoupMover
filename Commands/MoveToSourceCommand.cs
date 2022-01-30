using SoupMover.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoupMover.Commands
{
    public class MoveToSourceCommand : BaseCommand
    {
        private readonly HomeViewModel HVM;
        private readonly ObservableCollection<DestinationPathViewModel> Directories;
        private readonly ObservableCollection<string> SourceFiles;
        public override void Execute(object parameter)
        {
            if (parameter != null)
            {
                System.Collections.IList items = (System.Collections.IList)parameter; var selection = items?.Cast<ModFileViewModel>();
                List<ModFileViewModel> test = selection.ToList();
                HVM.SelectedDestinationIndex = -1;
                foreach (ModFileViewModel file in test)
                {
                    if (!SourceFiles.Contains(file.FileName))
                        SourceFiles.Add(file.FileName);
                    Directories[HVM.SelectedDirectoryIndex].RemoveFile(file.FileName);
                    HVM.TotalCount -= 1;
                }
                HVM.RefreshDestinationListView();
            }
        }

        public override bool CanExecute(object parameter)
        {
            return HVM.SelectedDirectoryIndex > -1 && HVM.SelectedDestinationIndex > -1;
        }

        public MoveToSourceCommand(HomeViewModel HVM, ObservableCollection<DestinationPathViewModel> Directories, ObservableCollection<string> SourceFiles)
        {
            this.HVM = HVM;
            this.Directories = Directories;
            this.SourceFiles = SourceFiles;
            this.HVM.PropertyChanged += HVM_PropertyChanged;
        }
        private void HVM_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(HomeViewModel.SelectedDirectoryIndex) || e.PropertyName == nameof(HomeViewModel.SelectedDestinationIndex))
                OnCanExecutedChanged();
        }
    }
}
