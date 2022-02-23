using SoupMover.Services;
using SoupMover.Stores;
using SoupMover.ViewModels;
using System.Windows;

namespace SoupMover
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        //TODO: https://stackoverflow.com/questions/793100/globally-catch-exceptions-in-a-wpf-application
        private readonly NavStore nav;
        private readonly ModalNavStore modal;
        private readonly DialogStore dialog;
        protected override void OnStartup(StartupEventArgs e)
        {
            nav.CurrentVM = new HomeViewModel(new ModalNavSvc(modal, CreateFileCompareViewModel), dialog, new PreviewViewModel());
            MainWindow = new MainWindow()
            {
                DataContext = new MainViewModel(nav, modal)
            };
            MainWindow.Show();
            base.OnStartup(e);

        }

        public App()
        {
            nav = new NavStore();
            modal = new ModalNavStore();
            dialog = new DialogStore();
        }

        private FileCompareViewModel CreateFileCompareViewModel()
        {
            return new FileCompareViewModel(new ModalNavSvc(modal, CreateFileCompareViewModel), dialog);
        }
    }
}
