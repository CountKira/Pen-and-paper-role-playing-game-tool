using DCOM.WPF.MVVM;
using Pen_and_paper_role_playing_tool;
using StefanRiedmillerUtility;
using System;
using System.ComponentModel;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using WpfApplication.Properties;
using static WpfApplication.Properties.Resources;
namespace WpfApplication.ViewModel
{
	class MainWindowViewModel : INotifyPropertyChanged
	{
		IDialogService dialogService;
		private string chatName;
		private IClientServer clientServer;
		private string messageOutput;
		private string messageInput;
		public ICommand SendButton { get; set; }
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
				clientServer.Writer += (object sender, WriterEventArgs message) => TextBoxWriteLine(message.Message);
				chatName = clientServerViewModel.ChatName;
			}
		}
		private void Send(object obj)
		{
			var text = $"{chatName}: {MessageInput}";

			clientServer?.SendMessage(text);
			TextBoxWriteLine(text);
			MessageInput = "";
		}

		private void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

		private void TextBoxWriteLine(string message) => MessageOutput += $"{message}{Environment.NewLine}";
	}
}
