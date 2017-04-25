using System;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace TCP_Framework
{
    public static class DataHandler
    {
        private const int IntByteSize = 4;

        public static async Task<DataHolder> ReceiveDataAsync(TcpClient client, CancellationToken token)
        {
            var networkStream = client.GetStream();
            token.Register(() => networkStream.Close());
            var sizeLength = await ReceiveDataLengthAsync(networkStream, token);
            var inStream = await ReceiveDataBytesAsync(networkStream, sizeLength, token);
            var data = Serializer.Deserialize<DataHolder>(inStream);
            return data;
        }

        private static async Task<int> ReceiveDataLengthAsync(NetworkStream networkStream, CancellationToken token)
        {
            if (networkStream == null) throw new ArgumentNullException(nameof(networkStream));
            var inStream = new byte[4];
            await networkStream.ReadAsync(inStream, 0, IntByteSize, token);
            var dataSize = BitConverter.ToInt32(inStream, 0);
            return dataSize;
        }

        private static async Task<byte[]> ReceiveDataBytesAsync(NetworkStream networkStream, int dataSize, CancellationToken token)
        {
            if (networkStream == null) throw new ArgumentNullException(nameof(networkStream));
            var inStream = new byte[dataSize];
            var receiveBufferSize = dataSize;
            var dataRead = 0;
            do
            {
                dataRead += await networkStream.ReadAsync(inStream, dataRead, receiveBufferSize - dataRead, token);
            } while (dataRead < dataSize);
            return inStream;
        }

        public static void SendData(TcpClient client, DataHolder dataholder)
        {
            var serializedData = Serializer.Serialize(dataholder);
            var networkStream = client.GetStream();
            var dataByteSize = BitConverter.GetBytes(serializedData.Length);
            networkStream.Write(dataByteSize, 0, IntByteSize);
            networkStream.Write(serializedData, 0, serializedData.Length);
            networkStream.Flush();
        }
    }
}