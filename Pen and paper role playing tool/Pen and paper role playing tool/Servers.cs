using System;
using System.Collections.Generic;
using System.Configuration;
using System.Threading;
using System.Threading.Tasks;

namespace Pen_and_paper_role_playing_tool
{
	public class Servers : IClientServer
	{
		List<Server> servers = new List<Server>();
		public EventHandler<WriterEventArgs> Writer { get; set; }
		public int PortNumber;

		public Servers(int portNumber)
		{
			PortNumber = portNumber;
			OpenNewServerAsync();
		}

		async private void OpenNewServerAsync()
		{
			var token = new CancellationToken();
			while (true)
			{
				var server = new Server();
				if (servers.Count > 0)
					await server.EstablishConnection(servers[0]);
				else
				{
					await server.EstablishConnection(PortNumber);
				}
				new Thread(() => ReceiveMessagesAsync(token, server)).Start();
				servers.Add(server);
			}
		}

		private async void ReceiveMessagesAsync(CancellationToken token, Server server)
		{
			while (true)
			{
				try
				{
					var message = await server.ReceiveMessage(token);
					foreach (var serveritem in servers)
					{
						if (serveritem != server)
							serveritem.SendMessage(message);
					}
					Writer(this, new WriterEventArgs(message));
				}
				catch (Exception)
				{
					//TODO: Have to check if this needs to be thread safe
					servers.Remove(server);
					return;
				}
			};
		}

		public void SendMessage(string input)
		{
			foreach (var server in servers)
				server.SendMessage(input);
		}

		public Task<string> ReceiveMessage(CancellationToken token)
		{
			throw new NotImplementedException();
		}
	}
}
