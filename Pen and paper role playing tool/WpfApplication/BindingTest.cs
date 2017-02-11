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
		private IClientServer clientServer;
		private string messageOutput;
		private string messageInput;

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
			SendButton = new RelayCommand(Send_Click);
			ServerButton = new RelayCommand(MenuServerItem_Click);
			ClientButton = new RelayCommand(MenuClientItem_Click);
		}

		//TODO: Handle the case that the server or client could not be initiated
		private void MenuServerItem_Click(object obj)
		{
			var serverSetup = new ServerSetup();
			serverSetup.ShowDialog();

			TextBoxWriteLine("Connection established");
			clientServer = serverSetup.Server;
			ReceiveMessagesAsync();
		}
		private void MenuClientItem_Click(object obj)
		{
			var clientSetup = new ClientSetup();
			clientSetup.ShowDialog();

			TextBoxWriteLine("Connection established");
			clientServer = clientSetup.Client;
			ReceiveMessagesAsync();
		}
		private void Send_Click(object obj)
		{
			var text = obj.ToString();
			Console.WriteLine(MessageInput);
			clientServer?.SendMessage(text);
			TextBoxWriteLine($"Server: {text}");
			MessageInput = "";
		}

		private async void ReceiveMessagesAsync()
		{
			while (true)
			{
				var message = await clientServer.ReceiveMessage(new CancellationToken());
				TextBoxWriteLine($"Client: {message}");
			}
		}

		private void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

		private void TextBoxWriteLine(string message) => MessageOutput += $"{message}{Environment.NewLine}";
	}
}
