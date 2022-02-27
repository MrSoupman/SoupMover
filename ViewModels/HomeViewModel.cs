using SoupMover.Commands;
using SoupMover.Models;
using SoupMover.Services;
using SoupMover.Stores;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace SoupMover.ViewModels
{
    public class HomeViewModel : ViewModelBase
    {
        #region Viewmodel specific variables
        /// <summary>
        /// _SourceFiles differs from SourceFiles as the latter is used to display filtered results; _SourceFiles has the full data
        /// </summary>
        private List<string> _SourceFiles { get; init; }
        public ObservableCollection<string> SourceFiles { get; set; }

        private List<DestinationPathViewModel> _Directories { get; init; } //Stores the current directories with files to be moved in
        public ObservableCollection<string> Directories { get; set; }

        private ObservableCollection<ModFileViewModel> _DestinationFiles { get; set; }
        public IEnumerable<ModFileViewModel> DestinationFiles => _DestinationFiles;

        #endregion

        #region Public facing View Variables
        private int _SelectedSourceIndex;
        public int SelectedSourceIndex
        {
            get => _SelectedSourceIndex;
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
            get => _SelectedDirectoryIndex;
            set
            {
                _SelectedDirectoryIndex = value;
                if (value >= 0)
                {
                    SelectedDirectory = _Directories[ConvertDirectoryIndex()].Path;
                    var files = _Directories[ConvertDirectoryIndex()].GetFiles();
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
            get => _SelectedDestinationIndex;
            set
            {
                _SelectedDestinationIndex = value;
                if (value >= 0)
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
            get => _SelectedDirectory;
            set
            {
                _SelectedDirectory = value;
                OnPropertyChanged(nameof(SelectedDirectory));
            }
        }

        private int _TotalCount;
        public int TotalCount
        {
            get => _TotalCount;
            set
            {
                _TotalCount = value;
                OnPropertyChanged(nameof(TotalCount));
            }
        }

        private int _CurrentCount;
        public int CurrentCount
        {
            get => _CurrentCount;
            set
            {
                _CurrentCount = value;
                OnPropertyChanged(nameof(CurrentCount));
            }
        }
        private int _Progress;
        public int Progress
        {
            get => _Progress;
            set
            {
                _Progress = value;
                OnPropertyChanged(nameof(Progress));
            }
        }

        private PreviewViewModel _Preview;
        public PreviewViewModel Preview
        {
            get => _Preview;
            set
            {
                _Preview = value;
                OnPropertyChanged(nameof(Preview));
            }
        }

        private string _SelectedFile;
        public string SelectedFile
        {
            get => _SelectedFile;
            set
            {
                _SelectedFile = value;
                OnPropertyChanged(nameof(SelectedFile));
            }
        }

        private bool _IsMoving;
        public bool IsMoving
        {
            get => _IsMoving;
            set
            {
                _IsMoving = value;
                OnPropertyChanged(nameof(IsMoving));
            }
        }

        private bool _CancelMoving;
        public bool CancelMoving
        {
            get => _CancelMoving;
            set
            {
                _CancelMoving = value;
                OnPropertyChanged(nameof(CancelMoving));
            }
        }

        private string _SourceSearch;
        public string SourceSearch
        {
            get => _SourceSearch;
            set
            {
                _SourceSearch = value;
                if (!value.Equals("Search...") && !value.Equals(""))
                {
                    var temp = _SourceFiles.Where(file => file.Contains(value, System.StringComparison.InvariantCultureIgnoreCase)).ToList();
                    SourceFiles.Clear();

                    foreach (string file in temp)
                        SourceFiles.Add(file);
                }
                else
                {
                    SourceFiles.Clear();
                    foreach (string file in _SourceFiles)
                        SourceFiles.Add(file);
                }
            }
        }

        private string _DirectorySearch;
        public string DirectorySearch
        {
            get => _DirectorySearch;
            set
            {
                _DirectorySearch = value;
                if (!value.Equals("Search...") && !value.Equals(""))
                {
                    var temp = _Directories.Where(file => file.Path.Contains(value, System.StringComparison.InvariantCultureIgnoreCase)).ToList();
                    Directories.Clear();

                    foreach (DestinationPathViewModel path in temp)
                        Directories.Add(path.Path);
                }
                else
                {
                    Directories.Clear();
                    foreach (DestinationPathViewModel path in _Directories)
                        Directories.Add(path.Path);
                }
            }
        }

        private string _DestinationSearch;
        public string DestinationSearch
        {
            get => _DestinationSearch;
            set
            {
                _DestinationSearch = value;
                if (!value.Equals("Search...") && !value.Equals(""))
                {
                    var temp = _DestinationFiles.Where(file => file.FileName.Contains(value, System.StringComparison.InvariantCultureIgnoreCase)).ToList();
                    _DestinationFiles.Clear();

                    foreach (ModFileViewModel file in temp)
                        _DestinationFiles.Add(file);
                }
                else
                {
                    _DestinationFiles.Clear();
                    foreach (ModFileViewModel file in _DestinationFiles)
                        _DestinationFiles.Add(file);
                }
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
        public ICommand ResetCommand { get; }
        #endregion

        #region Methods
        public void RefreshDestinationListView()
        {
            var files = _Directories[ConvertDirectoryIndex()].GetFiles();
            _DestinationFiles.Clear();
            foreach (ModFile file in files)
            {
                _DestinationFiles.Add(new ModFileViewModel(file));
                
            }
        }

        public bool SourceFilesContains(string file) => _SourceFiles.Contains(file);

        public void AddToSourceFiles(string file)
        {
            if (!_SourceFiles.Contains(file))
            {
                _SourceFiles.Add(file);
                if (SourceSearch.Equals("") || SourceSearch.Equals("Search...") || file.ToLower().Contains(SourceSearch.ToLower()))
                    SourceFiles.Add(file);
            }
                
        }

        public void RemoveFromSourceFiles(int index)
        {
            if (index > -1)
            { 
                _SourceFiles.RemoveAt(index);
            }
        }

        public void RemoveFromSourceFiles(string file)
        {
            RemoveFromSourceFiles(_SourceFiles.IndexOf(file));
            if (SourceFiles.Contains(file))
                SourceFiles.Remove(file);
        }

        public void AddToDirectories(DestinationPathViewModel DestPath)
        {
            if (!_Directories.Contains(DestPath))
            {
                _Directories.Add(DestPath);
                _Directories.Sort();
                if (DirectorySearch.Equals("") || DirectorySearch.Equals("Search...") || DestPath.Path.ToLower().Contains(DirectorySearch.ToLower()))
                    Directories.Insert(_Directories.IndexOf(DestPath), DestPath.Path);

            }

        }

        public void RemoveFromDirectories(int index)
        {
            if (index > -1)
                _Directories.RemoveAt(index);
        }

        public void RemoveFromDirectories(DestinationPathViewModel DestPath)
        {
            RemoveFromDirectories(_Directories.IndexOf(DestPath));
            if (Directories.Contains(DestPath.Path))
                Directories.Remove(DestPath.Path);
        }

        public DestinationPathViewModel GetDirectory(int index)
        {
            DestinationPathViewModel DestVM = new DestinationPathViewModel(new DestinationPath(Directories[index]));
            return _Directories[_Directories.IndexOf(DestVM)];
        }

        public void AddToDestination(string file)
        {
            
            _Directories[ConvertDirectoryIndex()].AddFile(file);
        }

        public void RemoveFromDestination(string file)
        {
            _Directories[ConvertDirectoryIndex()].RemoveFile(file);
        }

        private int ConvertDirectoryIndex()
        {
            DestinationPathViewModel DestVM = new DestinationPathViewModel(new DestinationPath(Directories[SelectedDirectoryIndex]));
            return _Directories.IndexOf(DestVM);
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

        public void Reset()
        {
            SelectedSourceIndex = -1;
            SelectedDestinationIndex = -1;
            SelectedDirectoryIndex = -1;
            SelectedDirectory = "(No directory selected)";
            CurrentCount = 0;
            TotalCount = 0;
            SourceSearch = "Search...";
            DirectorySearch = "Search...";
            DestinationSearch = "Search...";

            _SourceFiles.Clear();
            SourceFiles.Clear();
            _Directories.Clear();
            Directories.Clear();
            _DestinationFiles.Clear();
        }

        #endregion
        public HomeViewModel(ModalNavSvc modal, DialogStore dialog, PreviewViewModel Preview)
        {
            _SourceFiles = new List<string>();
            SourceFiles = new ObservableCollection<string>();
            _Directories = new List<DestinationPathViewModel>();
            Directories = new ObservableCollection<string>();
            _DestinationFiles = new ObservableCollection<ModFileViewModel>();
            //DestinationFiles = new ObservableCollection<string>();
            _SelectedSourceIndex = -1;
            _SelectedDestinationIndex = -1;
            _SelectedDirectoryIndex = -1;
            _SelectedDirectory = "(No directory selected)";
            _CurrentCount = 0;
            _TotalCount = 0;
            _SourceSearch = "Search...";
            _DirectorySearch = "Search...";
            _DestinationSearch = "Search...";
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
            ResetCommand = new ResetCommand(this);
        }
    }
}
