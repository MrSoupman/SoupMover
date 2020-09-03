using System;
using System.Collections.Generic;
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
            MessageBoxResult result = MessageBox.Show("Are you sure you want to exit?", "Exit",MessageBoxButton.OKCancel,MessageBoxImage.Question,MessageBoxResult.Cancel);
            if(result == MessageBoxResult.OK)
                Application.Current.Shutdown();
            
        }

        private void About(object sender, RoutedEventArgs e)
        {

        }

        private void Reset(object sender, RoutedEventArgs e)
        {

        }

        private void GithubPage(object sender, RoutedEventArgs e)
        {

        }

        public MainWindow()
        {
            InitializeComponent();
            
        }
    }
}
