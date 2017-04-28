using MVVM_Framework;
using System;
using System.ComponentModel;
using System.Configuration;
using System.Windows;
using System.Windows.Input;
using TCP_Framework;
using static WpfApplication.Properties.Resources;

namespace WpfApplication.ViewModel
{
    internal class ClientSetupViewModel : INotifyPropertyChanged, IClientServerViewModel
    {
        public IClientServer ClientServer { get; set; }
        private string chatName;

        public string ChatName
        {
            get => chatName;
            set { chatName = value; OnPropertyChanged(nameof(ChatName)); }
        }

        private string ipAddressbox;

        public string IpAddressbox
        {
            get => ipAddressbox;
            set { ipAddressbox = value; OnPropertyChanged(nameof(IpAddressbox)); }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public event EventHandler<DialogCloseRequestedEventArgs> CloseRequested;

        private void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        public ICommand ConnectClick { get; set; }
        public ICommand CancelClick { get; set; }

        public ClientSetupViewModel()
        {
            ConnectClick = new ActionCommand(Connect_Click2);
            CancelClick = new ActionCommand(p => CloseRequested?.Invoke(this, new DialogCloseRequestedEventArgs(false)));
#if DEBUG
            ChatName = "Bill";
            IpAddressbox = "127.0.0.1";
#endif
        }

        private void Connect_Click2(object sender)
        {
            var portNumber = int.Parse(ConfigurationManager.AppSettings["portNumber"]);
            var client = new Client(portNumber, ipAddressbox);
            if (client.TryConnectingToServer())
            {
                ClientServer = client;
                CloseRequested?.Invoke(this, new DialogCloseRequestedEventArgs(true));
            }
            else
            {
                MessageBox.Show(Could_not_establish_a_connection_with_the_server);
            }
        }
    }
}