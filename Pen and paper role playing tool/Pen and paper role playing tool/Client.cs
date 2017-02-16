using System;
using System.Text;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace Pen_and_paper_role_playing_tool
{
	public class Client : IClientServer, IDisposable
	{

		private TcpClient client = new TcpClient();
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
				client.Connect(address, port);
				return true;
			}
			catch (Exception)
			{
				return false;
			}
		}

		public void SendMessage(string input)
		{
			MessageHandler.SendMessage(client, input);
		}

		public Task<string> ReceiveMessage(CancellationToken token)
		{
			return MessageHandler.ReceiveMessagesAsync(client, token);
		}

		#region IDisposable Support
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2213:DisposableFieldsShouldBeDisposed", MessageId = "client")]
		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
				client?.Close();
			}
		}

		// This code added to correctly implement the disposable pattern.
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}
		#endregion
	}
}