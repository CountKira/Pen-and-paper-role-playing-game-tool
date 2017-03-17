using System;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace TCP_Framework
{
    public static class DataHandler
    {
        private const int intByteSize = 4;

        public static async Task<DateHolder> ReceiveDataAsync(TcpClient client, CancellationToken token)
        {
            var networkStream = client.GetStream();
            token.Register(() => networkStream.Close());
            var SizeLength = await ReceiveDataLengthAsync(networkStream, token);
            var inStream = await ReceiveDataBytesAsync(networkStream, SizeLength, token);
            var data = Serializer.Deserialize<DateHolder>(inStream);
            return data;
        }

        private static async Task<int> ReceiveDataLengthAsync(NetworkStream networkStream, CancellationToken token)
        {
            var inStream = new byte[4];
            await networkStream.ReadAsync(inStream, 0, intByteSize, token);
            var dataSize = BitConverter.ToInt32(inStream, 0);
            return dataSize;
        }

        private static async Task<byte[]> ReceiveDataBytesAsync(NetworkStream networkStream, int dataSize, CancellationToken token)
        {
            var inStream = new byte[dataSize];
            var receiveBufferSize = dataSize;
            int dataRead = 0;
            do
            {
                dataRead += await networkStream.ReadAsync(inStream, dataRead, receiveBufferSize - dataRead, token);
            } while (dataRead < dataSize);
            return inStream;
        }

        public static void SendData(TcpClient client, DateHolder dataholder)
        {
            var serializedData = Serializer.Serialize(dataholder);
            var networkStream = client.GetStream();
            var dataByteSize = BitConverter.GetBytes(serializedData.Length);
            networkStream.Write(dataByteSize, 0, intByteSize);
            networkStream.Write(serializedData, 0, serializedData.Length);
            networkStream.Flush();
        }
    }
}