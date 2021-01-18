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
        private static Mutex mutex = new Mutex();

        private void Debug(object sender, RoutedEventArgs e)
        {
            //FileCompare compare = new FileCompare();
            //compare.ShowDialog();
        }
        private void UpdateProgress()
        {
            txtProg.Text = (intCurrentFile + "/" + intTotalFiles);
            if (intTotalFiles != 0) //edge case
                pb.Value = Convert.ToInt32(((double)intCurrentFile / intTotalFiles) * 100.0);
            else
                pb.Value = 0;
        }

        private void RefreshListViews()
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
                int index = directories.IndexOf(new FilesToMove(e.AddedItems[0].ToString()));
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
            bool OverwriteAll = false;
            bool NoToAll = false;
            for (int i = 0; i < directories.Count; i++) //we initially check for duplicate files or invalid directories
            {
                List<String> removedFiles = new List<string>();
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

                    /*
                    if (File.Exists(destination) && !OverwriteAll) //if file exists at destination and overwrite flag isn't set:
                    { 
                        worker.RunWorkerAsync(new Tuple<string, string, bool>(file, destination, true));
                    }
                    else if (!OverwriteAll && !NoToAll) //no data on either no to all or overwrite all
                    {

                        FileCompare compare = new FileCompare(file, destination);
                        compare.ShowDialog();
                        if (compare.RESULT == Result.YES)
                        {
                            worker.RunWorkerAsync(new Tuple<string, string, bool>(file, destination, true));
                        }
                        else if (compare.RESULT == Result.YESTOALL)
                        {
                            OverwriteAll = true;
                            worker.RunWorkerAsync(new Tuple<string, string, bool>(file, destination, true));
                        }
                        else if (compare.RESULT == Result.KEEPBOTH)
                        {
                            string ext = Path.GetExtension(file);
                            string filename = Path.GetFileNameWithoutExtension(file);
                            //First we need the amount of duplicates that are in the folder
                            //Since we'll be off by one due to the original file not including a (x), we add one
                            int offset = (Directory.GetFiles(directories[i].GetDirectory(), filename + " (?)" + ext)).Length + 2;
                            destination = directories[i].GetDirectory() + "\\" + Path.GetFileNameWithoutExtension(file) + " (" + offset + ")" + ext;
                            worker.RunWorkerAsync(new Tuple<string, string, bool>(file, destination, false));

                        }
                        else if (compare.RESULT == Result.NOTOALL)
                        {
                            NoToAll = true;
                            listSourceFiles.Add(file);
                        }
                        else if (compare.RESULT == Result.CANCEL)
                        {
                            worker.CancelAsync();
                            Thread.Sleep(1000);
                            return; //user clicked on cancel, cease all actions
                        }
                        else
                        {
                            listSourceFiles.Add(file);
                        }
                    */
                    }

                    intCurrentFile++;
                    UpdateProgress();
                }
                directories[i].Clear(); //clear the i-th list of destinations since we're done with them

            }
        }

        private void Worker_DoWork(object sender, DoWorkEventArgs e)
        {
            Tuple<string,string,bool> args = e.Argument as Tuple<string, string, bool>;
            string file = args.Item1;
            string destination = args.Item2;
            bool overwrite = args.Item3;
            File.Move(file, destination, overwrite);

            (sender as BackgroundWorker).ReportProgress(1);
            Thread.Sleep(300);

        }

        private void Worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            /**
             * Passed in args list:
             * >0 the actual progress
             * -1 duplicate file detected in directory
             * -2 source file does not exist
             */
            UpdateProgress();
            if (((int)e.ProgressPercentage) > 0)
                UpdateProgress();
            else if()
        }

        public MainWindow()
        {
            InitializeComponent();
            listViewSourceFiles.ItemsSource = listSourceFiles; //binds List source files to the list view
            listViewDirectories.ItemsSource = directories;
        }

        
    }
}
