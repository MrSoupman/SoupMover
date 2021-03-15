﻿using Microsoft.Win32;
using SoupMover.FileWindow;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Xml;
using System.Threading;
using System.Windows.Threading;
using Path = System.IO.Path;
using MimeTypes;
//using LibVLCSharp.WPF;
using LibVLCSharp.Shared;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

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
		BackgroundWorker worker = new BackgroundWorker();
		DispatcherTimer time = new DispatcherTimer();
		LibVLC lib;
		LibVLCSharp.Shared.MediaPlayer media;

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

		private void Load(object sender, RoutedEventArgs e)
		{
			OpenFileDialog open = new OpenFileDialog();
			open.Multiselect = false;
			open.Filter = "JSON file (*.json)|*.json";
			if (open.ShowDialog() == true)
			{
				Reset();
				using (StreamReader reader = File.OpenText(open.FileName))
				{
					JObject jFile = (JObject)JToken.ReadFrom(new JsonTextReader(reader));
					JArray sourceArray = JArray.Parse(jFile["SourceFiles"].ToString());
					foreach (JToken token in sourceArray)
						listSourceFiles.Add(token.ToString());
					JObject dirObj = JObject.Parse(jFile["Directories"].ToString());
					Console.WriteLine(dirObj.Count);
					foreach (JContainer obj in dirObj.Children())
					{
						JProperty property = obj.ToObject<JProperty>();
						FilesToMove dir = new FilesToMove(property.Name);
						JArray filesArray = JArray.Parse(property.Value.ToString());
						foreach (JToken token in filesArray)
						{
							dir.Add(token.ToString());
							intTotalFiles++;
						}
						directories.Add(dir);
					}
				}
				UpdateProgress();
				RefreshListViews();
			}
		}

		private void Save(object sender, RoutedEventArgs e)
		{
			SaveFileDialog save = new SaveFileDialog();
			save.Filter = "json file (*.json)|*.json";
			if (save.ShowDialog() == true)
			{
				StringBuilder sb = new StringBuilder();
				using (StringWriter sw = new StringWriter(sb))
				{
					using (JsonWriter writer = new JsonTextWriter(sw))
					{
						writer.Formatting = Newtonsoft.Json.Formatting.Indented;
						writer.WriteStartObject();
						writer.WritePropertyName("SourceFiles");
						writer.WriteStartArray();
						foreach (string file in listSourceFiles)
							writer.WriteValue(file);
						writer.WriteEndArray();
						writer.WritePropertyName("Directories");
						writer.WriteStartObject();
						foreach (FilesToMove dirs in directories)
						{
							writer.WritePropertyName(dirs.GetDirectory());
							writer.WriteStartArray();
							foreach (string file in dirs.GetFiles())
								writer.WriteValue(file);
							writer.WriteEndArray();
						}
						writer.WriteEnd();
						writer.WriteEnd();
					}
					File.WriteAllText(save.FileName, sw.ToString());
				}
			}
		}

		private void SaveJ(object sender, RoutedEventArgs e)
		{
			
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
			MessageBox.Show("Soup Mover V0.8\nSoup Mover is a program for moving files to various folders. " +
				"Made by MrSoupman.\n Thanks to samuelneff for MimeTypeMap, Mono Company for their icons, and VLC for LibVLCSharp!", "About", MessageBoxButton.OK, MessageBoxImage.Information);
		}

		private void Reset()
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
			if (!e.Cancelled)
			{
				bool finished = true;
				foreach (FilesToMove dirs in directories)
				{
					MessageBoxResult result = MessageBoxResult.None;
					if (dirs.Count() != 0 && finished == true)
					{
						result = MessageBox.Show("Some files could not be moved successfully. Would you like to keep the files in their selection and try again? (Selecting No will return them back to the source list.)", "Retry?", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No);
						finished = false;
						if(result == MessageBoxResult.No)
                        {
							foreach (string file in dirs.GetFiles())
								listSourceFiles.Add(file);
							dirs.Clear();
							RefreshListViews();
						}
							
					}
					else if (dirs.Count() != 0 && result == MessageBoxResult.No) //todo: I don't think I ever actually finished this?
					{
						foreach (string file in dirs.GetFiles())
							listSourceFiles.Add(file);
						RefreshListViews();
					}
				}
				MessageBox.Show("All files moved.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
			}
			else
				MessageBox.Show("Cancelled further moves.", "Cancelled", MessageBoxButton.OK, MessageBoxImage.Information);

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
					if(worker.CancellationPending == true)
					{
						e.Cancel = true;
						return;
					}
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
					}
					currentFiles++;
					int prog = Convert.ToInt32(((double)currentFiles / (double)totalFiles) * 100);
					(sender as BackgroundWorker).ReportProgress(prog, currentFiles); //pass along the actual progress as well as the numerical amount of files that have been moved
					Thread.Sleep(300);
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

		private void Cancel(object sender, RoutedEventArgs e)
		{
			MessageBoxResult result = MessageBox.Show("Are you sure you want to cancel moving?","Cancel?",MessageBoxButton.YesNo,MessageBoxImage.Question);
			if(result == MessageBoxResult.Yes)
				worker.CancelAsync();
		}

		private void HidePreview() 
		{
			imgPreview.Visibility = Visibility.Collapsed;
			previewGrid.Visibility = Visibility.Hidden;
			media.Stop();
			txtPreview.Visibility = Visibility.Collapsed;
			txtPreviewScroller.Visibility = Visibility.Collapsed;
			nullPreview.Visibility = Visibility.Collapsed;
			

		}

		private void PreviewHandler(string file)
		{
			Uri uri = new Uri(file);
			if (MimeTypeMap.GetMimeType(uri.ToString()).Contains("image"))
			{
				HidePreview();
				imgPreview.Visibility = Visibility.Visible;
				imgPreview.Source = new BitmapImage(uri);
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

        public MainWindow()
		{
			worker.WorkerSupportsCancellation = true;
			worker.WorkerReportsProgress = true;
			worker.DoWork += Worker_DoWork;
			worker.ProgressChanged += Worker_ProgressChanged;
			worker.RunWorkerCompleted += Worker_RunWorkerCompleted;
			InitializeComponent();
			listViewSourceFiles.ItemsSource = listSourceFiles; //binds List source files to the list view
			listViewDirectories.ItemsSource = directories;
			time.Interval = TimeSpan.FromSeconds(1);
            time.Tick += Time_Tick;
			vidPreview.Loaded += vidPreview_Loaded;
		}

        
    }
}
