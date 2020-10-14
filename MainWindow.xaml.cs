using Microsoft.Win32;
using SoupMover.FileWindow;
using System;
using System.Collections.Generic;
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
using Path = System.IO.Path;

namespace SoupMover
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        List<string> listSourceFiles = new List<string>(); //list to store all source files
        List<string> listDirectories = new List<string>(); //stores the directories where we can move files
        List<List<string>> listDestination = new List<List<string>>(); //stores the files per directories
        int intCurrentFile = 0,intTotalFiles = 0; //current file tracks which file is being moved, total tracks all files
        Boolean moveLock = false;

        private void Debug(object sender, RoutedEventArgs e)
        {
            //FileCompare compare = new FileCompare();
            //compare.ShowDialog();
        }
        private void UpdateProgress()
        {
            txtProg.Text = (intCurrentFile + "/" + intTotalFiles);
            if (intTotalFiles != 0) //edge case
                pb.Value = (double)(intCurrentFile / intTotalFiles) * 100.0;
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
                Reset(sender,e);
                XmlDocument xml = new XmlDocument();
                xml.Load(open.FileName);
                foreach (XmlNode node in xml.DocumentElement.ChildNodes[0].ChildNodes) //adds sourcefiles back to source list
                    listSourceFiles.Add(node.InnerText);

                foreach (XmlNode node in xml.DocumentElement.ChildNodes[1].ChildNodes)
                {
                    listDirectories.Add(node.Attributes["dir"].Value);
                    listDestination.Add(new List<string>());
                    foreach (XmlNode file in xml.DocumentElement.ChildNodes[1].ChildNodes[index])
                    { 
                        listDestination[index].Add(file.InnerText);
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
                foreach (string directory in listDirectories)
                {
                    xml.WriteStartElement("Directory");
                    xml.WriteAttributeString("dir", directory);
                    foreach (string filename in listDestination[index])
                    {
                        xml.WriteStartElement("file");
                        xml.WriteString(filename);
                        xml.WriteEndElement();
                    }
                    index++;
                    xml.WriteEndElement();

                }
                xml.WriteEndElement();

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
            MessageBox.Show("Soup Mover V0.1\nSoup Mover is a program for moving files to various folders. " +
                "Made by MrSoupman.", "About", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void Reset(object sender, RoutedEventArgs e)
        {
            listSourceFiles.Clear();
            listDestination.Clear();
            listDirectories.Clear();
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
                if (result == System.Windows.Forms.DialogResult.OK && !listDirectories.Contains(dialog.SelectedPath))
                {
                    //If we get a valid directory to drop things in, we need to create a new directory, 
                    //as well as a list to hold what files go to that directory
                    listDirectories.Add(dialog.SelectedPath);
                    listDestination.Add(new List<string>());
                    RefreshListViews();
                }

            }
        }

        private void RemoveDirectories(object sender, RoutedEventArgs e)
        {
            if (listViewDirectories.SelectedItem != null)
            {
                
                int intIndex = listViewDirectories.SelectedIndex;
                if (listDestination[intIndex].Count == 0) //directory contains no items to be moved to it
                {
                    listDirectories.RemoveAt(intIndex);
                    listDestination.RemoveAt(intIndex);
                    RefreshListViews();
                }
                else if(listDestination[intIndex].Count != 0)
                {
                    MessageBoxResult result = MessageBox.Show("There are files to be moved to this destination. Do you still want to remove this directory? All files will be returned to the source files list.", "Remove Directory?", MessageBoxButton.YesNo, MessageBoxImage.Warning, MessageBoxResult.No);
                    if (result == MessageBoxResult.Yes)
                    {
                        foreach (string files in listDestination[intIndex]) //need to add each file back into source list
                        {
                            listSourceFiles.Add(files);
                        }
                        listDestination.RemoveAt(intIndex);
                        listDirectories.RemoveAt(intIndex);
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
                    listDestination[listViewDirectories.SelectedIndex].Add(file);
                    listSourceFiles.Remove(file);
                    intTotalFiles++;
                    UpdateProgress();
                }
                listViewDestination.ItemsSource = listDestination[listViewDirectories.SelectedIndex];
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
                    listDestination[listViewDirectories.SelectedIndex].Remove(file);
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
                int index = listDirectories.IndexOf(e.AddedItems[0].ToString());
                listViewDestination.ItemsSource = listDestination[index];
                RefreshListViews();
            }
            
        }

        private void MoveFiles(object sender, RoutedEventArgs e)
        {
            //TODO: Get user confirmation to move files
            //Lock all buttons except for cancel until process is complete
            //Maybe show some feedback that program is moving a file? Would be nice for larger files
            //TestFinal.xml is broke yo
            //Might want to thread.sleep after a move is finished
            bool OverwriteAll = false;
            bool NoToAll = false;
            for (int i = 0; i < listDirectories.Count; i++)
            {
                if(!Directory.Exists(listDirectories[i]))
                {
                    MessageBox.Show("Error: directory " + listDirectories[i] + " not found. Was this from an old save?","Error",MessageBoxButton.OK,MessageBoxImage.Error);
                    intCurrentFile += listDestination[i].Count; //since the directory straight up does not exist, add all the files to the progress and move on
                    listDestination[i].Clear();
                    continue;
                }
                foreach (string file in listDestination[i]) 
                {
                    if (!File.Exists(file))
                    {
                        MessageBox.Show("Error: source file " + file + " not found. Was this from an old save?", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        intCurrentFile++;
                        UpdateProgress();
                        continue;
                    }
                    string destination = listDirectories[i] + "\\" + Path.GetFileName(file);
                    if (!File.Exists(destination) || OverwriteAll)
                    {
                        File.Move(file, destination,true);
                    }
                    else if (!OverwriteAll && !NoToAll) //no data on either no to all or overwrite all
                    {
                        FileCompare compare = new FileCompare(file, destination);
                        compare.ShowDialog();
                        if (compare.RESULT == Result.YES)
                        {
                            File.Move(file, destination, true);
                        }
                        else if (compare.RESULT == Result.YESTOALL)
                        {
                            OverwriteAll = true;
                            File.Move(file, destination, true);
                        }
                        else if (compare.RESULT == Result.KEEPBOTH)
                        {
                            string ext = Path.GetExtension(file);
                            string filename = Path.GetFileNameWithoutExtension(file);
                            //First we need the amount of duplicates that are in the folder
                            //Since we'll be off by one due to the original file not including a (x), we add one
                            int count = (Directory.GetFiles(listDirectories[i], filename + " (?)" + ext)).Length + 2;
                            destination = listDirectories[i] + "\\" + Path.GetFileNameWithoutExtension(file) + " (" + count + ")" + ext;
                            File.Move(file, destination);
                        }
                        else if (compare.RESULT == Result.NOTOALL)
                        {
                            NoToAll = true;
                            listSourceFiles.Add(file);
                        }
                        else if (compare.RESULT == Result.CANCEL)
                            return;
                        else
                        {
                            listSourceFiles.Add(file);
                        }
                    }
                    
                    intCurrentFile++;
                    UpdateProgress();
                }
                listDestination[i].Clear(); //clear the i-th list of destinations since we're done with them

            }
            intCurrentFile = 0;
            intTotalFiles = 0;
            RefreshListViews();
        }

        public MainWindow()
        {
            InitializeComponent();
            listViewSourceFiles.ItemsSource = listSourceFiles; //binds List source files to the list view
            listViewDirectories.ItemsSource = listDirectories;
        }

        
    }
}
