using System;
using System.ComponentModel;

namespace WpfApplication
{
    [Serializable]
    public class TableElement : INotifyPropertyChanged, IEquatable<TableElement>
    {
        private double x, y;

        public double X
        {
            get => x;
            set => Set(value, ref x, nameof(X));
        }

        public double Y
        {
            get => y;
            set => Set(value, ref y, nameof(Y));
        }

        private void Set(double value, ref double field, string propertyName)
        {
            if (value != field)
            {
                field = value;
                OnPropertyChanged(propertyName);
            }
        }

        [field: NonSerialized]
        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        public override bool Equals(object obj) => Equals(obj as TableElement);

        public bool Equals(TableElement other) => other != null && other.x == x && other.y == y;

        public override int GetHashCode() => base.GetHashCode();
    }
}