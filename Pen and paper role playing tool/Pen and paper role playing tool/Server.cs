using System;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace Pen_and_paper_role_playing_tool
{
	public class Server : IClientServer
	{
		private TcpListener listener;
		private TcpClient clientSocket;
		public Task EstablishConnection(int port)
		{
			listener = new TcpListener(IPAddress.Any, port);
			listener.Start();
			return Task.Factory.StartNew(() => clientSocket = listener.AcceptTcpClient());
		}
		public Task EstablishConnection(Server server)
		{
			var listener = server.listener;
			return Task.Factory.StartNew(() => clientSocket = listener.AcceptTcpClient());
		}
		public void SendMessage(string input)
		{
			MessageHandler.SendMessage(clientSocket, input);
		}

		public Task<string> ReceiveMessage(CancellationToken token)
		{
			if (token.IsCancellationRequested || !clientSocket.Client.Connected)
				return null;
			var receiver = MessageHandler.ReceiveMessagesAsync(clientSocket, token);
			return receiver;
		}

		public void StopServer()
		{
			listener.Stop();
		}
	}
}