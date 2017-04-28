using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace TCP_Framework
{
    public class MultiServer : IClientServer, IDisposable
    {
        public SynchronizedCollection<IServer> Servers { get; } = new SynchronizedCollection<IServer>();
        public EventHandler<DataReceivedEventArgs> DataReceivedEvent { get; set; }
        private readonly int portNumber;
        public bool Running { get; private set; } = true;

        public MultiServer(int portNumber)
        {
            this.portNumber = portNumber;
            OpenNewServerAsync();
        }

        private readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

        public void Dispose() => cancellationTokenSource.Cancel();

        private async void OpenNewServerAsync()
        {
            while (!cancellationTokenSource.IsCancellationRequested)
            {
                var server = ServerFactory.GenerateServer();
                var token = cancellationTokenSource.Token;
                if (Servers.Count > 0)
                    await server.EstablishConnection(Servers[0], token);
                else
                    await server.EstablishConnection(portNumber, token);
                Servers.Add(server);
                var newThread = new Thread(() => ReceiveDatasAsync(token, server));
                newThread.Start();
            }
            cancellationTokenSource.Cancel();
        }

        public void AddServer(IServer server)
        {
            Servers.Add(server);
            var token = new CancellationToken();
            new Thread(() => ReceiveDatasAsync(token, server)).Start();
        }

        private async void ReceiveDatasAsync(CancellationToken token, IServer server)
        {
            while (true)
            {
                try
                {
                    var data = await server.ReceiveData(token);
                    foreach (var serveritem in Servers)
                        if (serveritem != server)
                            serveritem.SendData(data);
                    DataReceivedEvent(this, new DataReceivedEventArgs(data));
                }
                catch (IOException e)
                {
                    lock (this)
                    {
                        DataReceivedEvent(this, new DataReceivedEventArgs(new DataHolder { Tag = "Server", Data = e }));
                        Servers.Remove(server);
                    }
                    return;
                }
            }
        }

        public void SendData(DataHolder dataholder)
        {
            foreach (var server in Servers)
                server.SendData(dataholder);
        }
    }
}