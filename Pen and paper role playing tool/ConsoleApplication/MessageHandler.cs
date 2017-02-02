using System.Text;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Threading;
using System;

namespace ConsoleApplication
{
	internal class MessageHandler
	{
		private const char stringEndCharacter = '\0';

		public static async Task<string> ReceiveMessagesAsync(TcpClient client, CancellationToken token)
		{
			try
			{
				var inStream = new byte[65536];
				var receiveBufferSize = client.ReceiveBufferSize;
				var networkStream = client.GetStream();
				token.Register(() => networkStream.Close());
				await networkStream.ReadAsync(inStream, 0, receiveBufferSize, token);
				var message = Encoding.ASCII.GetString(inStream);
				var length = message.IndexOf(stringEndCharacter);
				message = message.Substring(0, length);
				return message;
			}
			catch (Exception e)
			{
				return string.Empty;
			}
		}

		public static void SendMessages(TcpClient client, string message)
		{
			var networkStream = client.GetStream();
			var sendBytes = Encoding.ASCII.GetBytes(message);
			networkStream.Write(sendBytes, 0, sendBytes.Length);
			networkStream.Flush();
		}
	}
}