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
        private ObservableCollection<string> _SourceFiles { get; init; } //Stores the current files that have not been added to any destination
        public IEnumerable<string> SourceFiles => _SourceFiles;

        private ObservableCollection<DestinationPathViewModel> _Directories { get; init; } //Stores the current directories with files to be moved in
        public IEnumerable<DestinationPathViewModel> Directories => _Directories;

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

        private int _SelectedDirectoryIndex;
        public int SelectedDirectoryIndex
        {
            get
            {
                return _SelectedDirectoryIndex;
            }
            set
            {
                _SelectedDirectoryIndex = value;
                OnPropertyChanged(nameof(SelectedDirectoryIndex));
            }
        }

        //ICommands
        public ICommand AddFileCommand { get; }
        public ICommand RemoveFileCommand { get; }
        public ICommand AddDirectoryCommand { get; }
        public ICommand RemoveDirectoryCommand { get; }
        public HomeViewModel()
        {
            _SourceFiles = new ObservableCollection<string>();
            _Directories = new ObservableCollection<DestinationPathViewModel>();
            _SelectedSourceIndex = -1;
            _SelectedDirectoryIndex = -1;

            //Commands
            AddFileCommand = new AddFileCommand(_SourceFiles);
            RemoveFileCommand = new RemoveFileCommand(_SourceFiles, this);
            AddDirectoryCommand = new AddDirectoryCommand(_Directories);
            RemoveDirectoryCommand = new RemoveDirectoryCommand(this, _Directories);

        }
    }
}
