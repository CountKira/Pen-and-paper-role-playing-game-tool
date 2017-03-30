using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace TCP_Framework
{
    public class Server : IClientServer
    {
        private TcpListener listener;
        private TcpClient clientSocket;
        public EventHandler<DataReceivedEventArgs> DataReceivedEvent { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public Task EstablishConnection(int port)
        {
            listener = new TcpListener(IPAddress.Any, port);
            listener.Start();
            return Task.Factory.StartNew(() => clientSocket = listener.AcceptTcpClient());
        }

        public Task EstablishConnection(Server server)
        {
            var listener = server.listener;
            return Task.Factory.StartNew(() => clientSocket = listener.AcceptTcpClient());
        }

        public void SendData(DataHolder dataHolder) => DataHandler.SendData(clientSocket, dataHolder);

        public Task<DataHolder> ReceiveData(CancellationToken token)
        {
            if (token.IsCancellationRequested || !clientSocket.Client.Connected)
                return null;
            var receiver = DataHandler.ReceiveDataAsync(clientSocket, token);
            return receiver;
        }

        public void StopServer() => listener.Stop();
    }
}