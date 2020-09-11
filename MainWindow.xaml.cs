using Microsoft.Win32;
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

        private void RefreshListViews()
        {
            //refreshes all list views so it reflects any changes made to lists
            listViewSourceFiles.Items.Refresh();
            listViewDirectories.Items.Refresh();
            listViewDestination.Items.Refresh();
        }
        void wb_LoadCompleted(object sender, NavigationEventArgs e)
        {
            string script = "document.body.style.overflow ='auto'";
            WebBrowser wb = (WebBrowser)sender;
            wb.InvokeScript("execScript", new Object[] { script, "JavaScript" });
        }

        private void Load(object sender, RoutedEventArgs e)
        { 
            
        }

        private void Save(object sender, RoutedEventArgs e)
        {

        }

        private void Exit(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Are you sure you want to exit?", "Exit",MessageBoxButton.OKCancel,MessageBoxImage.Warning,MessageBoxResult.Cancel);
            if(result == MessageBoxResult.OK)
                Application.Current.Shutdown();
            
        }

        private void About(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Soup Mover V0.1\nSoup Mover is a program for moving files to various folders. Made by MrSoupman.", "About", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void Reset(object sender, RoutedEventArgs e)
        {

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
                    //If we get a valid directory to drop things in, we need to create a new directory, as well as a list to hold what files go to that directory
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
                }
                RefreshListViews();
            }
        }

        private void listViewDirectories_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            //MessageBox.Show(e.AddedItems[0].ToString());
            int index = listDirectories.IndexOf(e.AddedItems[0].ToString());
            listViewDestination.ItemsSource = listDestination[index];
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
