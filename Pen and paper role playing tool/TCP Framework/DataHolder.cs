using System;

namespace TCP_Framework
{
    [Serializable]
    public class DataHolder
    {
        public object Data { get; set; }
        public string Tag { get; set; }
    }
}