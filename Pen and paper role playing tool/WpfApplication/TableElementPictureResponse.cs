using System;

namespace WpfApplication
{
    [Serializable]
    public class TableElementPictureResponse
    {
        public byte[] PictureData { get; set; }
        public int Index { get; set; }
        public string PictureName { get; set; }
    }
}