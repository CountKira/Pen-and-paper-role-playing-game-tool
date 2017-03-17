using MVVM_Framework;
using TCP_Framework;
using System;
using System.ComponentModel;
using System.IO;
using System.Windows.Input;
using static WpfApplication.Properties.Resources;
using System.Windows;

namespace WpfApplication.ViewModel
{
    internal class MainWindowViewModel : INotifyPropertyChanged
    {
        private IDialogService dialogService;
        private string chatName;
        private IClientServer clientServer;
        private string messageOutput;
        private string messageInput;
        public ICommand SendButton { get; set; }
        public ICommand BoardButton { get; set; }
        public ICommand ServerButton { get; set; }
        public ICommand ClientButton { get; set; }

        public string MessageOutput
        {
            get => messageOutput;
            set { messageOutput = value; OnPropertyChanged(nameof(MessageOutput)); }
        }

        public string MessageInput
        {
            get => messageInput;
            set { messageInput = value; OnPropertyChanged(nameof(MessageInput)); }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public MainWindowViewModel(IDialogService service)
        {
            dialogService = service;
            SendButton = new ActionCommand(Send);
            ServerButton = new ActionCommand(MenuServerItem);
            ClientButton = new ActionCommand(MenuClientItem);
            BoardButton = new ActionCommand(MenuBoardGameItem);
        }

        private void MenuBoardGameItem(object obj)
        {
            var boardViewModel = new BoardViewModel(clientServer);
            if (clientServer != null)
            {
                clientServer.DataReceivedEvent += boardViewModel.DataReception;
            }
            dialogService.Show(boardViewModel);
        }

        //TODO: Handle the case that the server or client could not be initiated
        private void MenuServerItem(object obj)
        {
            var serverSetupViewModel = new ServerSetupViewModel();
            var result = dialogService.ShowDialog(serverSetupViewModel);
            SetupServerOrClient(serverSetupViewModel, result);
        }

        private void MenuClientItem(object obj)
        {
            var clientSetupViewModel = new ClientSetupViewModel();
            var result = dialogService.ShowDialog(clientSetupViewModel);
            SetupServerOrClient(clientSetupViewModel, result);
        }

        private void SetupServerOrClient(IClientServerViewModel clientServerViewModel, bool? result)
        {
            if (result == true)
            {
                TextBoxWriteLine(Connection_established);
                clientServer = clientServerViewModel.ClientServer;
                clientServer.DataReceivedEvent += (object sender, DataReceivedEventArgs data) =>
                {
                    if (data.Dataholder.Tag == "Picture")
                    {
                        var bytes = data.Dataholder.Data as byte[];
                        File.WriteAllBytes(@"ClientPicture\SpaceChem.mp4", bytes);
                    }
                    else if (data.Dataholder.Tag == "Text")
                    {
                        TextBoxWriteLine((string)data.Dataholder.Data);
                    }
                };
                chatName = clientServerViewModel.ChatName;
            }
        }

        private void Send(object obj)
        {
            var text = $"{chatName}: {MessageInput}";

            DateHolder dataHolder;
            if (MessageInput == "p")
                dataHolder = new DateHolder { Tag = "Picture", Data = File.ReadAllBytes(@"ServerPicture\SpaceChem.mp4") };
            else
                dataHolder = new DateHolder { Tag = "Text", Data = text };
            clientServer?.SendData(dataHolder);
            TextBoxWriteLine(text);
            MessageInput = "";
        }

        private void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        private void TextBoxWriteLine(string message) => MessageOutput += $"{message}{Environment.NewLine}";
    }
}