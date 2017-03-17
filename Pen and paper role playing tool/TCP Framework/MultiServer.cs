using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace TCP_Framework
{
    public class MultiServer : IClientServer
    {
        private List<Server> servers = new List<Server>();
        public EventHandler<DataReceivedEventArgs> DataReceivedEvent { get; set; }
        public int PortNumber;

        public MultiServer(int portNumber)
        {
            PortNumber = portNumber;
            OpenNewServerAsync();
        }

        async private void OpenNewServerAsync()
        {
            var token = new CancellationToken();
            while (true)
            {
                var server = new Server();
                if (servers.Count > 0)
                    await server.EstablishConnection(servers[0]);
                else
                {
                    await server.EstablishConnection(PortNumber);
                }
                new Thread(() => ReceiveDatasAsync(token, server)).Start();
                servers.Add(server);
            }
        }

        private async void ReceiveDatasAsync(CancellationToken token, Server server)
        {
            while (true)
            {
                try
                {
                    var data = await server.ReceiveData(token);
                    foreach (var serveritem in servers)
                        if (serveritem != server)
                            serveritem.SendData(data);
                    DataReceivedEvent(this, new DataReceivedEventArgs(data));
                }
                catch (Exception)
                {
                    //TODO: Have to check if this needs to be thread safe
                    DataReceivedEvent(this, new DataReceivedEventArgs(new DateHolder { Tag = "Text", Data = "Verbindung zu Client getrennt." }));
                    servers.Remove(server);
                    return;
                }
            };
        }

        public void SendData(DateHolder dataholder)
        {
            foreach (var server in servers)
                server.SendData(dataholder);
        }
    }
}