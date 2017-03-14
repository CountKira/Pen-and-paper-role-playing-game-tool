using DCOM.WPF.MVVM;
using System.Diagnostics;
using System.Globalization;
using System.Threading;
using System.Windows;
using WpfApplication.ViewModel;

namespace WpfApplication
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
#if DEBUG
            foreach (var arg in e.Args)
            {
                //TODO: Remove setting the culture with arguments after debug
                try
                {
                    Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo(arg);
                }
                catch (CultureNotFoundException cultureException)
                {
                    Debug.WriteLine(cultureException.Message);
                }
            }
#endif
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
        }
    }
}