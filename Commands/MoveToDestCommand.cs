using SoupMover.ViewModels;
using System.Collections.Generic;
using System.Linq;

namespace SoupMover.Commands
{
    public class MoveToDestCommand : BaseCommand
    {
        private readonly HomeViewModel HVM;
        private short count = 0; //every time this hits 10, we autosave
        public override void Execute(object parameter)
        {
            if (parameter != null)
            {
                System.Collections.IList items = (System.Collections.IList)parameter; var selection = items?.Cast<string>();
                List<string> test = selection.ToList(); //Need to add selection to its own list because we remove items as we go along, breaking enumeration
                HVM.SelectedSourceIndex = -1;
                foreach (string file in test)
                {
                    HVM.RemoveFromSourceFiles(file);
                    HVM.AddToDestination(file);
                    HVM.TotalCount += 1;
                    count++;
                }
                HVM.RefreshDestinationListView();
                if (count >= 10)
                {
                    HVM.SaveCommand.Execute("autosave");
                    count = 0;
                }
            }
        }

        public override bool CanExecute(object parameter)
        {
            return HVM.SelectedDirectoryIndex > -1 && HVM.SelectedSourceIndex > -1;
        }

        public MoveToDestCommand(HomeViewModel HVM)
        {
            this.HVM = HVM;
            this.HVM.PropertyChanged += HVM_PropertyChanged;
        }

        private void HVM_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(HomeViewModel.SelectedDirectoryIndex) || e.PropertyName == nameof(HomeViewModel.SelectedSourceIndex))
                OnCanExecutedChanged();
        }
    }
}
