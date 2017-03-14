using System;

namespace Pen_and_paper_role_playing_tool
{
    [Serializable]
    public class MessageHolder
    {
        public object Message { get; set; }
        public string MessageType { get; set; }
    }
}