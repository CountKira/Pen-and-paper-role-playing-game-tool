using System;

namespace TCP_Framework
{
    public class DataReceivedEventArgs : EventArgs
    {
        public DateHolder Dataholder { get; }

        public DataReceivedEventArgs(DateHolder dataholder)
        {
            Dataholder = dataholder;
        }
    }
}