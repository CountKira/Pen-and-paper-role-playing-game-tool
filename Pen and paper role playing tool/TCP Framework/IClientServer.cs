using System;

namespace TCP_Framework
{
    public interface IClientServer
    {
        EventHandler<DataReceivedEventArgs> DataReceivedEvent { get; set; }

        void SendData(DataHolder dataholder);
    }
}