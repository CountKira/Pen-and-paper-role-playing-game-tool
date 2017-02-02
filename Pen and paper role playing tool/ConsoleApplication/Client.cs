using System;
using System.Text;
using System.Net.Sockets;
using System.Net;
using System.Threading;

namespace ConsoleApplication
{
	public class Client
	{
		private TcpClient clientSocket = new TcpClient();
		private int port;
		private string address;

		public Client(int port, string address)
		{
			this.port = port;
			this.address = address;
		}

		public bool TryConnectingToServer()
		{
			try
			{
				clientSocket.Connect(address, port);
			}
			catch (Exception)
			{
				return false;
			}
			return true;
		}

		public void SendMessageToServer(string input)
		{
			MessageHandler.SendMessages(clientSocket, input);
		}

		public void ReceiveMessage(CancellationToken token)
		{
			try
			{
				while (!token.IsCancellationRequested)
				{
					var message = MessageHandler.ReceiveMessagesAsync(clientSocket, token).Result;
					WriteMessageToConsole($"Server: {message}");
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.ToString());
				return;
			}
		}

		private void WriteMessageToConsole(string message) => Console.WriteLine($"{message}");

		public void DisconnectFromServer()
		{
			clientSocket.Close();
		}
	}
}