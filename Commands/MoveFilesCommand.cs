using SoupMover.Models;
using SoupMover.Services;
using SoupMover.Stores;
using SoupMover.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace SoupMover.Commands
{
    public class MoveFilesCommand : BaseCommand
    {
        private readonly HomeViewModel HVM;
        private readonly ObservableCollection<DestinationPathViewModel> Directories;
        private readonly ModalNavSvc modal;
        private readonly DialogStore dialog;
        private TaskCompletionSource<int> result;
        private BackgroundWorker worker;
        private enum CompareResult {Yes, YesToAll, No, NoToAll, KeepBoth, Cancel }

        public override void Execute(object parameter)
        {
            var res = MessageBox.Show("Move files?", "Confirm", MessageBoxButton.OKCancel, MessageBoxImage.Question);
            if (res == MessageBoxResult.OK)
            {
                worker.RunWorkerAsync();
            }
            
            
            
        }

        public override bool CanExecute(object parameter)
        {
            return HVM.TotalCount > 0;
        }

        public MoveFilesCommand(HomeViewModel HVM, ObservableCollection<DestinationPathViewModel> Directories, ModalNavSvc modal, DialogStore dialog)
        {
            this.HVM = HVM;
            this.HVM.PropertyChanged += HVM_PropertyChanged;
            this.Directories = Directories;
            this.modal = modal;
            this.dialog = dialog;
            this.dialog.FileCompareResult += dialog_FileCompareResult;
            worker = new BackgroundWorker()
            {
                WorkerSupportsCancellation = true,
                WorkerReportsProgress = true
            };
            worker.DoWork += Worker_DoWork;
            worker.ProgressChanged += Worker_ProgressChanged;
            
        }

        private void Worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            HVM.Progress = e.ProgressPercentage;
        }

        private async void Worker_DoWork(object sender, DoWorkEventArgs e)
        {
            bool NoToAll = false, YesToAll = false;
            foreach (DestinationPathViewModel path in Directories)
            {
                List<ModFile> files = path.GetFiles();
                foreach (ModFile file in files)
                {
                    if (worker.CancellationPending)
                        e.Cancel = true;
                    string Destination = path.Path;
                    if (File.Exists(Destination + Path.DirectorySeparatorChar + Path.GetFileName(file.FileName)) && !NoToAll && !YesToAll) //File exists in destination it should be moved to
                    {
                        if (!YesToAll) //user hasn't specified to overwrite all
                        {
                            result = new TaskCompletionSource<int>();
                            dialog.Title = file.FileName;
                            dialog.Message = Destination + Path.DirectorySeparatorChar + Path.GetFileName(file.FileName);
                            modal.Navigate();
                            await result.Task;

                            switch (result.Task.Result)
                            {
                                case (int)CompareResult.Yes:
                                    File.Move(file.FileName, Destination, true);
                                    break;
                                case (int)CompareResult.YesToAll:
                                    YesToAll = true;
                                    goto case (int)CompareResult.Yes;
                                case (int)CompareResult.No:
                                    //TODO: add to sourcefiles here
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
                                    File.Move(file.FileName, Destination + Path.DirectorySeparatorChar + file.NewName);
                                    break;
                                case (int)CompareResult.Cancel:
                                    worker.CancelAsync();
                                    return;
                            }
                        }
                        else
                            File.Move(file.FileName, Destination, true);


                    } 
                    else if (!NoToAll) //File doesn't already exist, and user hasn't said no to all
                    {
                        File.Move(file.FileName, Destination);
                    }
                    Thread.Sleep(300);
                    //increase progress
                    HVM.CurrentCount += 1;
                    int prog = (int)(((double)HVM.CurrentCount / (double)HVM.TotalCount) * 100);
                    worker.ReportProgress(prog);

                }
            }
        }

        private void dialog_FileCompareResult(int obj)
        {
            result?.TrySetResult(obj);
        }

        private void HVM_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(HomeViewModel.TotalCount))
                OnCanExecutedChanged();
        }
    }
}
