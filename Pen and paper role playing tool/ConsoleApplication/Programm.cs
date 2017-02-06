using NATUPNPLib;
using System;
using System.Net;
using System.Threading;
using System.Linq;
using System.Threading.Tasks;
using Pen_and_paper_role_playing_tool;

namespace ConsoleApplication
{
	internal class Program
	{
		private const int portNumber = 8888;

		private static void Main(string[] args)
		{
			InternalTest();

			Console.Write("Press any key to exit...");
			Console.ReadLine();
		}

		private static void InternalTest()
		{
			while (true)
			{
				Console.WriteLine("1. Server\n2. Client\n3. Exit");
				var input = Console.ReadLine();
				switch (input)
				{
					case "1": RunServer(); break;
					case "2": RunClient(); break;
					case "3": return;
					default: Console.WriteLine("Please enter 1, 2 or 3."); break;
				}
			}
		}

		private static void RunClient()
		{
			Console.Write("Pleas enter the IP address of the server: ");
			var address = Console.ReadLine();
			var client = new Client(portNumber, address);
			Console.WriteLine("Client tries to connect");
			var success = client.TryConnectingToServer();

			if (success)
			{
				var cancelTokenSource = new CancellationTokenSource();
				var cancelToken = cancelTokenSource.Token;
				Console.WriteLine("Client has connected");
				var receive = Task.Factory.StartNew(() => client.ReceiveMessage(cancelToken), cancelToken);
				while (!receive.IsCanceled)
				{
					var message = Console.ReadLine();
					if (message.ToLower() == "exit")
						break;
					client.SendMessage(message);
				}
				cancelTokenSource.Cancel();
				receive.Wait();
				client.DisconnectFromServer();
				Console.WriteLine("Client closed");
			}
			else
			{
				Console.WriteLine($"Unable to connect to server with the address: {address}");
			}
		}

		private static void RunServer()
		{
			var upnpnat = DisplayAdresses();
			var mappings = upnpnat.StaticPortMappingCollection;
			SetupUPnPMapping(mappings);
			var server = new Server();
			server.EstablishConnection(portNumber);
			Console.WriteLine("Server closed.");
			try
			{
				mappings.Remove(portNumber, "TCP");
			}
			catch (Exception)
			{
			}
		}

		private static UPnPNATClass DisplayAdresses()
		{
			var addresses = Dns.GetHostAddresses(Dns.GetHostName());
			foreach (var address in addresses)
			{
				Console.WriteLine(address);
			}
			var upnpnat = new UPnPNATClass();
			return upnpnat;
		}

		private static void SetupUPnPMapping(IStaticPortMappingCollection mappings)
		{
			Console.Write("Please enter your IP Address for port forwarding or leave blank: ");
			var input = Console.ReadLine();
			if (string.IsNullOrEmpty(input))
			{
				return;
			}
			mappings.Add(portNumber, "TCP", portNumber, input, true, "Local Game Server");
		}
	}
}