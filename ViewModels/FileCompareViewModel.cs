using SoupMover.Commands.FileCompareCommands;
using SoupMover.Services;
using SoupMover.Stores;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media.Imaging;

namespace SoupMover.ViewModels
{
    public class FileCompareViewModel : ViewModelBase
    {
        private string FileSizeUnit(string file)
        {
            string[] unit = { "bytes", "KB", "MB", "GB", "TB" };
            double size = new FileInfo(file).Length;
            int count;
            for (count = 0; count < unit.Length; count++)
            {
                if (size / 1024 > 1)
                    size /= 1024;
                else
                    break;
            }
            string sizeUnit = String.Format("{0:F2} {1}", size, unit[count]);
            return sizeUnit;
        }


        #region Bindings
        private string _SourceFileName;
        public string SourceFileName 
        {
            get
            {
                return _SourceFileName;
            }
            set
            {
                _SourceFileName = value;
                OnPropertyChanged(nameof(SourceFileName));
            }
        }

        private string _SourceDirectoryName;
        public string SourceDirectoryName
        {
            get
            {
                return _SourceDirectoryName;
            }
            set
            {
                _SourceDirectoryName = value;
                OnPropertyChanged(nameof(SourceDirectoryName));
            }
        }
        private string _SourceFileSize;
        public string SourceFileSize
        {
            get
            {
                return _SourceFileSize;
            }
            set
            {
                _SourceFileSize = value;
                OnPropertyChanged(nameof(SourceFileSize));
            }
        }
        private string _SourceCreated;
        public string SourceCreated
        {
            get
            {
                return _SourceCreated;
            }
            set
            {
                _SourceCreated = value;
                OnPropertyChanged(nameof(SourceCreated));
            }
        }

        private BitmapSource _SourceFileIcon;
        public BitmapSource SourceFileIcon
        {
            get
            {
                return _SourceFileIcon;
            }
            set
            {
                _SourceFileIcon = value;
                OnPropertyChanged(nameof(SourceFileIcon));
            }
        }

        private string _DestinationFileName;
        public string DestinationFileName
        {
            get
            {
                return _DestinationFileName;
            }
            set
            {
                _DestinationFileName = value;
                OnPropertyChanged(nameof(DestinationFileName));
            }
        }

        private string _DestinationDirectoryName;
        public string DestinationDirectoryName
        {
            get
            {
                return _DestinationDirectoryName;
            }
            set
            {
                _DestinationDirectoryName = value;
                OnPropertyChanged(nameof(DestinationDirectoryName));
            }
        }
        private string _DestinationFileSize;
        public string DestinationFileSize
        {
            get
            {
                return _DestinationFileSize;
            }
            set
            {
                _DestinationFileSize = value;
                OnPropertyChanged(nameof(DestinationFileSize));
            }
        }
        private string _DestinationCreated;
        public string DestinationCreated
        {
            get
            {
                return _DestinationCreated;
            }
            set
            {
                _DestinationCreated = value;
                OnPropertyChanged(nameof(DestinationCreated));
            }
        }

        private BitmapSource _DestinationFileIcon;
        public BitmapSource DestinationFileIcon
        {
            get
            {
                return _DestinationFileIcon;
            }
            set
            {
                _DestinationFileIcon = value;
                OnPropertyChanged(nameof(DestinationFileIcon));
            }
        }
        #endregion

        public ICommand YesCommand { get; }
        public ICommand YesToAllCommand { get; }
        public ICommand NoCommand { get; }
        public ICommand NoToAllCommand { get; }
        public ICommand KeepBothCommand { get; }
        public ICommand CancelCommand { get; }

        public FileCompareViewModel(ModalNavSvc modal, DialogStore store)
        {
            YesCommand = new YesCommand(modal, store);
            //ignore this jank way i decided to do this
            string SourceFile = store.Title;
            string DestinationFile = store.Message;
            if (File.Exists(SourceFile))
            {
                
                using (Icon ico = Icon.ExtractAssociatedIcon(SourceFile))
                {
                    SourceFileIcon = Imaging.CreateBitmapSourceFromHIcon(ico.Handle, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
                    SourceFileIcon.Freeze();
                }
                SourceFileName = Path.GetFileName(SourceFile);
                SourceDirectoryName = Directory.GetParent(SourceFile).ToString();
                SourceFileSize = "Size: " + FileSizeUnit(SourceFile);
                SourceCreated = "Date Created: " + File.GetCreationTime(SourceFile).ToString();
            }
            if (File.Exists(DestinationFile))
            {
                
                using (Icon ico = Icon.ExtractAssociatedIcon(DestinationFile))
                {
                    DestinationFileIcon = Imaging.CreateBitmapSourceFromHIcon(ico.Handle, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
                    DestinationFileIcon.Freeze();
                }
                DestinationFileName = Path.GetFileName(DestinationFile);
                DestinationDirectoryName = Directory.GetParent(DestinationFile).ToString();
                DestinationFileSize = "Size: " + FileSizeUnit(DestinationFile);
                DestinationCreated = "Date Created: " + File.GetCreationTime(DestinationFile).ToString();
            }
        }

        public override void Dispose()
        {
            base.Dispose();
        }
    }
}
