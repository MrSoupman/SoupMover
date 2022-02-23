using SoupMover.ViewModels;

namespace SoupMover.Commands
{
    public class CancelCommand : BaseCommand
    {
        private readonly HomeViewModel HVM;
        public override void Execute(object parameter)
        {
            HVM.CancelMoving = true;
        }
        public override bool CanExecute(object parameter)
        {
            return HVM.IsMoving;
        }

        public CancelCommand(HomeViewModel HVM)
        {
            this.HVM = HVM;
            this.HVM.PropertyChanged += HVM_PropertyChanged;
        }

        private void HVM_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(HomeViewModel.IsMoving))
            {
                OnCanExecutedChanged();
            }
        }
    }
}
