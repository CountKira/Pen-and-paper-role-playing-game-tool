using System;

namespace WpfApplication.ViewModel
{
    [Serializable]
    internal class TableElementPictureRequest
    {
        public string PictureName { get; set; }
        public int Index { get; set; }
    }
}