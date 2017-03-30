﻿using MVVM_Framework;
using TCP_Framework;
using System;
using System.ComponentModel;
using System.IO;
using System.Windows.Input;
using static WpfApplication.Properties.Resources;
using System.Windows;

namespace WpfApplication.ViewModel
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        private IDialogService dialogService;
        private string chatName;
        public IClientServer ClientServer { get; set; }
        private string messageOutput;
        private string messageInput;
        public ICommand SendMessageCommand { get; set; }
        public ICommand OpenTableCommand { get; set; }
        public ICommand SetupServerCommand { get; set; }
        public ICommand SetupClientCommand { get; set; }

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
            SendMessageCommand = new ActionCommand(SendMessageMethod);
            SetupServerCommand = new ActionCommand(SetupServerMethod);
            SetupClientCommand = new ActionCommand(SetupClientMethod);
            OpenTableCommand = new ActionCommand(OpenTableMethod);
        }

        //TODO: Handle the case that the server or client could not be initiated
        private void SetupServerMethod(object parameter)
        {
            var serverSetupViewModel = new ServerSetupViewModel();
            var result = dialogService.ShowDialog(serverSetupViewModel);
            SetupServerOrClient(serverSetupViewModel, result);
        }

        private void SetupClientMethod(object parameter)
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
                ClientServer = clientServerViewModel.ClientServer;
                ClientServer.DataReceivedEvent += (object sender, DataReceivedEventArgs data) =>
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

        private void SendMessageMethod(object parameter)
        {
            var text = $"{chatName}: {MessageInput}";

            DataHolder dataHolder = new DataHolder { Tag = "Text", Data = text };
            ClientServer?.SendData(dataHolder);
            TextBoxWriteLine(text);
            MessageInput = "";
        }

        private void TextBoxWriteLine(string message) => MessageOutput += $"{message}{Environment.NewLine}";

        private void OpenTableMethod(object parameter)
        {
            var tableViewModel = new TableViewModel(ClientServer);
            dialogService.Show(tableViewModel);
        }

        private void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}