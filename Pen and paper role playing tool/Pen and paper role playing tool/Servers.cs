using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Pen_and_paper_role_playing_tool
{
	public class Servers
	{
		List<Server> servers = new List<Server>();
		public event Action<string> Writer;
		public Servers()
		{
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
					await server.EstablishConnection(8888);
				new Thread(() => ReceiveMessagesAsync(token, server)).Start();
				servers.Add(server);
			}
		}

		private async void ReceiveMessagesAsync(CancellationToken token, Server server)
		{
			while (true)
			{
				var message = await server.ReceiveMessage(token);
				foreach (var serve in servers)
				{
					if (serve!=server)
						serve.SendMessage(message);
				}
				Writer(message);
			};
		}

		public void SendMessage(string input)
		{
			foreach (var server in servers)
				server.SendMessage(input);
		}
	}
}
