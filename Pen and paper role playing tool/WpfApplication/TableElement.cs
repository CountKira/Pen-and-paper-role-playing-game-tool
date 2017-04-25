using System;
using System.ComponentModel;

namespace WpfApplication
{
    [Serializable]
    public class TableElement : INotifyPropertyChanged, IEquatable<TableElement>
    {
        private double x;
        private double y;

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
            if (IsDoubleEqual(value, field)) return;
            field = value;
            OnPropertyChanged(propertyName);
        }

        private static bool IsDoubleEqual(double value, double field)
        {
            return (Math.Abs(value - field) < 0.00001);
        }

        [field: NonSerialized]
        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        public override bool Equals(object obj) => Equals(obj as TableElement);

        public bool Equals(TableElement other) => other != null && IsDoubleEqual(other.x, x) && IsDoubleEqual(other.y, y);

        public override int GetHashCode()
        {
            return X.GetHashCode() + Y.GetHashCode();
        }
    }
}