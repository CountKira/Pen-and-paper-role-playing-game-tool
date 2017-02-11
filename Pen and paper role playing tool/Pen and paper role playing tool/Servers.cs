using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Pen_and_paper_role_playing_tool
{
	class Servers:IClientServer
	{
		List<Server> servers;
		public event Action<string> Writer;
		public Task<string> ReceiveMessage(CancellationToken token)
		{

			//Let every server who does not already receive messages start receiving;
			//Wait until one of the servers got a message
			//Send the Message to all other Servers
			//Return the received message
			return null;
		}
		public void SendMessage(string input)
		{
			foreach (var server in servers)
			{
				server.SendMessage(input);
			}
		}
	}
}
