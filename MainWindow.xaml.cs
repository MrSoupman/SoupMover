using Microsoft.Win32;
using SoupMover.FileWindow;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml;
using System.Threading;
using Path = System.IO.Path;

namespace SoupMover
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        List<string> listSourceFiles = new List<string>(); //list to store all source files
        List<FilesToMove> directories = new List<FilesToMove>(); //list to store all files to move
        int intCurrentFile = 0,intTotalFiles = 0; //current file tracks which file is being moved, total tracks all files

        private void Debug(object sender, RoutedEventArgs e)
        {
            //FileCompare compare = new FileCompare();
            //compare.ShowDialog();
        }
        private void DisableButtons()
        {
            btnAddFiles.IsEnabled = false;
            btnRemoveFiles.IsEnabled = false;
            btnMoveTo.IsEnabled = false;
            btnMoveFrom.IsEnabled = false;
            btnAddDirectory.IsEnabled = false;
            btnRemoveDirectory.IsEnabled = false;
            btnMove.IsEnabled = false;
            btnUndo.IsEnabled = false;
        }

        private void EnableButtons()
        {
            btnAddFiles.IsEnabled = true;
            btnRemoveFiles.IsEnabled = true;
            btnMoveTo.IsEnabled = true;
            btnMoveFrom.IsEnabled = true;
            btnAddDirectory.IsEnabled = true;
            btnRemoveDirectory.IsEnabled = true;
            btnMove.IsEnabled = true;
            btnUndo.IsEnabled = true;
        }
        private void UpdateProgress()
        {
            txtProg.Text = (intCurrentFile + "/" + intTotalFiles);
            if (intTotalFiles != 0) //edge case
                pb.Value = Convert.ToInt32(((double)intCurrentFile / intTotalFiles) * 100.0);
            else
                pb.Value = 0;
        }

        private void RefreshListViews() //TODO: Replace List with ObservableList, and remove this method
        {
            //refreshes all list views so it reflects any changes made to lists
            listViewSourceFiles.Items.Refresh();
            listViewDirectories.Items.Refresh();
            listViewDestination.Items.Refresh();
        }
        void wb_LoadCompleted(object sender, NavigationEventArgs e)
        {
            //hides scroll bar in webview
            string script = "document.body.style.overflow ='auto'";
            WebBrowser wb = (WebBrowser)sender;
            wb.InvokeScript("execScript", new Object[] { script, "JavaScript" });
        }

        private void Load(object sender, RoutedEventArgs e)
        {
            int index = 0;
            OpenFileDialog open = new OpenFileDialog();
            open.Multiselect = false;
            open.Filter = "XML file (*.xml)|*.xml";
            if (open.ShowDialog() == true)
            {
                Reset(sender,e); //resets the window to prepare loading
                XmlDocument xml = new XmlDocument();
                xml.Load(open.FileName);
                foreach (XmlNode node in xml.DocumentElement.ChildNodes[0].ChildNodes) //adds sourcefiles back to source list
                    listSourceFiles.Add(node.InnerText);

                foreach (XmlNode node in xml.DocumentElement.ChildNodes[1].ChildNodes) //adds directories, and all, if any, files back to the correct directory
                {
                    directories.Add(new FilesToMove(node.Attributes["dir"].Value));
                    foreach (XmlNode file in xml.DocumentElement.ChildNodes[1].ChildNodes[index])
                    { 
                        directories[index].Add(file.InnerText);
                        intTotalFiles++;
                    }

                    index++;
                }
                UpdateProgress();
                RefreshListViews();
            }
        }

        private void Save(object sender, RoutedEventArgs e)
        {
            int index = 0;
            SaveFileDialog save = new SaveFileDialog();
            save.Filter = "XML file (*.xml)|*.xml";
            if (save.ShowDialog() == true)
            {
                XmlWriter xml = XmlWriter.Create(save.FileName);
                xml.WriteStartDocument();
                xml.WriteStartElement("root");

                xml.WriteStartElement("SourceFiles");
                foreach (string filename in listSourceFiles)
                {
                    xml.WriteStartElement("file");
                    xml.WriteString(filename);
                    xml.WriteEndElement();
                }
                xml.WriteEndElement(); //end of SourceFiles

                xml.WriteStartElement("Directories");
                foreach (FilesToMove directory in directories)
                {
                    xml.WriteStartElement("Directory");
                    xml.WriteAttributeString("dir", directory.GetDirectory());
                    foreach (string filename in directories[index].GetFiles())
                    {
                        xml.WriteStartElement("file");
                        xml.WriteString(filename);
                        xml.WriteEndElement();
                    }
                    index++;
                    xml.WriteEndElement(); //end of files to move

                }
                xml.WriteEndElement(); //end of directories

                xml.WriteEndElement(); //end of root
                xml.WriteEndDocument();
                xml.Close();
                MessageBox.Show("Successfully saved list to " + save.FileName, "Success",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void Exit(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Are you sure you want to exit?", "Exit",
                MessageBoxButton.OKCancel,MessageBoxImage.Warning,MessageBoxResult.Cancel);
            if(result == MessageBoxResult.OK)
                Application.Current.Shutdown();
            
        }

        private void About(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Soup Mover V0.4\nSoup Mover is a program for moving files to various folders. " +
                "Made by MrSoupman.", "About", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void Reset(object sender, RoutedEventArgs e)
        {
            listSourceFiles.Clear();
            directories.Clear();
            RefreshListViews();
            listViewDestination.ItemsSource = null;
            pb.Value = 0;
            intCurrentFile = 0;
            intTotalFiles = 0;
            UpdateProgress();
        }

        private void GithubPage(object sender, RoutedEventArgs e)
        {

        }

        private void AddFiles(object sender, RoutedEventArgs e)
        {
            OpenFileDialog open = new OpenFileDialog();
            open.Multiselect = true;
            if (open.ShowDialog() == true)
            {
                foreach (string filename in open.FileNames)
                    listSourceFiles.Add(filename);
                RefreshListViews();
            }
            
        }

        private void RemoveFiles(object sender, RoutedEventArgs e)
        {
            if (listViewSourceFiles.SelectedItems != null)
            {
                foreach (string file in listViewSourceFiles.SelectedItems)
                    listSourceFiles.Remove(file);
                RefreshListViews();
            }    
        }

        private void AddDirectories(object sender, RoutedEventArgs e)
        {
            using (var dialog = new System.Windows.Forms.FolderBrowserDialog())
            {
                System.Windows.Forms.DialogResult result = dialog.ShowDialog(); //WPF .netcore appearently doesn't have a built in directory browser????? for like 7 years now??? ok
                if (result == System.Windows.Forms.DialogResult.OK && !directories.Contains(new FilesToMove(dialog.SelectedPath)))
                {
                    //If we get a valid directory to drop things in, we need to create a new directory, 
                    //as well as a list to hold what files go to that directory
                    directories.Add(new FilesToMove(dialog.SelectedPath));
                    RefreshListViews();
                }

            }
        }

        private void RemoveDirectories(object sender, RoutedEventArgs e)
        {
            if (listViewDirectories.SelectedItem != null)
            {
                int index = listViewDirectories.SelectedIndex;
                if (directories[index].IsEmpty()) //directory contains no items to be moved to it
                {
                    directories.RemoveAt(index);
                    RefreshListViews();
                }
                else
                {
                    MessageBoxResult result = MessageBox.Show("There are files to be moved to this destination. Do you still want to remove this directory? All files will be returned to the source files list.", "Remove Directory?", MessageBoxButton.YesNo, MessageBoxImage.Warning, MessageBoxResult.No);
                    if (result == MessageBoxResult.Yes)
                    {
                        foreach (string files in directories[index].GetFiles()) //need to add each file back into source list
                        {
                            listSourceFiles.Add(files);
                        }
                        directories.RemoveAt(index);
                        listViewDestination.ItemsSource = null; //Used to reset listViewDestination, clear is for when you add items directly to the LV
                        RefreshListViews();
                    }
                }

            }
        }

        private void MoveToDestination(object sender, RoutedEventArgs e)
        {
            if (listViewSourceFiles.SelectedItems != null && listViewDirectories.SelectedItem != null)
            {
                foreach (string file in listViewSourceFiles.SelectedItems)
                {
                    directories[listViewDirectories.SelectedIndex].Add(file);
                    listSourceFiles.Remove(file);
                    intTotalFiles++;
                    UpdateProgress();
                }
                listViewDestination.ItemsSource = directories[listViewDirectories.SelectedIndex].GetFiles();
                RefreshListViews();
            }
        }

        private void MoveToSource(object sender, RoutedEventArgs e)
        {
            if (listViewDestination.SelectedItems != null && listViewDirectories.SelectedItem != null)
            {
                foreach (string file in listViewDestination.SelectedItems)
                {
                    listSourceFiles.Add(file);
                    directories[listViewDirectories.SelectedIndex].Remove(file);
                    intTotalFiles--;
                    UpdateProgress();
                }
                RefreshListViews();
            }
        }

        private void listViewDirectories_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(e.AddedItems.Count > 0)
            {
                FilesToMove ftm = new FilesToMove(e.AddedItems[0].ToString());
                int index = directories.IndexOf(ftm);
                listViewDestination.ItemsSource = directories[index].GetFiles();
                RefreshListViews();
            }
            
        }

        private void MoveHandler(object sender, RoutedEventArgs e)
        {
            if (directories.Count == 0) //TODO: Refactor so any and all GUI calls are handled elsewhere
            {
                MessageBox.Show("No files to move.");
                return;
            }

            MessageBoxResult result = MessageBox.Show("Are you sure you want to move the files?", "Move all files?", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No);
            if (result == MessageBoxResult.No)
                return;
            BackgroundWorker worker = new BackgroundWorker();
            worker.WorkerReportsProgress = true;
            worker.DoWork += Worker_DoWork;
            worker.ProgressChanged += Worker_ProgressChanged;
            worker.RunWorkerCompleted += Worker_RunWorkerCompleted;
            bool OverwriteAll = false;
            bool NoToAll = false;
            for (int i = 0; i < directories.Count; i++) //we initially check for duplicate files or invalid directories
            {
                List<string> removedFiles = new List<string>();
                if (!Directory.Exists(directories[i].GetDirectory()))
                {
                    MessageBox.Show("Error: directory " + directories[i].GetDirectory() + " not found. Was this from an old save?", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    intCurrentFile += directories[i].Count(); //since the directory straight up does not exist, add all the files to the progress and move on
                    directories[i].Clear();
                    continue;
                }
                foreach (string file in directories[i].GetFiles())
                {
                    if (!File.Exists(file))
                    {
                        MessageBox.Show("Error: source file " + file + " not found. Was this from an old save?", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        removedFiles.Add(file);
                        continue;
                    }
                    string destination = directories[i].GetDirectory() + "\\" + Path.GetFileName(file);
                    if (File.Exists(destination) && !OverwriteAll) //file already exists at destination, and no overwrite flag set
                    {
                        FileCompare compare = new FileCompare(file, destination);
                        compare.ShowDialog();
                        if (compare.RESULT == Result.YESTOALL)
                            OverwriteAll = true;
                        else if (compare.RESULT == Result.NO)
                            removedFiles.Add(file);
                        else if (compare.RESULT == Result.NOTOALL)
                        {
                            NoToAll = true;
                            removedFiles.Add(file);
                        }
                        else if (compare.RESULT == Result.KEEPBOTH)
                        {
                            string ext = Path.GetExtension(file);
                            string filename = Path.GetFileNameWithoutExtension(file);
                            //First we need the amount of duplicates that are in the folder
                            //Since we'll be off by one due to the original file not including a (x), we add one
                            int offset = (Directory.GetFiles(directories[i].GetDirectory(), filename + " (?)" + ext)).Length + 2;
                            destination = directories[i].GetDirectory() + "\\" + Path.GetFileNameWithoutExtension(file) + " (" + offset + ")" + ext;
                            File.Move(file, Path.GetFullPath(file) + Path.GetFileName(destination)); //Renames file
                            if (!directories[i].UpdateFileName(file, destination)) //Attempts to update the file name within the db
                            {
                                MessageBox.Show("Unexpected Error, something went wrong trying to keep both files. It may not exist.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                                return;
                            }
                        }
                        else if (compare.RESULT == Result.CANCEL)
                            return;

                    }
                    else if (File.Exists(destination) && NoToAll)
                    {
                        removedFiles.Add(file);
                    }
                }

            }
            DisableButtons();
            worker.RunWorkerAsync(intTotalFiles);
        }

        private void Worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                string logName = Directory.GetCurrentDirectory() + "\\" + DateTime.Now.ToString("MM-dd-yy-hhmmss") + ".txt";
                if (!File.Exists(logName))
                {
                    File.Create(logName).Dispose();
                }
                using (StreamWriter writer = File.AppendText(logName))
                {
                    writer.WriteLine("Exception Dump on " + DateTime.Now.ToString("MM-dd-yy-hhmmss"));
                    writer.WriteLine("----------------------------------------\n");
                    writer.WriteLine(e.Error);
                    writer.WriteLine("----------------------------------------");
                    writer.WriteLine("End of dump.");
                }
                MessageBox.Show("An error has occurred while moving files. An exception log has been created where this program exists.","Error",MessageBoxButton.OK,MessageBoxImage.Error);
            }
            bool finished = true;
            foreach (FilesToMove dirs in directories)
            {
                MessageBoxResult result = MessageBoxResult.None;
                if (dirs.Count() != 0 && finished == true)
                {
                    result = MessageBox.Show("Some files could not be moved successfully. Would you like to keep the files and try again? (Selecting No will return them back to the source list.)", "Retry?", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No);
                    finished = false;
                }
                else if (dirs.Count() != 0 && result == MessageBoxResult.No)
                {
                    foreach (string file in dirs.GetFiles())
                        listSourceFiles.Add(file);
                }
            }
            MessageBox.Show("All files moved.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            EnableButtons();
        }

        private void Worker_DoWork(object sender, DoWorkEventArgs e)
        {
           
            int totalFiles = (int)e.Argument, currentFiles = 0;
            for (int i = 0; i < directories.Count; i++)
            {
                List<string> removedFiles = new List<string>();
                for (int j = 0; j < directories[i].Count(); j++)
                {
                    string file = directories[i].GetFile(j); //The file to move
                    string destination = directories[i].GetDirectory() + "\\" + Path.GetFileName(directories[i].GetFile(j)); //The destination folder
                    if (File.Exists(file) && !File.Exists(destination)) //to be ABSOLUTELY CERTAIN, once we get to this file it exists in the source directory and not in the destination folder as well
                    {
                        try
                        {
                            File.Move(file, destination);
                            removedFiles.Add(file);
                        }
                        catch (Exception exc)
                        {
                            throw exc;
                        }
                        currentFiles++;
                        int prog = Convert.ToInt32(((double)currentFiles / (double)totalFiles) * 100);
                        (sender as BackgroundWorker).ReportProgress(prog, currentFiles); //pass along the actual progress as well as the numerical amount of files that have been moved
                        Thread.Sleep(100);
                    }
                }
                foreach (string removed in removedFiles)
                    directories[i].Remove(removed);
            }

        }

        private void Worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            pb.Value = e.ProgressPercentage;
            txtProg.Text = e.UserState.ToString() + "/" + intTotalFiles;
            
        }


        public MainWindow()
        {
            InitializeComponent();
            listViewSourceFiles.ItemsSource = listSourceFiles; //binds List source files to the list view
            listViewDirectories.ItemsSource = directories;
        }

        
    }
}
