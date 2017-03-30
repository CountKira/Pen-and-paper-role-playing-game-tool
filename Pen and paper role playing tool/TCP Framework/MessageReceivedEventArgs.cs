using System;

namespace TCP_Framework
{
    public class DataReceivedEventArgs : EventArgs
    {
        public DataHolder Dataholder { get; }

        public DataReceivedEventArgs(DataHolder dataholder)
        {
            Dataholder = dataholder;
        }
    }
}