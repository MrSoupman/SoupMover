using Microsoft.Win32;
using SoupMover.FileWindow;
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
        public MainWindow()
        {
            InitializeComponent();
        }
    }
}
