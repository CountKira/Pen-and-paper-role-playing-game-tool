using System;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleApplication
{
	public class Server
	{
		public event Action<string> MessageWriter;

		public void RunServer(int port)
		{
			var listener = new TcpListener(IPAddress.Any, port);
			listener.Start();
			WriteMessage("Server started. Waiting for client.");
			using (var client = listener.AcceptTcpClient())
			{
				var cancelTokenSource = new CancellationTokenSource();
				var cancelToken = cancelTokenSource.Token;
				WriteMessage("Accept connection from client");
				var send = Task.Factory.StartNew(() => SendMessages(client), cancelTokenSource.Token);
				var receive = Task.Factory.StartNew(() =>
				{
					ReceiveMessages(client, cancelToken);
				}, cancelToken);
				send.Wait();
				cancelTokenSource.Cancel();
				receive.Wait();
			}
			listener.Stop();
		}

		private void SendMessages(TcpClient clientSocket)
		{
			while (true)
			{
				var input = Console.ReadLine();
				if (input == "exit")
					return;
				try
				{
					MessageHandler.SendMessages(clientSocket, input);
				}
				catch (Exception ex)
				{
					WriteMessage(ex.ToString());
					return;
				}
			}
		}

		private void ReceiveMessages(TcpClient clientSocket, CancellationToken token)
		{
			while (true)
			{
				if (token.IsCancellationRequested || !clientSocket.Client.Connected)
				{
					WriteMessage("Client has disconnected");
					return;
				}
				var message = MessageHandler.ReceiveMessagesAsync(clientSocket, token).Result;
				WriteMessage($"Client: {message}");
			}
		}

		private void WriteMessage(string message) => MessageWriter(message);
	}
}