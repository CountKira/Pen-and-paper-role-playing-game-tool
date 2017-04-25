using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace TCP_Framework
{
    public class Server : IServer
    {
        public TcpListener Listener { get; set; }
        private TcpClient clientSocket;
        public EventHandler<DataReceivedEventArgs> DataReceivedEvent { get; set; }

        public Task EstablishConnection(int port, CancellationToken cancellationToken)
        {
            Listener = new TcpListener(IPAddress.Any, port);
            Listener.Start();
            return Task.Factory.StartNew(() => clientSocket = Listener.AcceptTcpClient(), cancellationToken);
        }

        public Task EstablishConnection(IServer server, CancellationToken cancellationToken)
        {
            Listener = server.Listener;
            return Task.Factory.StartNew(() => clientSocket = Listener.AcceptTcpClient(), cancellationToken);
        }

        public void SendData(DataHolder dataHolder) => DataHandler.SendData(clientSocket, dataHolder);

        public Task<DataHolder> ReceiveData(CancellationToken token)
        {
            if (token.IsCancellationRequested || !clientSocket.Client.Connected)
                return null;
            var receiver = DataHandler.ReceiveDataAsync(clientSocket, token);
            return receiver;
        }
    }
}