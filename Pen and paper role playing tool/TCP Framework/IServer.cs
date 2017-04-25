using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace TCP_Framework
{
    public interface IServer : IClientServer
    {
        Task<DataHolder> ReceiveData(CancellationToken token);
        TcpListener Listener { get; }
        Task EstablishConnection(IServer server, CancellationToken cancellationToken);
        Task EstablishConnection(int portNumber, CancellationToken cancellationToken);
    }
}