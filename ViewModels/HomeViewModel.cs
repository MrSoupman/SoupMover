﻿using SoupMover.Commands;
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

        public void AddToSourceFiles(string file)
        {
            _SourceFiles.Add(file);
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
            AddFileCommand = new AddFileCommand(_SourceFiles);
            RemoveFileCommand = new RemoveFileCommand(_SourceFiles, this);
            AddDirectoryCommand = new AddDirectoryCommand(_Directories);
            RemoveDirectoryCommand = new RemoveDirectoryCommand(this, _Directories);
            MoveToDestCommand = new MoveToDestCommand(this, _Directories, _SourceFiles);
            MoveToSourceCommand = new MoveToSourceCommand(this, _Directories, _SourceFiles);
            MoveFilesCommand = new MoveFilesCommand(this, _Directories, modal, dialog);
            CancelCommand = new CancelCommand(this);
        }
    }
}
