using SoupMover.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace SoupMover.Views
{
    /// <summary>
    /// Interaction logic for HomeView.xaml
    /// </summary>
    public partial class HomeView : UserControl
    {
        public HomeView()
        {
            InitializeComponent();

        }

        private void DirectorySearch_GotFocus(object sender, RoutedEventArgs e)
        {
            if (DirectorySearch.Text == "Search...")
                DirectorySearch.Text = "";
        }

        private void DirectorySearch_LostFocus(object sender, RoutedEventArgs e)
        {
            if (DirectorySearch.Text == "")
                DirectorySearch.Text = "Search...";
        }

        private void SourceSearch_GotFocus(object sender, RoutedEventArgs e)
        {
            if (SourceSearch.Text == "Search...")
                SourceSearch.Text = "";
        }

        private void SourceSearch_LostFocus(object sender, RoutedEventArgs e)
        {
            if (SourceSearch.Text == "")
                SourceSearch.Text = "Search...";
        }

        private void DestinationSearch_GotFocus(object sender, RoutedEventArgs e)
        {
            if (DestinationSearch.Text == "Search...")
                DestinationSearch.Text = "";
        }

        private void DestinationSearch_LostFocus(object sender, RoutedEventArgs e)
        {
            if (DestinationSearch.Text == "")
                DestinationSearch.Text = "Search...";
        }
    }
}
