using MVVM_Framework;
using System;
using System.ComponentModel;
using System.IO;
using System.Windows.Input;
using TCP_Framework;
using WpfApplication.Annotations;
using static WpfApplication.Properties.Resources;

namespace WpfApplication.ViewModel
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        private readonly IDialogService dialogService;
        private string chatName;
        public IClientServer ClientServer { get; set; }
        private string messageOutput;
        private string messageInput;
        public ICommand SendMessageCommand { get; }

        [UsedImplicitly]
        public ICommand OpenTableCommand { get; }

        [UsedImplicitly]
        public ICommand SetupServerCommand { get; }

        [UsedImplicitly]
        public ICommand SetupClientCommand { get; }

        private readonly TableViewModel tableViewModel;

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
            tableViewModel = new TableViewModel();
        }

        private void SendData(string tag, object data)
        {
            Logger.Log($"MainWindowViewModel.SendData-> tag: {tag}, data: {data}");
            ClientServer?.SendData(new DataHolder { Tag = tag, Data = data });
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
            SendData("TableViewModelRequest", null);
        }

        private void SetupServerOrClient(IClientServerViewModel clientServerViewModel, bool? result)
        {
            if (result != true) return;
            TextBoxWriteLine(Connection_established);
            ClientServer = clientServerViewModel.ClientServer;
            tableViewModel.ClientServer = ClientServer;
            ClientServer.DataReceivedEvent += (sender, data) =>
            {
                var tag = data.Dataholder.Tag;
                Logger.Log($"MainWindowViewModel.SetupServerOrClient-> tag: {tag}, data: {data.Dataholder.Data}");
                switch (data.Dataholder.Data)
                {
                    case string text:
                        switch (tag)
                        {
                            case "BackgroundChanged":
                                tableViewModel.ImageName = text;
                                break;

                            case "PictureRequest":
                                SendPicture(text);
                                break;

                            default:
                                TextBoxWriteLine(text);
                                break;
                        }
                        break;

                    case IOException _:
                        var x = tag == "Client" ? Connection_to_client_lost : Connection_to_server_lost;
                        TextBoxWriteLine(x);
                        break;

                    case TableViewData tvm:
                        tableViewModel.SetData(tvm);
                        break;

                    case null:
                        if (tag == "TableViewModelRequest")
                            SendData("TableViewModelResponse", tableViewModel.GetData());
                        break;

                    case TableElementData newTableElementData:
                        tableViewModel.Add(newTableElementData);
                        break;

                    case TableElementPropertyChangedData tepcd:
                        tableViewModel.ChangeTableElement(tepcd);
                        break;

                    case byte[] bytes:
                        tableViewModel.ReceivePictureData(bytes);
                        break;

                    case TableElementPictureRequest tableElementPictureRequest:
                        var pictureData = PictureHelper.GetPictureData(tableElementPictureRequest.PictureName);
                        SendData("TableElementPictureResponse", new TableElementPictureResponse
                        {
                            PictureData = pictureData,
                            Index = tableElementPictureRequest.Index,
                            PictureName = tableElementPictureRequest.PictureName
                        });
                        break;

                    case TableElementPictureResponse tableElementPictureResponse:
                        tableViewModel.SetTableElementPicture(tableElementPictureResponse);
                        break;
                }
            };
            chatName = clientServerViewModel.ChatName;
        }

        private void SendPicture(string pictureName)
        {
            var pictureData = PictureHelper.GetPictureData(pictureName);
            SendData("PictureResponse", pictureData);
        }

        private void SendMessageMethod(object parameter)
        {
            var text = $"{chatName}: {MessageInput}";

            SendData("Text", text);
            TextBoxWriteLine(text);
            MessageInput = "";
        }

        private void TextBoxWriteLine(string message) => MessageOutput += $"{message}{Environment.NewLine}";

        private void OpenTableMethod(object parameter)
        {
            dialogService.Show(tableViewModel);
        }

        private void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}