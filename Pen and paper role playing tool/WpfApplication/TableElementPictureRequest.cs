using System;

namespace WpfApplication
{
    [Serializable]
    internal class TableElementPictureRequest
    {
        public string PictureName { get; set; }
        public int Index { get; set; }
    }
}