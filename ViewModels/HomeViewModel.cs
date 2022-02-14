using SoupMover.Commands;
using SoupMover.Models;
using SoupMover.Services;
using SoupMover.Stores;
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
        #region Viewmodel specific variables
        private ObservableCollection<string> _SourceFiles { get; init; } //Stores the current files that have not been added to any destination
        public IEnumerable<string> SourceFiles => _SourceFiles;

        private ObservableCollection<DestinationPathViewModel> _Directories { get; init; } //Stores the current directories with files to be moved in
        public IEnumerable<DestinationPathViewModel> Directories => _Directories;

        private ObservableCollection<ModFileViewModel> _DestinationFiles { get; set; }
        public IEnumerable<ModFileViewModel> DestinationFiles => _DestinationFiles;

        #endregion

        #region Public facing View Variables
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
                if (value >= 0)
                    SelectedFile = _SourceFiles[value];
                else
                    SelectedFile = "";
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
                if (value >= 0)
                {
                    SelectedDirectory = _Directories[SelectedDirectoryIndex].Path;
                    var files = _Directories[SelectedDirectoryIndex].GetFiles();
                    _DestinationFiles.Clear();
                    foreach (ModFile file in files)
                    {
                        _DestinationFiles.Add(new ModFileViewModel(file));
                    }
                }
                OnPropertyChanged(nameof(SelectedDirectoryIndex));
            }
        }

        private int _SelectedDestinationIndex;
        public int SelectedDestinationIndex
        {
            get
            {
                return _SelectedDestinationIndex;
            }
            set
            {
                _SelectedDestinationIndex = value;
                if(value >= 0)
                    SelectedFile = _DestinationFiles[value].ToString();
                else
                    SelectedFile = "";
                OnPropertyChanged(nameof(SelectedDestinationIndex));
            }
        }

        private string _SelectedDirectory;
        /// <summary>
        /// Represents the TEXT of the selected directory
        /// </summary>
        public string SelectedDirectory
        {
            get
            {
                return _SelectedDirectory;
            }
            set
            {
                _SelectedDirectory = value;
                OnPropertyChanged(nameof(SelectedDirectory));
            }
        }

        private int _TotalCount;
        public int TotalCount
        {
            get
            {
                return _TotalCount;
            }
            set
            {
                _TotalCount = value;
                OnPropertyChanged(nameof(TotalCount));
            }
        }

        private int _CurrentCount;
        public int CurrentCount
        {
            get
            {
                return _CurrentCount;
            }
            set
            {
                _CurrentCount = value;
                OnPropertyChanged(nameof(CurrentCount));
            }
        }
        private int _Progress;
        public int Progress
        {
            get
            {
                return _Progress;
            }
            set
            {
                _Progress = value;
                OnPropertyChanged(nameof(Progress));
            }
        }

        private PreviewViewModel _Preview;
        public PreviewViewModel Preview
        {
            get
            {
                return _Preview;
            }
            set
            {
                _Preview = value;
                OnPropertyChanged(nameof(Preview));
            }
        }

        private string _SelectedFile;
        public string SelectedFile
        {
            get
            {
                return _SelectedFile;
            }
            set
            {
                _SelectedFile = value;
                OnPropertyChanged(nameof(SelectedFile));
            }
        }

        private bool _IsMoving;
        public bool IsMoving
        {
            get
            {
                return _IsMoving;
            }
            set
            {
                _IsMoving = value;
                OnPropertyChanged(nameof(IsMoving));
            }
        }

        private bool _CancelMoving;
        public bool CancelMoving
        {
            get
            {
                return _CancelMoving;
            }
            set
            {
                _CancelMoving = value;
                OnPropertyChanged(nameof(CancelMoving));
            }
        }

        #endregion

        #region ICommands
        public ICommand AddFileCommand { get; }
        public ICommand RemoveFileCommand { get; }
        public ICommand AddDirectoryCommand { get; }
        public ICommand RemoveDirectoryCommand { get; }
        public ICommand MoveToDestCommand { get; }
        public ICommand MoveToSourceCommand { get; }
        public ICommand MoveFilesCommand { get; }
        public ICommand CancelCommand { get; }
        public ICommand SaveCommand { get; }
        public ICommand LoadCommand { get; }
        #endregion

        #region Methods
        public void RefreshDestinationListView()
        {
            var files = _Directories[SelectedDirectoryIndex].GetFiles();
            _DestinationFiles.Clear();
            foreach (ModFile file in files)
            {
                _DestinationFiles.Add(new ModFileViewModel(file));
            }
        }

        public bool SourceFilesContains(string file) => _SourceFiles.Contains(file);

        public void AddToSourceFiles(string file)
        {
            if(!_SourceFiles.Contains(file))
                _SourceFiles.Add(file);
        }

        public void RemoveFromSourceFiles(int index)
        {
            if (index > -1)
                _SourceFiles.RemoveAt(index);
        }

        public void RemoveFromSourceFiles(string file)
        {
            RemoveFromSourceFiles(_SourceFiles.IndexOf(file));
        }

        public void AddToDirectories(DestinationPathViewModel DestPath)
        {
            if (!_Directories.Contains(DestPath))
            {
                _Directories.Add(DestPath);
                SortDirectories();
            }
               
        }

        private void SortDirectories()
        {

            ObservableCollection<DestinationPathViewModel> temp;
            var ordered = _Directories.OrderBy(p => p).AsEnumerable();
            temp = new ObservableCollection<DestinationPathViewModel>(ordered);
            _Directories.Clear();
            foreach (DestinationPathViewModel j in temp) 
                _Directories.Add(j);
        }

        public void RemoveFromDirectories(int index)
        {
            if(index > -1)
                _Directories.RemoveAt(index);
        }

        public DestinationPathViewModel GetDirectory(int index)
        {
            return _Directories[index];
        }

        public void AddToDestination(string file)
        {
            _Directories[SelectedDirectoryIndex].AddFile(file);
        }

        public void RemoveFromDestination(string file)
        {
            _Directories[SelectedDirectoryIndex].RemoveFile(file);
        }

        public void ClearDestinationListView()
        {
            SelectedDestinationIndex = -1;
            _DestinationFiles.Clear();
        }
        public void ResetIndices()
        {
            SelectedDestinationIndex = -1;
            SelectedDirectoryIndex = -1;
            SelectedSourceIndex = -1;
        }

        

        #endregion
        public HomeViewModel(ModalNavSvc modal, DialogStore dialog, PreviewViewModel Preview)
        {
            _SourceFiles = new ObservableCollection<string>();
            _Directories = new ObservableCollection<DestinationPathViewModel>();
            _DestinationFiles = new ObservableCollection<ModFileViewModel>();
            _SelectedSourceIndex = -1;
            _SelectedDestinationIndex = -1;
            _SelectedDirectoryIndex = -1;
            _SelectedDirectory = "(No directory selected)";
            _CurrentCount = 0;
            _TotalCount = 0;
            this.Preview = Preview;
            this.Preview.SetHomeViewModel(this);

            //Commands
            AddFileCommand = new AddFileCommand(this);
            RemoveFileCommand = new RemoveFileCommand(this);
            AddDirectoryCommand = new AddDirectoryCommand(this);
            RemoveDirectoryCommand = new RemoveDirectoryCommand(this);
            MoveToDestCommand = new MoveToDestCommand(this);
            MoveToSourceCommand = new MoveToSourceCommand(this);
            MoveFilesCommand = new MoveFilesCommand(this, _Directories, modal, dialog);
            CancelCommand = new CancelCommand(this);
            SaveCommand = new SaveCommand(_SourceFiles, _Directories);
            LoadCommand = new LoadCommand(this);
        }
    }
}
