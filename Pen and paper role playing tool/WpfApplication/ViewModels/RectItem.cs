using System;
using System.ComponentModel;

namespace WpfApplication.ViewModel
{
    [Serializable]
    public class RectItem : INotifyPropertyChanged
    {
        private double x, y;
        public double X { get => x; set { x = value; OnPropertyChanged(nameof(x)); } }
        public double Y { get => y; set { y = value; OnPropertyChanged(nameof(y)); } }
        public double Width { get; set; }
        public double Height { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs((name)));
        }
    }
}