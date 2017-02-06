using Pen_and_paper_role_playing_tool;
using System;
using System.Threading;
using System.Windows;

namespace WpfApplication
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();
		}
		private IClientServer clientServer;
		//TODO: Handle the case that the server or client could not be initiated
		private void MenuServerItem_Click(object sender, RoutedEventArgs e)
		{
			var serverSetup = new ServerSetup();
			serverSetup.ShowDialog();

			TextBoxWriteLine("Connection established");
			clientServer = serverSetup.server;
			ReceiveMessagesAsync();
		}
		private void MenuClientItem_Click(object sender, RoutedEventArgs e)
		{
			var clientSetup = new ClientSetup();
			clientSetup.ShowDialog();

			TextBoxWriteLine("Connection established");
			clientServer = clientSetup.Client;
			ReceiveMessagesAsync();
		}

		private async void ReceiveMessagesAsync()
		{
			while (true)
			{
				var message = await clientServer.ReceiveMessage(new CancellationToken());
				TextBoxWriteLine($"Client: {message}");
			}
		}

		private void Server_MessageWriter(string message)
		{
			TextBoxWriteLine(message);
		}

		private void TextBoxWriteLine(string message) => messageOutput.Text += $"{message}{Environment.NewLine}";

		private void Send_Click(object sender, RoutedEventArgs e)
		{
			var text = messageInput.Text;
			clientServer?.SendMessage(text);
			TextBoxWriteLine($"Server: {text}");
			messageInput.Text = "";
		}

	}
}