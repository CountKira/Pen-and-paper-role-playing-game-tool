using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Input;
using Microsoft.Win32;
using MVVM_Framework;
using WpfApplication.ViewModel;

namespace WpfApplication
{
    [Serializable]
    public class TableElement : INotifyPropertyChanged, IEquatable<TableElement>
    {
        private double x;
        private double y;
        private double baseSize = 50;
        private double sizeMultiplier = 1;
        private string imageUrl = @"C:\Users\Riedmiller Stefan\Pictures\Doctors.jpg";
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
        public double Size
        {
            get => baseSize * sizeMultiplier;
            set => Set(value, ref baseSize, nameof(Size));
        }
        public string ImageUrl
        {
            get => imageUrl;
            set
            {
                if (value == imageUrl) return;
                imageUrl = value;
                OnPropertyChanged(nameof(ImageUrl));
            }
        }

        public ObservableCollection<ContextAction> Actions { get; }
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

        public TableElement()
        {
            Actions = new ObservableCollection<ContextAction>
            {
                new ContextAction {Action = new ActionCommand(ChangePictureMethod), Name = "Change Picture"},
                new ContextAction {Action = new ActionCommand(IncreaseSizeMethod), Name = "Increase Size"},
                new ContextAction {Action = new ActionCommand(DecreaseSizeMethod), Name = "Decrease Size"},
            };
        }

        private void IncreaseSizeMethod(object obj)
        {
            sizeMultiplier += 1;
            OnPropertyChanged(nameof(Size));
        }
        private void DecreaseSizeMethod(object obj)
        {
            if (sizeMultiplier <= 1) return;

            sizeMultiplier -= 1;
            OnPropertyChanged(nameof(Size));
        }

        private void ChangePictureMethod(object obj)
        {
            var (imageSelected, imageUrl) = PictureChanger.ChangePictureUrl();
            if (imageSelected)
                ImageUrl = imageUrl;
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