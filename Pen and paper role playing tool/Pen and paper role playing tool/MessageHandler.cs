using System.Text;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Threading;
using System;

namespace Pen_and_paper_role_playing_tool
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
				var message = Encoding.UTF8.GetString(inStream);
				var length = message.IndexOf(stringEndCharacter);
				message = message.Substring(0, length);
				return message;
			}
			catch (Exception)
			{
				return "";
			}
		}

		public static void SendMessage(TcpClient client, string message)
		{
			var networkStream = client.GetStream();
			var sendBytes = Encoding.UTF8.GetBytes(message);
			networkStream.Write(sendBytes, 0, sendBytes.Length);
			networkStream.Flush();
		}
	}
}