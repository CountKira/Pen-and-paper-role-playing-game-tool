using System;

namespace Pen_and_paper_role_playing_tool
{
    public class WriterEventArgs : EventArgs
    {
        public string Message { get; }

        public WriterEventArgs(string message)
        {
            Message = message;
        }
    }
}