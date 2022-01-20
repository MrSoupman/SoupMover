using SoupMover.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SoupMover.ViewModels
{
    public class HomeViewModel : ViewModelBase
    {
        //Viewmodel specific variables
        public ObservableCollection<string> SourceFiles { get; init; } //Stores the current files that have not been added to any destination

        public ObservableCollection<DestinationPathViewModel> Directories { get; init; } //Stores the current directories with files to be moved in

        //Public facing View Variables
        private int _SelectedSourceIndex;
        public int SelectedSourceIndex 
        {
            get
            {
                return _SelectedSourceIndex;
            }
            set
            {
                _SelectedSourceIndex = value;
                OnPropertyChanged(nameof(SelectedSourceIndex));
            }
        }

        public ICommand AddFileCommand { get; }
        public ICommand AddDirectoryCommand { get; }
        public HomeViewModel()
        {
            SourceFiles = new ObservableCollection<string>();
            Directories = new ObservableCollection<DestinationPathViewModel>();
            _SelectedSourceIndex = -1;

            //Commands
            AddFileCommand = new AddFileCommand(SourceFiles);
            AddDirectoryCommand = new AddDirectoryCommand(Directories);

        }
    }
}
