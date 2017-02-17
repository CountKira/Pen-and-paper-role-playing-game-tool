using System;
using System.Text;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.IO;

namespace Pen_and_paper_role_playing_tool
{
	public class Client : IClientServer, IDisposable
	{

		private TcpClient client = new TcpClient();
		private int port;
		private string address;

		public EventHandler<WriterEventArgs> Writer { get; set; }
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
				StartReceivingAsync();
				return true;
			}
			catch (Exception)
			{
				return false;
			}
		}

		private async void StartReceivingAsync()
		{
			var cancellationToken = new CancellationToken();
			while (true)
			{
				try
				{
					var message = await MessageHandler.ReceiveMessagesAsync(client, cancellationToken);
					Writer(this, new WriterEventArgs(message));

				}
				catch (IOException)
				{
					Writer(this, new WriterEventArgs("Verbindung zum Server getrennt."));
					return;
				}
			}
		}

		public void SendMessage(string input)
		{
			MessageHandler.SendMessage(client, input);
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