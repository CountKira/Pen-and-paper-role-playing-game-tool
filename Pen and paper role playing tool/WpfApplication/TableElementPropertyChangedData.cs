using System;

namespace WpfApplication
{
    [Serializable]
    public class TableElementPropertyChangedData
    {
        public string PropertyName { get; set; }
        public object Value { get; set; }
        public int Index { get; set; }
    }
}