using MVVM_Framework;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using Microsoft.Win32;
using WpfApplication.Annotations;
using WpfApplication.ViewModel;

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
        private readonly CharacterSheetViewModel characterSheetViewModel = new CharacterSheetViewModel();
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
                new ContextAction {Action = new ActionCommand(SaveElementMethod), Name = "Save Element"},
                new ContextAction {Action = new ActionCommand(LoadElementMethod), Name = "Load Element"},
                new ContextAction {Action = new ActionCommand(IncreaseSizeMethod), Name = "Increase Size"},
                decreaseSizeAction,
                new ContextAction { Action = new ActionCommand(ChangePictureMethod), Name = "Change Picture"},
                new ContextAction{Action=new ActionCommand(DeleteMethod), Name="Delete"},
                new ContextAction{Action=new ActionCommand(ShowCharacterSheetMethod), Name="Character Sheet"},
            };
            ImageName = "Background.png";
            characterSheetViewModel.PropertyChanged += (s, e) =>
            {
                var characterSheetChange = new CharacterSheetChange { Item = s as Item, TableElement = this };
                PropertyChanged?.Invoke(characterSheetChange, e);
            };
        }

        private void SaveElementMethod(object obj)
        {
            var dialog = new SaveFileDialog { Filter = "Element files (*.element)|*.element" };
            if (dialog.ShowDialog() != true) return;
            var tableElementData = TableElementData.ConvertFromTableElement(this);
            var fileName = dialog.FileName;
            try
            {
                XmlSerializerHelper.SaveXml(tableElementData, fileName);
            }
            catch (Exception)
            {
                MessageBox.Show($"Could not save as {fileName}.");
            }
        }

        private void LoadElementMethod(object obj)
        {
            var dialog = new OpenFileDialog { Filter = "Element files (*.element)|*.element" };
            if (dialog.ShowDialog() != true) return;
            var fileName = dialog.FileName;
            try
            {
                var tableElementData = XmlSerializerHelper.ReadXml<TableElementData>(fileName);
                CharacterSheet = tableElementData.CharacterSheet;
                ImageName = tableElementData.ImageName;
                SizeMultiplier = tableElementData.SizeMultiplier;
            }
            catch (Exception)
            {
                MessageBox.Show($"Could not load the file: {fileName}.");
            }
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

        public List<Item> CharacterSheet
        {
            get => characterSheetViewModel.Items;
            set => characterSheetViewModel.Items = value;
        }

        public ObservableCollection<TableElement> Container { get; set; }

        public IDialogService DialogService { get; set; }

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

        public void ChangeProperty(TableElementPropertyChangedData tepcd)
        {
            switch (tepcd.Value)
            {
                case Item item:
                    characterSheetViewModel.SetItem(item);
                    break;

                default:
                    typeof(TableElement).GetProperty(tepcd.PropertyName)?.SetValue(this, tepcd.Value);
                    break;
            }
        }

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

        private void DeleteMethod(object obj) => DispatcherHelper.Invoke(() => Container.Remove(this));

        private void IncreaseSizeMethod(object obj)
        {
            SizeMultiplier++;
        }

        private void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        private void ShowCharacterSheetMethod(object obj)
        {
            DialogService.Show(characterSheetViewModel);
        }
    }
}