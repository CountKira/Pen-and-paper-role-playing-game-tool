using System;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace Pen_and_paper_role_playing_tool
{
    public static class MessageHandler
    {
        private const int intByteSize = 4;

        public static async Task<MessageHolder> ReceiveMessagesAsync(TcpClient client, CancellationToken token)
        {
            var networkStream = client.GetStream();
            token.Register(() => networkStream.Close());
            var messageLength = await ReceiveMessageLengthAsync(networkStream, token);
            var inStream = await ReceiveMessageBytesAsync(networkStream, messageLength, token);
            var messageObj = Serializer.Deserialize<MessageHolder>(inStream);
            return messageObj;
        }

        private static async Task<int> ReceiveMessageLengthAsync(NetworkStream networkStream, CancellationToken token)
        {
            var inStream = new byte[4];
            await networkStream.ReadAsync(inStream, 0, intByteSize, token);
            var messageLength = BitConverter.ToInt32(inStream, 0);
            return messageLength;
        }

        private static async Task<byte[]> ReceiveMessageBytesAsync(NetworkStream networkStream, int messageLength, CancellationToken token)
        {
            var inStream = new byte[messageLength];
            var receiveBufferSize = messageLength;
            int dataRead = 0;
            do
            {
                dataRead += await networkStream.ReadAsync(inStream, dataRead, receiveBufferSize - dataRead, token);
            } while (dataRead < messageLength);
            return inStream;
        }

        public static void SendMessage(TcpClient client, MessageHolder message)
        {
            var serializedMessage = Serializer.Serialize(message);
            var networkStream = client.GetStream();
            var byteSizeMessage = BitConverter.GetBytes(serializedMessage.Length);
            networkStream.Write(byteSizeMessage, 0, intByteSize);
            networkStream.Write(serializedMessage, 0, serializedMessage.Length);
            networkStream.Flush();
        }
    }
}