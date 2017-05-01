using MVVM_Framework;
using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Input;
using TCP_Framework;
using WpfApplication.Annotations;
using System.Text.RegularExpressions;
using static WpfApplication.Properties.Resources;

namespace WpfApplication.ViewModel
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        private readonly IDialogService dialogService;
        private string chatName;
        public IClientServer ClientServer { private get; set; }
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
            tableViewModel = new TableViewModel(service);
        }

        private void SendData(string tag, object data)
        {
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
                        var message = tag == "Client" ? Connection_to_client_lost : Connection_to_server_lost;
                        TextBoxWriteLine(message);
                        break;

                    case int integer:
                        tableViewModel.DeleteElement(integer);
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
            const string diceCommand = @"^(?<quantity>\d+)\s*d\s*(?<sides>\d+)\s*(?<modifier>[+-]\d*)?";
            var regex = new Regex(diceCommand);
            var match = regex.Match(MessageInput);
            var messageText = match.Success ? RollDiceAndConvertToString(match) : MessageInput;
            var text = $"{chatName}: {messageText}";
            SendData("Text", text);
            TextBoxWriteLine(text);
            MessageInput = "";
        }

        private static string RollDiceAndConvertToString(Match match)
        {
            var sides = int.Parse(match.Groups["sides"].Value);
            var quantity = int.Parse(match.Groups["quantity"].Value);
            var modifierGroup = match.Groups["modifier"];
            int? modifier = null;
            if (modifierGroup.Success)
                modifier = int.Parse(match.Groups["modifier"].Value);
            var dices = Dice.Throw(quantity, sides);
            var builder = new StringBuilder();
            foreach (var dice in dices)
            {
                builder.Append($"{dice}+");
            }
            builder.Remove(builder.Length - 1, 1);
            string modifierText = "";
            if (modifier != null)
            {
                var sign = modifier >= 0 ? "+" : "";
                modifierText = $"{sign}{modifier}";
            }
            return $"{quantity}d{sides} = ({builder}){modifierText} = {dices.Sum() + (modifier ?? 0)}";
        }

        private void TextBoxWriteLine(string message) => MessageOutput += $"{message}{Environment.NewLine}";

        private void OpenTableMethod(object parameter)
        {
            dialogService.Show(tableViewModel);
        }

        private void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    internal static class Dice
    {
        private static readonly Random rand = new Random();

        public static int[] Throw(int quantity, int sides)
        {
            var results = new int[quantity];
            for (int i = 0; i < quantity; i++)
                results[i] = rand.Next(1, sides + 1);
            return results;
        }
    }
}