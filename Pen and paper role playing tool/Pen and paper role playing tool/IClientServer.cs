using System;

namespace Pen_and_paper_role_playing_tool
{
    public interface IClientServer
    {
        EventHandler<WriterEventArgs> Writer { get; set; }

        void SendMessage(MessageHolder message);
    }
}