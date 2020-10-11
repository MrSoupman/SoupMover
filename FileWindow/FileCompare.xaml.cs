﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Path = System.IO.Path;
namespace SoupMover.FileWindow
{
    /// <summary>
    /// Interaction logic for FileCompare.xaml
    /// </summary>

    public enum Result{ YES,YESTOALL,NO,NOTOALL,KEEPBOTH }
    
    public partial class FileCompare : Window
    {
        //Returns a formatted string; the highest binary unit size next to the size of the file.
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
            string sizeUnit = String.Format("{0:F2} {1}", size,unit[count]);
            return sizeUnit;
        }

        public Result RESULT;
        public FileCompare(string originalFile,string destinationFile)
        {
            InitializeComponent();
            if (File.Exists(originalFile))
            {
                using (Icon ico = System.Drawing.Icon.ExtractAssociatedIcon(originalFile))
                {
                    sourceFileIcon.Source = Imaging.CreateBitmapSourceFromHIcon(ico.Handle, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
                }
                sourceFileName.Text = Path.GetFileName(originalFile);
                sourceDirectory.Text = Directory.GetParent(originalFile).ToString();
                sourceFileSize.Text = "Size: " + FileSizeUnit(originalFile);
                sourceCreated.Text = "Date Created: " + File.GetCreationTime(originalFile).ToString();
            }
            if (File.Exists(destinationFile))
            {
                using (Icon ico = System.Drawing.Icon.ExtractAssociatedIcon(destinationFile))
                {
                    destinationFileIcon.Source = Imaging.CreateBitmapSourceFromHIcon(ico.Handle, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
                }
                destinationFileName.Text = Path.GetFileName(destinationFile);
                destinationDirectory.Text = Directory.GetParent(destinationFile).ToString();
                destinationFileSize.Text = "Size: " + FileSizeUnit(destinationFile);
                destinationCreated.Text = "Date Created: " + File.GetCreationTime(destinationFile).ToString();
            }
            //After setting all the textt to their rightful info, format the window to to handle all the text
            this.SizeToContent = SizeToContent.Manual;
            this.SizeToContent = SizeToContent.Height;
            this.Topmost = true;
        }

        private void Btn_Yes(object sender, RoutedEventArgs e)
        {
            RESULT = Result.YES;
            this.Close();
        }
        private void Btn_YesToAll(object sender, RoutedEventArgs e)
        {
            RESULT = Result.YESTOALL;
            this.Close();
        }
        private void Btn_No(object sender, RoutedEventArgs e)
        {
            RESULT = Result.NO;
            this.Close();
        }
        private void Btn_NoToAll(object sender, RoutedEventArgs e)
        {
            RESULT = Result.NOTOALL;
            this.Close();
        }
        private void Btn_Both(object sender, RoutedEventArgs e)
        {
            RESULT = Result.KEEPBOTH;
            this.Close();
        }
    }
}
