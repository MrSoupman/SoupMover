using System;
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
        public Result RESULT;
        public FileCompare(string originalFile,string destinationFile)
        {
            InitializeComponent();
            FileInfo info;
            //sourceFileIcon = System.Drawing.Icon.ExtractAssociatedIcon(originalFile);
            if (File.Exists(originalFile))
            {
                using (Icon ico = System.Drawing.Icon.ExtractAssociatedIcon(originalFile))
                {
                    sourceFileIcon.Source = Imaging.CreateBitmapSourceFromHIcon(ico.Handle, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
                }
                sourceFileName.Text = Path.GetFileName(originalFile);
                sourceDirectory.Text = Path.GetFullPath(originalFile);
                info = new FileInfo(originalFile);
                sourceFileSize.Text = info.Length
            }
            if (File.Exists(destinationFile))
            {
                using (Icon ico = System.Drawing.Icon.ExtractAssociatedIcon(destinationFile))
                {
                    destinationFileIcon.Source = Imaging.CreateBitmapSourceFromHIcon(ico.Handle, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
                }
            }
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
