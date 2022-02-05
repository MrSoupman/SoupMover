using SoupMover.ViewModels;
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
            PVM.SetMediaTime();
        }

        private void vidPreview_Loaded(object sender, RoutedEventArgs e)
        {
            PVM.InitVidPreview();
        }

        private void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            PVM = this.DataContext as PreviewViewModel;
        }
    }
}
