using SoupMover.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace SoupMover.Views
{
    /// <summary>
    /// Interaction logic for PreviewView.xaml
    /// </summary>
    public partial class PreviewView : UserControl
    {
        private PreviewViewModel PVM;
        public PreviewView()
        {
            InitializeComponent();
        }

        private void slider_DragCompleted(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
        {
            PVM.IsDragging = false;
            PVM.SetMediaTime();
        }

        private void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            PVM = this.DataContext as PreviewViewModel;
        }

        private void slider_DragStarted(object sender, System.Windows.Controls.Primitives.DragStartedEventArgs e)
        {
            PVM.IsDragging = true;
        }
    }
}
