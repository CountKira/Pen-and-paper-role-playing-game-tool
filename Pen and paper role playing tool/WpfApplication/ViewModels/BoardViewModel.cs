using System;
using System.Windows;
using MVVM_Framework;
using System.ComponentModel;
using TCP_Framework;

namespace WpfApplication.ViewModel
{
    internal class BoardViewModel : INotifyPropertyChanged
    {
        private double left, top;
        private IClientServer clientServer;

        public BoardViewModel(IClientServer clientServer)
        {
            //if (clientServer is MultiServer)
            //{
            this.clientServer = clientServer;
            //}
        }

        public double Left
        {
            get => left;
            set
            {
                if (left != value)
                {
                    left = value;
                    clientServer?.SendData(new DateHolder { Tag = "Position", Data = new Point(left, top) });
                    OnPropertyChanged(nameof(Left));
                }
            }
        }

        public double Top
        {
            get => top;
            set
            {
                if (top != value)
                {
                    top = value;
                    clientServer?.SendData(new DateHolder { Tag = "Position", Data = new Point(left, top) });
                    OnPropertyChanged(nameof(Top));
                }
            }
        }

        public int SelectedElementIndex { get; internal set; }

        public void ReceiveNewPosition(Point point)
        {
            top = point.Y;
            left = point.X;
            OnPropertyChanged(nameof(Left));
            OnPropertyChanged(nameof(Top));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        internal void DataReception(object sender, DataReceivedEventArgs data)
        {
            var dataholder = data.Dataholder;
            if (dataholder.Tag == "Position")
            {
                var point = (Point)dataholder.Data;
                ReceiveNewPosition(point);
            }
        }
    }
}