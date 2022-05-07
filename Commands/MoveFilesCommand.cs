using SoupMover.Models;
using SoupMover.Services;
using SoupMover.Stores;
using SoupMover.ViewModels;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace SoupMover.Commands
{
    public class MoveFilesCommand : BaseCommand
    {
        private readonly HomeViewModel HVM;
        private readonly List<DestinationPathViewModel> Directories;
        private readonly ModalNavSvc modal;
        private readonly DialogStore dialog;
        private TaskCompletionSource<int> result;
        private BackgroundWorker Worker;
        private List<string> SkippedFiles;
        private enum CompareResult { Yes, YesToAll, No, NoToAll, KeepBoth, Cancel }

        public override async void Execute(object parameter)
        {
            var res = MessageBox.Show("Move files?", "Confirm", MessageBoxButton.OKCancel, MessageBoxImage.Question);
            if (res == MessageBoxResult.OK)
            {
                HVM.IsMoving = true;
                //Prep work - Checking that the source file exists, and does not exist in the directory it should be moved to
                HVM.ClearDestinationListView();
                HVM.ResetIndices();
                SkippedFiles.Clear();
                bool NoToAll = false, YesToAll = false;
                foreach (DestinationPathViewModel path in Directories)
                {
                    List<ModFile> files = path.GetFiles();
                    foreach (ModFile file in files)
                    {
                        if (!File.Exists(file.FileName)) //need to check if file exists in first place
                            continue;
                        string Destination = path.Path;
                        if (File.Exists(Destination + Path.DirectorySeparatorChar + Path.GetFileName(file.FileName))) //File exists in destination it should be moved to
                        {
                            if (NoToAll) //user specified to skip all conflicting entries
                                file.ToSkip = true;
                            else if (!YesToAll) //user hasn't specified to overwrite all or skip all; get user response
                            {
                                result = new TaskCompletionSource<int>();
                                dialog.Title = file.FileName;
                                dialog.Message = Destination + Path.DirectorySeparatorChar + Path.GetFileName(file.FileName);
                                modal.Navigate();
                                await result.Task; //wait until we get user's response

                                switch (result.Task.Result)
                                {
                                    case (int)CompareResult.Yes:
                                        //File.Move(file.FileName, Destination + Path.DirectorySeparatorChar + Path.GetFileName(file.FileName), true);
                                        file.OverwriteExist = true;
                                        break;
                                    case (int)CompareResult.YesToAll:
                                        YesToAll = true;
                                        goto case (int)CompareResult.Yes;
                                    case (int)CompareResult.No:
                                        file.ToSkip = true;
                                        break;
                                    case (int)CompareResult.NoToAll:
                                        NoToAll = true;
                                        goto case (int)CompareResult.No;
                                    case (int)CompareResult.KeepBoth:
                                        string ext = Path.GetExtension(file.FileName);
                                        string filename = Path.GetFileNameWithoutExtension(file.FileName);
                                        //First we need the amount of duplicates that are in the folder
                                        //Since we'll be off by one due to the original file not including a (x), we add one
                                        int offset = Directory.GetFiles(path.Path, filename + " (?)" + ext).Length + 2;
                                        file.NewName = filename + " (" + offset + ")" + ext;
                                        //File.Move(file.FileName, Destination + Path.DirectorySeparatorChar + file.NewName);
                                        break;
                                    case (int)CompareResult.Cancel:
                                        return;
                                }
                            }
                            else
                            {
                                file.OverwriteExist = true;
                                //File.Move(file.FileName, Destination + Path.DirectorySeparatorChar + Path.GetFileName(file.FileName), true);
                            }
                        }
                    }
                }
                Worker.RunWorkerAsync(); //Now we actually move the files
            }
        }

        public override bool CanExecute(object parameter)
        {
            return HVM.TotalCount > 0;
        }

        public MoveFilesCommand(HomeViewModel HVM, List<DestinationPathViewModel> Directories, ModalNavSvc modal, DialogStore dialog)
        {
            this.HVM = HVM;
            this.HVM.PropertyChanged += HVM_PropertyChanged;
            this.Directories = Directories;
            this.modal = modal;
            this.dialog = dialog;
            this.dialog.FileCompareResult += dialog_FileCompareResult;
            Worker = new BackgroundWorker()
            {
                WorkerReportsProgress = true,
                WorkerSupportsCancellation = true
            };
            Worker.ProgressChanged += Worker_ProgressChanged;
            Worker.RunWorkerCompleted += Worker_RunWorkerCompleted;
            Worker.DoWork += Worker_DoWork;
            SkippedFiles = new List<string>();

        }

        private void Worker_DoWork(object sender, DoWorkEventArgs e)
        {
            foreach (DestinationPathViewModel path in Directories)
            {
                List<ModFile> files = path.GetFiles();
                string dir = path.Path + Path.DirectorySeparatorChar;
                foreach (ModFile file in files)
                {
                    if (Worker.CancellationPending)
                        e.Cancel = true;
                    if (!file.ToSkip) //first check if we're skipping the file
                    {
                        if (file.OverwriteExist) //check if this is a file set to overwrite
                            File.Move(file.FileName, dir + Path.GetFileName(file.FileName), true);
                        else if (!string.IsNullOrEmpty(file.NewName)) //if this is a keep both situation
                            File.Move(file.FileName, dir + Path.GetFileName(file.NewName));
                        else
                            File.Move(file.FileName, dir + Path.GetFileName(file.FileName)); // i don't actually think this part is necessary
                    }
                    else
                        SkippedFiles.Add(file.FileName);
                    HVM.CurrentCount += 1;
                    (sender as BackgroundWorker).ReportProgress((int)((HVM.CurrentCount / (double)HVM.TotalCount) * 100));
                    Thread.Sleep(300);

                }
                path.Clear();
            }
        }

        private void Worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            HVM.IsMoving = false;
            HVM.CancelMoving = false;
            HVM.TotalCount = 0;
            HVM.CurrentCount = 0;
            foreach (string file in SkippedFiles)
                HVM.AddToSourceFiles(file);
            MessageBox.Show("Completed.");
        }

        private void Worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            HVM.Progress = e.ProgressPercentage;
        }

        private void dialog_FileCompareResult(int obj)
        {
            result?.TrySetResult(obj);
        }

        private void HVM_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(HomeViewModel.TotalCount))
                OnCanExecutedChanged();
            else if (e.PropertyName == nameof(HomeViewModel.CancelMoving)) //The only time this fires is when cancel is used (and when we turn it off)
            {
                if (Worker.IsBusy)
                {
                    Worker.CancelAsync();

                }

            }
        }
    }
}
