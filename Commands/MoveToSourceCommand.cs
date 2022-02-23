using SoupMover.ViewModels;
using System.Collections.Generic;
using System.Linq;

namespace SoupMover.Commands
{
    public class MoveToSourceCommand : BaseCommand
    {
        private readonly HomeViewModel HVM;
        public override void Execute(object parameter)
        {
            if (parameter != null)
            {
                System.Collections.IList items = (System.Collections.IList)parameter; var selection = items?.Cast<ModFileViewModel>();
                List<ModFileViewModel> test = selection.ToList();
                HVM.SelectedDestinationIndex = -1;
                foreach (ModFileViewModel file in test)
                {
                    if (!HVM.SourceFilesContains(file.FileName))
                        HVM.AddToSourceFiles(file.FileName);
                    HVM.RemoveFromDestination(file.FileName);
                    HVM.TotalCount -= 1;
                }
                HVM.RefreshDestinationListView();
            }
        }

        public override bool CanExecute(object parameter)
        {
            return HVM.SelectedDirectoryIndex > -1 && HVM.SelectedDestinationIndex > -1;
        }

        public MoveToSourceCommand(HomeViewModel HVM)
        {
            this.HVM = HVM;
            this.HVM.PropertyChanged += HVM_PropertyChanged;
        }
        private void HVM_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(HomeViewModel.SelectedDirectoryIndex) || e.PropertyName == nameof(HomeViewModel.SelectedDestinationIndex))
                OnCanExecutedChanged();
        }
    }
}
