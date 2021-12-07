using Microsoft.Win32;
using SoupMover.FileWindow;
using SoupMover.Database;
using SoupMover.FTM;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Threading;
using System.Windows.Threading;
using Path = System.IO.Path;
using MimeTypes;
using LibVLCSharp.Shared;
using WpfAnimatedGif;

namespace SoupMover
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		Data db = new Data();
		BackgroundWorker worker = new BackgroundWorker();
		DispatcherTimer time = new DispatcherTimer();
		LibVLC lib;
		LibVLCSharp.Shared.MediaPlayer media;
        string SaveLocation = "";
		uint IntCurrentFile = 0;
		//TODO:Context menu for all listviews, Check for move/changes made before exiting, search bars for all list views
		private void Debug(object sender, RoutedEventArgs e)
		{
			
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
		}
		private void UpdateProgress()
		{
			txtProg.Text = (IntCurrentFile + "/" + db.IntTotalFiles);
			if (IntCurrentFile != 0) //edge case
				pb.Value = Convert.ToInt32(((double)IntCurrentFile / db.IntTotalFiles) * 100.0);
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

		private void Load(object sender, RoutedEventArgs e) //TODO: Check for valid json file (I.E does it fit the format we have)
		{
            OpenFileDialog open = new OpenFileDialog
            {
                Multiselect = false,
                Filter = "JSON file (*.json)|*.json"
            };
			if (SaveLocation != "")
				open.InitialDirectory = SaveLocation;
            if (open.ShowDialog() == true)
			{
				Reset();
				db.LoadJSon(open.FileName);
				SaveLocation = Directory.GetParent(open.FileName).ToString();
				UpdateProgress();
				RefreshListViews();
			}
		}

		private void Save(object sender, RoutedEventArgs e)
		{
			SaveFileDialog save = new SaveFileDialog();
			save.Filter = "json file (*.json)|*.json";
			if (SaveLocation != "")
				save.InitialDirectory = SaveLocation;
			if (save.ShowDialog() == true)
			{
				db.SaveJSon(save.FileName);
				SaveLocation = Directory.GetParent(save.FileName).ToString();
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
			MessageBox.Show("Soup Mover V0.9\nSoup Mover is a program for moving files to various folders. " +
				"Made by MrSoupman.\n Thanks to samuelneff for MimeTypeMap, Mono Company for their icons, Thomas Levesque for WpfAnimatedGif, and VLC for LibVLCSharp!", "About", MessageBoxButton.OK, MessageBoxImage.Information);
		}

		private void Reset()
		{
			//GUI
			RefreshListViews();
			listViewDestination.ItemsSource = null;
			pb.Value = 0;
			
			TextDirLbl.Text = "(No directory selected)";
			SearchBox.Text = "Search...";
			HidePreview();

			//DB
			db.ResetDB();
			IntCurrentFile = 0;

			UpdateProgress(); //have to do at the end b/c db holds info on current/total files

		}

		private void ResetHandler(object sender, RoutedEventArgs e)
		{
			Reset();
		}

		private void AddFiles(object sender, RoutedEventArgs e)
		{
			OpenFileDialog open = new OpenFileDialog();
			open.Multiselect = true;
			if (open.ShowDialog() == true)
			{
				foreach (string filename in open.FileNames)
					db.AddFileToSource(filename);
				RefreshListViews();
			}
			
		}

		private void RemoveFiles(object sender, RoutedEventArgs e)
		{
			if (listViewSourceFiles.SelectedItems != null && listViewSourceFiles.SelectedIndex != -1) //remove from source files list
			{
				foreach (string file in listViewSourceFiles.SelectedItems)
					db.RemoveFileFromSource(file);
				RefreshListViews();
				HidePreview();
			}
			else if (listViewDestination.SelectedItems != null && listViewDestination.SelectedIndex != -1) //remove from destination list
			{
				foreach (string file in listViewDestination.SelectedItems)
					db.RemoveFileFromDestination(file, listViewDirectories.SelectedItem.ToString());
				RefreshListViews();
				HidePreview();
			}
			UpdateProgress();
		}


		private void AddDirectories(object sender, RoutedEventArgs e)
		{
			using (var dialog = new System.Windows.Forms.FolderBrowserDialog())
			{
				System.Windows.Forms.DialogResult result = dialog.ShowDialog(); //WPF .netcore appearently doesn't have a built in directory browser????? for like 7 years now??? ok
				if (result == System.Windows.Forms.DialogResult.OK && dialog.SelectedPath.Length > 0)
				{
					if (db.AddDirectory(dialog.SelectedPath))
					{
						SearchBox.Text = "Search..."; //not sure if i should keep it this way, but we reset search prior to adding the directory, just in case.
						int index = db.GetIndexOfDirectory(dialog.SelectedPath);
						listViewDirectories.SelectedIndex = index;
						listViewDestination.ItemsSource = db.GetFilesFromDirectory(dialog.SelectedPath);
						TextDirLbl.Text = db.GetDirInfo(index).GetDirectory(); //This is pretty ugly, may refactor later
						RefreshListViews();
					}
				}

			}
		}
		
		private void AddDirectoriesRecursively(object sender, RoutedEventArgs e)
		{
			using (var dialog = new System.Windows.Forms.FolderBrowserDialog())
			{
				System.Windows.Forms.DialogResult result = dialog.ShowDialog();
				if (result == System.Windows.Forms.DialogResult.OK)
				{
					//RecursiveEnum(dialog.SelectedPath);
					try 
					{
						db.RecursiveDir(dialog.SelectedPath);
					}
					catch (UnauthorizedAccessException exc)
					{
						MessageBox.Show("Cannot access directory. The following is a message on why: \n" + exc.Message, "Error Accessing Folder", MessageBoxButton.OK, MessageBoxImage.Error);
						return;
					}
					catch (PathTooLongException exc)
					{
						MessageBox.Show("Path character limit exceeded, exiting...", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
						return;
					}
					RefreshListViews();
				}
			}
		}


		private void AddFilesRecursively(object sender, RoutedEventArgs e)
		{
			using (var dialog = new System.Windows.Forms.FolderBrowserDialog())
			{
				System.Windows.Forms.DialogResult result = dialog.ShowDialog();
				if (result == System.Windows.Forms.DialogResult.OK)
				{
					db.RecursiveFiles(dialog.SelectedPath);
					RefreshListViews();
				}
			}
		}

		private void RemoveDirectories(object sender, RoutedEventArgs e)
		{
			if (listViewDirectories.SelectedItem != null)
			{
				int index = db.GetIndexOfDirectory(listViewDirectories.SelectedItem.ToString());
				if (db.GetDirInfo(index).IsEmpty()) //directory contains no items to be moved to it
				{
					db.RemoveDirectory(index);
					RefreshListViews();
				}
				else
				{
					MessageBoxResult result = MessageBox.Show("There are files to be moved to this destination. Do you still want to remove this directory? All files will be returned to the source files list.", "Remove Directory?", MessageBoxButton.YesNo, MessageBoxImage.Warning, MessageBoxResult.No);
					if (result == MessageBoxResult.Yes)
					{
						foreach (string files in db.GetDirInfo(index).GetFiles()) //need to add each file back into source list
							db.AddFileToSource(files);
						db.RemoveDirectory(index);
						listViewDestination.ItemsSource = null; //Used to reset listViewDestination, clear is for when you add items directly to the LV
						RefreshListViews();
					}
				}
				TextDirLbl.Text = "(No directory selected)";
			}
		}
		
		private void MoveToDestination(object sender, RoutedEventArgs e)
		{
			if (listViewSourceFiles.SelectedItems != null && listViewDirectories.SelectedItem != null)
			{
				string dir = listViewDirectories.SelectedItem.ToString();
				foreach (string file in listViewSourceFiles.SelectedItems)
				{
					db.AddFileToDestination(file, dir);
					UpdateProgress();
				}
				listViewDestination.ItemsSource = db.GetFilesFromDirectory(dir);
				RefreshListViews();
				HidePreview();
			}
		}

		private void MoveToSource(object sender, RoutedEventArgs e)
		{
			if (listViewDestination.SelectedItems != null && listViewDirectories.SelectedItem != null)
			{
				foreach (string file in listViewDestination.SelectedItems)
				{
					db.AddFileToSource(file);
					db.RemoveFileFromDestination(file, listViewDirectories.SelectedItem.ToString());
					UpdateProgress();
				}
				HidePreview();
				RefreshListViews();
			}
		}

		private void MoveHandler(object sender, RoutedEventArgs e)
		{
			if (db.Count() == 0) //TODO: Refactor so any and all GUI calls are handled elsewhere
			{
				MessageBox.Show("No files to move.");
				return;
			}

			MessageBoxResult result = MessageBox.Show("Are you sure you want to move the files?", "Move all files?", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No);
			if (result == MessageBoxResult.No)
				return;
			
			
			bool OverwriteAll = false;
			bool NoToAll = false;
			for (int i = 0; i < db.Count(); i++) //we initially check for duplicate files or invalid directories
			{
				FilesToMove dir = db.GetDirInfo(i);
				List<string> removedFiles = new List<string>();
				if (!Directory.Exists(db.GetDirInfo(i).GetDirectory()))
				{
					MessageBox.Show("Error: directory " + dir.GetDirectory() + " not found. Was this from an old save?", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
					db.RemoveDirectory(i);
					continue;
				}

				for (int j = 0; j < dir.Count(); j++)
				{
					string file = dir.GetFile(j);
					if (!File.Exists(file))
					{
						MessageBox.Show("Error: source file " + file + " not found. Was this from an old save?", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
						removedFiles.Add(file);
						continue;
					}
					string destination = dir.GetDirectory() + "\\" + Path.GetFileName(file);
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
							int offset = (Directory.GetFiles(dir.GetDirectory(), filename + " (?)" + ext)).Length + 2;
							destination = Path.GetFileNameWithoutExtension(file) + " (" + offset + ")" + ext;
							File.Move(file, Path.GetDirectoryName(file) + "\\" + destination); //Renames file
							if (!dir.UpdateFileName(file, Path.GetDirectoryName(file) + "\\" + destination)) //Attempts to update the file name within the db
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
			HidePreview();
			imgPreview.Source = null;
			listViewDestination.ItemsSource = null;
			worker.RunWorkerAsync();
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
					writer.WriteLine("----------------------------------------\n");
					writer.WriteLine("End of dump.");
				}
				MessageBox.Show("An error has occurred while moving files. An exception log has been created where this program exists.","Error",MessageBoxButton.OK,MessageBoxImage.Error);
			}
			if (!e.Cancelled)
			{
				bool finished = true;
				foreach (FilesToMove dirs in db.GetDirectories())
				{
					MessageBoxResult result = MessageBoxResult.None;
					if (dirs.Count() != 0 && finished == true)
					{
						result = MessageBox.Show("Some files could not be moved successfully. Would you like to keep the files in their selection and try again? (Selecting No will return them back to the source list.)", "Retry?", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No);
						finished = false;
						if(result == MessageBoxResult.No)
                        {
							foreach (string file in dirs.GetFiles())
								db.AddFileToSource(file);
							dirs.Clear();
							RefreshListViews();
						}
							
					}
					else if (dirs.Count() != 0 && result == MessageBoxResult.No) //todo: I don't think I ever actually finished this?
					{
						foreach (string file in dirs.GetFiles())
							db.AddFileToSource(file);
						RefreshListViews();
					}
				}
				MessageBox.Show("All files moved.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
			}
			else
				MessageBox.Show("Cancelled moving remaining files.", "Cancelled", MessageBoxButton.OK, MessageBoxImage.Information);
			db.IntTotalFiles = 0; //if user wants to add more files after moving the first batch, this resets the total count
			IntCurrentFile = 0;
			EnableButtons();
		}

		private void Worker_DoWork(object sender, DoWorkEventArgs e)
		{
		   
			for (int i = 0; i < db.Count(); i++)
			{
				FilesToMove dir = db.GetDirInfo(i);
				while (dir.Count() > 0)
				{
					if (worker.CancellationPending == true)
					{
						e.Cancel = true;
						return;
					}
					string file = dir.GetFile(0); //The file to move
					string destination = dir.GetDirectory() + "\\" + Path.GetFileName(dir.GetFile(0)); //The destination folder
					try
					{
						db.MoveFile(file, i);
					}
					catch
					{

					}
					IntCurrentFile++;
					int prog = Convert.ToInt32(((double)IntCurrentFile / (double)db.IntTotalFiles) * 100);
					(sender as BackgroundWorker).ReportProgress(prog, IntCurrentFile); //pass along the actual progress as well as the numerical amount of files that have been moved
					Thread.Sleep(300);
				}
				
			}

		}

		private void Worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
		{
			pb.Value = e.ProgressPercentage;
			txtProg.Text = e.UserState.ToString() + "/" + db.IntTotalFiles;
			
		}

		private void Cancel(object sender, RoutedEventArgs e)
		{
			MessageBoxResult result = MessageBox.Show("Are you sure you want to cancel moving?","Cancel?",MessageBoxButton.YesNo,MessageBoxImage.Question);
			if(result == MessageBoxResult.Yes)
				worker.CancelAsync();
		}

		private void HidePreview() 
		{
			imgPreview.Visibility = Visibility.Collapsed;
			gifPreview.Visibility = Visibility.Collapsed;
			previewGrid.Visibility = Visibility.Hidden;
			media.Stop();
			txtPreview.Visibility = Visibility.Collapsed;
			txtPreviewScroller.Visibility = Visibility.Collapsed;
			nullPreview.Visibility = Visibility.Collapsed;
		}

		private void PreviewHandler(string file)
		{
			
			if (File.Exists(file))
			{
				Uri uri = new Uri(file);
				if (MimeTypeMap.GetMimeType(uri.ToString()).Contains("image"))
				{
					HidePreview();
					BitmapImage image = new BitmapImage();
					image.BeginInit();
					image.CreateOptions = BitmapCreateOptions.IgnoreImageCache;
					image.CacheOption = BitmapCacheOption.OnLoad;
					image.UriSource = uri;
					image.EndInit();
					if (MimeTypeMap.GetMimeType(uri.ToString()).Contains("gif"))
					{
						gifPreview.Visibility = Visibility.Visible;
						ImageBehavior.SetAnimatedSource(gifPreview, image);
					}
					else
					{
						imgPreview.Visibility = Visibility.Visible;
						imgPreview.Source = image;
					}
					
				}
				else if (MimeTypeMap.GetMimeType(uri.ToString()).Contains("video") || MimeTypeMap.GetMimeType(uri.ToString()).Contains("audio"))
				{
					HidePreview();
					previewGrid.Visibility = Visibility.Visible;
					previewGrid.BringIntoView();
					time.Start();
					media.Play(new Media(lib, uri));
					media.Volume = 100;

				}
				else if (MimeTypeMap.GetMimeType(uri.ToString()).Contains("text"))
				{
					HidePreview();
					try
					{
						string contents = File.ReadAllText(file);
						txtPreviewScroller.Visibility = Visibility.Visible;
						txtPreview.Visibility = Visibility.Visible;
						txtPreview.Text = contents;
					}
					catch (Exception exc)
					{
						nullPreview.Visibility = Visibility.Visible;
					}
				}
				else
				{
					HidePreview();
					nullPreview.Visibility = Visibility.Visible;
				}
			}
			
			
		}


		private void listViewSourceFiles_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (e.AddedItems.Count > 0)
			{
				listViewDestination.SelectedItems.Clear();
				PreviewHandler(e.AddedItems[0].ToString());
			}
		}
		private void listViewDestination_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (e.AddedItems.Count > 0)
			{
				listViewSourceFiles.SelectedItems.Clear();
				PreviewHandler(e.AddedItems[0].ToString());
			}
		}

		private void Time_Tick(object sender, EventArgs e)
		{
			slider.Minimum = 0;
			slider.Maximum = media.Length / 1000;
			slider.Value = media.Time / 1000;

		}

        private void btnPause_Click(object sender, RoutedEventArgs e)
        {
			//vidPreview.Pause();
			media.Pause();
			time.Stop();
        }

        private void btnPlay_Click(object sender, RoutedEventArgs e)
        {
			//vidPreview.Play();
			media.Play();
			time.Start();
        }

        private void btnMute_Click(object sender, RoutedEventArgs e)
        {
			
			if (media.Volume == 0)
			{
				string image = @"D:\Programming Projects\C#\SoupMover\Images\volume-up.png";
				Uri uri = new Uri(image);
				media.Volume = 100;
				imgAudio.Source = new BitmapImage(uri);
			}
			else
			{
				string image = @"D:\Programming Projects\C#\SoupMover\Images\volume-off.png";
				Uri uri = new Uri(image);
				media.Volume = 0;
				imgAudio.Source = new BitmapImage(uri);
			}
			
        }

        private void slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
			lblTime.Text = TimeSpan.FromSeconds(slider.Value).ToString(@"hh\:mm\:ss");
        }

        private void slider_DragCompleted(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
        {
			media.Time = (long) TimeSpan.FromSeconds(slider.Value).TotalMilliseconds;
        }

		private void vidPreview_Loaded(object sender, RoutedEventArgs e)
		{
			Core.Initialize();
			lib = new LibVLC();
			media = new LibVLCSharp.Shared.MediaPlayer(lib);
			vidPreview.MediaPlayer = media;
			
		}

		private void listViewDirectories_MouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
			int index = listViewDirectories.SelectedIndex;
			if (index != -1)
            {
				int dirIndex = db.GetIndexOfDirectory(listViewDirectories.SelectedItem.ToString());
				listViewDestination.ItemsSource = db.GetDirInfo(dirIndex).GetFiles();
				TextDirLbl.Text = db.GetDirInfo(dirIndex).GetDirectory();
				RefreshListViews();
			}
				
		}

        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {

			if (SearchBox.Text != "Search..." && SearchBox.Text != "")
			{
				List<FilesToMove> temp = new List<FilesToMove>();
				foreach (var item in db.GetDirectories())
					if (item.GetDirectory().ToLower().Contains(SearchBox.Text.ToLower()))
						temp.Add(item);
				listViewDirectories.ItemsSource = temp;
			}
			else
				listViewDirectories.ItemsSource = db.GetDirectories();
			//Since we're changing the listview of directories, we don't want the user to think a directory is still selected
			listViewDirectories.SelectedIndex = -1;
			TextDirLbl.Text = "(No directory selected)";
			listViewDestination.ItemsSource = null;
			RefreshListViews();
		}

        private void SearchBox_GotFocus(object sender, RoutedEventArgs e)
        {
			if (SearchBox.Text == "Search...")
				SearchBox.Text = "";
        }

        private void SearchBox_LostFocus(object sender, RoutedEventArgs e)
        {
			if (SearchBox.Text == "")
				SearchBox.Text = "Search...";
		}

		private void SourceSearchBox_TextChanged(object sender, TextChangedEventArgs e)
		{

			if (SourceSearchBox.Text != "Search..." && SourceSearchBox.Text != "")
			{
				List<string> temp = new List<string>();
				foreach (var item in db.GetSourceFiles())
					if (item.ToLower().Contains(SourceSearchBox.Text.ToLower()))
						temp.Add(item);
				listViewSourceFiles.ItemsSource = temp;
			}
			else
				listViewSourceFiles.ItemsSource = db.GetSourceFiles();
			RefreshListViews();
		}

		private void SourceSearchBox_GotFocus(object sender, RoutedEventArgs e)
		{
			if (SourceSearchBox.Text == "Search...")
				SourceSearchBox.Text = "";
		}

		private void SourceSearchBox_LostFocus(object sender, RoutedEventArgs e)
		{
			if (SourceSearchBox.Text == "")
				SourceSearchBox.Text = "Search...";
		}

		private void DestinationSearchBox_TextChanged(object sender, TextChangedEventArgs e)
		{

			if (DestinationSearchBox.Text != "Search..." && DestinationSearchBox.Text != "" && listViewDirectories.SelectedIndex != -1)
			{
				List<string> temp = new List<string>();
				foreach (var item in db.GetFilesFromDirectory(listViewDirectories.SelectedItem.ToString()))
					if (item.ToLower().Contains(DestinationSearchBox.Text.ToLower()))
						temp.Add(item);
				listViewDestination.ItemsSource = temp;
			}
			else
			{
				if (listViewDirectories.SelectedIndex != -1)
					listViewDestination.ItemsSource = db.GetFilesFromDirectory(listViewDirectories.SelectedItem.ToString());
				else
					listViewDestination.ItemsSource = null;
			}
			RefreshListViews();
		}

		private void DestinationSearchBox_GotFocus(object sender, RoutedEventArgs e)
		{
			if (DestinationSearchBox.Text == "Search...")
				DestinationSearchBox.Text = "";
		}

		private void DestinationSearchBox_LostFocus(object sender, RoutedEventArgs e)
		{
			if (DestinationSearchBox.Text == "")
				DestinationSearchBox.Text = "Search...";
		}

		public MainWindow()
		{
			worker.WorkerSupportsCancellation = true;
			worker.WorkerReportsProgress = true;
			worker.DoWork += Worker_DoWork;
			worker.ProgressChanged += Worker_ProgressChanged;
			worker.RunWorkerCompleted += Worker_RunWorkerCompleted;
			InitializeComponent();
			listViewSourceFiles.ItemsSource = db.GetSourceFiles(); //binds List source files to the list view
			listViewDirectories.ItemsSource = db.GetDirectories();
			SearchBox.Text = "Search...";
			SourceSearchBox.Text = "Search...";
			DestinationSearchBox.Text = "Search...";
			time.Interval = TimeSpan.FromSeconds(1);
            time.Tick += Time_Tick;
			vidPreview.Loaded += vidPreview_Loaded;
		}

        
    }
}
