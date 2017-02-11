using Pen_and_paper_role_playing_tool;
using System;
using System.ComponentModel;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace WpfApplication
{
	class BindingTest : INotifyPropertyChanged
	{
		private string chatName;
		private IClientServer clientServer;
		private string messageOutput;
		private string messageInput;
		private Servers servers;
		public ICommand SendButton { get; set; }
		public ICommand ServerButton { get; set; }
		public ICommand ClientButton { get; set; }
		public event PropertyChangedEventHandler PropertyChanged;
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

		public BindingTest()
		{
			SendButton = new RelayCommand(Send);
			ServerButton = new RelayCommand(MenuServerItem);
			ClientButton = new RelayCommand(MenuClientItem);
		}

		//TODO: Handle the case that the server or client could not be initiated
		private void MenuServerItem(object obj)
		{
			var serverSetup = new ServerSetup();
			serverSetup.ShowDialog();

			TextBoxWriteLine("Connection established");
			servers = serverSetup.Servers;
			servers.Writer += TextBoxWriteLine;
			chatName = serverSetup.ChatName;
		}
		private void MenuClientItem(object obj)
		{
			var clientSetup = new ClientSetup();
			clientSetup.ShowDialog();

			TextBoxWriteLine("Connection established");
			clientServer = clientSetup.Client;
			ReceiveMessagesAsync();
			chatName = clientSetup.ChatName;
		}
		private void Send(object obj)
		{
			var text = obj.ToString();
			Console.WriteLine(MessageInput);
			text = $"{chatName}: {text}";

			clientServer?.SendMessage(text);
			servers?.SendMessage(text);

			TextBoxWriteLine(text);
			MessageInput = "";
		}

		private async void ReceiveMessagesAsync()
		{
			while (clientServer != null)
			{
				var message = await clientServer.ReceiveMessage(new CancellationToken());
				TextBoxWriteLine(message);
			}
		}

		private void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

		private void TextBoxWriteLine(string message) => MessageOutput += $"{message}{Environment.NewLine}";
	}
}
