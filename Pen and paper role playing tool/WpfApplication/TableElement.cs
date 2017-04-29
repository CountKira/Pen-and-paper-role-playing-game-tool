using MVVM_Framework;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using WpfApplication.Annotations;

namespace WpfApplication
{
    public class TableElement : INotifyPropertyChanged, IEquatable<TableElement>
    {
        private static readonly List<string> UnsendableProperties = new List<string>
        {
            nameof(Size),
            nameof(ImageUrl),
            nameof(XPosition),
            nameof(YPosition),
            nameof(ZoomMultiplier),
        };

        private static double baseSize = 50;
        private static double zoomMultiplier = 1;
        private readonly ContextAction decreaseSizeAction;
        private string imageName;
        private string imageUrl;
        private int sizeMultiplier = 1;
        private double x;
        private double y;

        public TableElement()
        {
            decreaseSizeAction = new ContextAction
            {
                Action = new ActionCommand(DecreaseSizeMethod),
                Name = "Decrease Size",
                Enabled = false
            };
            Actions = new ObservableCollection<ContextAction>
            {
                new ContextAction {Action = new ActionCommand(IncreaseSizeMethod), Name = "Increase Size"},
                decreaseSizeAction,
                new ContextAction { Action = new ActionCommand(ChangePictureMethod), Name = "Change Picture"},
            };
            ImageName = "Background.png";
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [UsedImplicitly]
        public ObservableCollection<ContextAction> Actions { get; }

        public double BaseSize
        {
            get => baseSize;
            set
            {
                baseSize = value;
                OnPropertyChanged(nameof(BaseSize));
                OnPropertyChanged(nameof(Size));
            }
        }

        [UsedImplicitly]
        public string ImageName
        {
            get => imageName;
            set
            {
                imageName = value;
                ImageUrl = PictureHelper.GetFullPictureUrl(ImageName);
                OnPropertyChanged(nameof(ImageName));
            }
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

        [UsedImplicitly]
        public double Size => BaseSize * SizeMultiplier * ZoomMultiplier;

        public int SizeMultiplier
        {
            get => sizeMultiplier;
            set
            {
                if (value < 1 || sizeMultiplier == value) return;
                sizeMultiplier = value;
                decreaseSizeAction.Enabled = SizeMultiplier > 1;
                OnPropertyChanged(nameof(SizeMultiplier));
                OnPropertyChanged(nameof(Size));
            }
        }

        public double X
        {
            get => x;
            set
            {
                //var unzoomedValue = value / zoomMultiplier;
                var unzoomedValue = value;
                if (IsDoubleEqual(unzoomedValue, x)) return;
                x = unzoomedValue;
                OnPropertyChanged(nameof(X));
                OnPropertyChanged(nameof(XPosition));
            }
        }

        [UsedImplicitly]
        public double XPosition => X * ZoomMultiplier;

        public double Y
        {
            get => y;
            set
            {
                var unzoomedValue = value;
                //var unzoomedValue = value / zoomMultiplier;
                if (IsDoubleEqual(unzoomedValue, y)) return;
                y = unzoomedValue;
                OnPropertyChanged(nameof(Y));
                OnPropertyChanged(nameof(YPosition));
            }
        }

        [UsedImplicitly]
        public double YPosition => Y * ZoomMultiplier;

        public double ZoomMultiplier
        {
            get => zoomMultiplier;
            set
            {
                zoomMultiplier = value;
                OnPropertyChanged(nameof(Size));
                OnPropertyChanged(nameof(XPosition));
                OnPropertyChanged(nameof(YPosition));
            }
        }

        public static bool IsPropertySendable(string propertyName) => !UnsendableProperties.Contains(propertyName);

        public void ChangeProperty(TableElementPropertyChangedData tepcd) => typeof(TableElement).GetProperty(tepcd.PropertyName)?.SetValue(this, tepcd.Value);

        public override bool Equals(object obj) => Equals(obj as TableElement);

        public bool Equals(TableElement other) => other != null && IsDoubleEqual(other.x, x) && IsDoubleEqual(other.y, y);

        public override int GetHashCode()
        {
            return X.GetHashCode() + Y.GetHashCode();
        }

        private static bool IsDoubleEqual(double value, double field)
        {
            return (Math.Abs(value - field) < 0.00001);
        }

        private void ChangePictureMethod(object obj)
        {
            var (imageSelected, pictureName) = PictureHelper.ChangePictureUrl();
            if (imageSelected)
            {
                ImageName = pictureName;
                ImageUrl = PictureHelper.GetFullPictureUrl(ImageName);
            }
        }

        private void DecreaseSizeMethod(object obj)
        {
            SizeMultiplier--;
        }

        private void IncreaseSizeMethod(object obj)
        {
            SizeMultiplier++;
        }

        private void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        private void Set(double value, ref double field, string propertyName)
        {
            if (IsDoubleEqual(value, field)) return;
            field = value;
            OnPropertyChanged(propertyName);
        }
    }
}