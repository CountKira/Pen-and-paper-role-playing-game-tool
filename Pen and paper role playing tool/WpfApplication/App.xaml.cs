using MVVM_Framework;
using System.Diagnostics;
using System.Globalization;
using System.Threading;
using System.Windows;
using WpfApplication.ViewModel;
using WpfApplication.Windows;

namespace WpfApplication
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            DispatcherHelper.Initialize(Dispatcher);
            IDialogService dialogService = new DialogService(MainWindow);
            RegisterViewModels(dialogService);

            var viewModel = new MainWindowViewModel(dialogService);
            var view = new MainWindow { DataContext = viewModel };

            view.ShowDialog();
        }

        private static void RegisterViewModels(IDialogService dialogService)
        {
            dialogService.Register<ClientSetupViewModel, ClientSetup>();
            dialogService.Register<ServerSetupViewModel, ServerSetup>();
            dialogService.Register<TableViewModel, TableWindow>();
        }
    }
}