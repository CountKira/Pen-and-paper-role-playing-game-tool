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
namespace WpfApplication
{
	class MainWindowViewModel : INotifyPropertyChanged
	{
		IDialogService dialogService;
		private string chatName;
		private IClientServer clientServer;
		private string messageOutput;
		private string messageInput;
		private Servers servers;
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
			SendButton = new RelayCommand(Send);
			ServerButton = new RelayCommand(MenuServerItem);
			ClientButton = new RelayCommand(MenuClientItem);
		}

		//TODO: Handle the case that the server or client could not be initiated
		private void MenuServerItem(object obj)
		{
			var serverSetupViewModel = new ServerSetupViewModel();
			var result = dialogService.ShowDialog(serverSetupViewModel);
			if (result == true)
			{
				TextBoxWriteLine(Connection_opened_Waiting_for_clients);
				servers = serverSetupViewModel.Servers;
				servers.Writer += (object sender, WriterEventArgs message) => TextBoxWriteLine(message.Message);
				chatName = serverSetupViewModel.ChatName;
			}
		}
		private void MenuClientItem(object obj)
		{
			var clientSetupViewModel = new ClientSetupViewModel();
			var result = dialogService.ShowDialog(clientSetupViewModel);
			if (result == true)
			{
				TextBoxWriteLine(Connection_established);
				clientServer = clientSetupViewModel.Client;
				ReceiveMessagesAsync();
				chatName = clientSetupViewModel.ChatName;
			}
		}
		private void Send(object obj)
		{
			var text = $"{chatName}: {MessageInput}";

			clientServer?.SendMessage(text);
			servers?.SendMessage(text);

			TextBoxWriteLine(text);
			MessageInput = "";
		}

		private async void ReceiveMessagesAsync()
		{
			while (clientServer != null)
			{
				try
				{
					var message = await clientServer.ReceiveMessage(new CancellationToken());
					TextBoxWriteLine(message);
				}
				catch (Exception)
				{
					return;
				}
			}
		}

		private void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

		private void TextBoxWriteLine(string message) => MessageOutput += $"{message}{Environment.NewLine}";
	}
}
