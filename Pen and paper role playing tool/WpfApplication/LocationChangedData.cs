using System;

namespace WpfApplication
{
    [Serializable]
    public class LocationChangedData
    {
        public double X { get; set; }
        public double Y { get; set; }
        public int Index { get; set; }
    }
}