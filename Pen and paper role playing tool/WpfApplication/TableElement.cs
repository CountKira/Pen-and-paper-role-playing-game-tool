using MVVM_Framework;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using WpfApplication.Annotations;

namespace WpfApplication
{
    public class TableElement : INotifyPropertyChanged, IEquatable<TableElement>
    {
        private readonly ContextAction decreaseSizeAction;
        private int sizeMultiplier = 1;
        private string imageName;
        private double x;
        private double y;
        private string imageUrl;

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

        public double BaseSize { get; set; } = 50;

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

        [UsedImplicitly]
        public double Size => BaseSize * SizeMultiplier;

        [UsedImplicitly]
        public ObservableCollection<ContextAction> Actions { get; }

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

        private void Set(double value, ref double field, string propertyName)
        {
            if (IsDoubleEqual(value, field)) return;
            field = value;
            OnPropertyChanged(propertyName);
        }

        public void ChangeProperty(TableElementPropertyChangedData tepcd) => typeof(TableElement).GetProperty(tepcd.PropertyName)?.SetValue(this, tepcd.Value);

        private static bool IsDoubleEqual(double value, double field)
        {
            return (Math.Abs(value - field) < 0.00001);
        }

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

        private void IncreaseSizeMethod(object obj)
        {
            SizeMultiplier++;
        }

        private void DecreaseSizeMethod(object obj)
        {
            SizeMultiplier--;
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

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        public override bool Equals(object obj) => Equals(obj as TableElement);

        public bool Equals(TableElement other) => other != null && IsDoubleEqual(other.x, x) && IsDoubleEqual(other.y, y);

        public override int GetHashCode()
        {
            return X.GetHashCode() + Y.GetHashCode();
        }

        public static bool IsPropertySendable(string propertyName) => propertyName != nameof(Size) && propertyName != nameof(ImageUrl);
    }
}