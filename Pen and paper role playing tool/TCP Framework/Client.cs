using System;
using System.IO;
using System.Net.Sockets;
using System.Threading;

namespace TCP_Framework
{
    public class Client : IClientServer, IDisposable
    {
        private readonly TcpClient client = new TcpClient();
        private readonly int port;
        private readonly string address;

        public EventHandler<DataReceivedEventArgs> DataReceivedEvent { get; set; }

        public Client(int port, string address)
        {
            this.port = port;
            this.address = address;
        }

        public bool TryConnectingToServer()
        {
            try
            {
                client.Connect(address, port);
                StartReceivingAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private async void StartReceivingAsync()
        {
            var cancellationToken = new CancellationToken();
            while (true)
            {
                try
                {
                    var data = await DataHandler.ReceiveDataAsync(client, cancellationToken);
                    DataReceivedEvent(this, new DataReceivedEventArgs(data));
                }
                catch (IOException)
                {
                    DataReceivedEvent(this, new DataReceivedEventArgs(new DataHolder { Tag = "Text", Data = "Verbindung zum Server getrennt." }));
                    return;
                }
            }
        }

        public void SendData(DataHolder dataholder) => DataHandler.SendData(client, dataholder);

        #region IDisposable Support

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2213:DisposableFieldsShouldBeDisposed", MessageId = "client")]
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                client?.Close();
            }
        }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion IDisposable Support
    }
}