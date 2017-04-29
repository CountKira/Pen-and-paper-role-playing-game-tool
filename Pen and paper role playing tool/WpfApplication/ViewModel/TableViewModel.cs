using System;
using MVVM_Framework;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using TCP_Framework;
using WpfApplication.Annotations;

namespace WpfApplication.ViewModel
{
    public sealed class TableViewModel : INotifyPropertyChanged
    {
        private const string TagAddCircle = "Add Circle";

        private string backgroundImageUrl;

        private string imageName;

        private bool setGridSizeActiv;
        private double sizeValue = 1;

        private bool triggerOnPropertyChanged = true;

        private Point? upperLeftCorner;

        public TableViewModel()
        {
            NewTableElementCommand = new ActionCommand(NewTableElementMethod);
            ChangeBackgroundCommand = new ActionCommand(ChangeBackgoundMethod);
            SetGridSizeReceiveMouseClickCommand = new ActionCommand(SetGridSizeReceiveMouseClickMethod);
            SetGridSizeCommand = new ActionCommand(SetGridSizeCommandMethod);
            GetContextMenuPositionCommand = new ActionCommand(GetContextMenuPositionMethod);
            Actions = new ObservableCollection<ContextAction>
            {
                new ContextAction{Name = "Add Element",Action = new ActionCommand(NewTableElementMethod)}
            };
            ImageName = "Background.png";
        }

        private Point rightMouseClickPosition;

        private void GetContextMenuPositionMethod(object obj) => rightMouseClickPosition = Mouse.GetPosition(obj as IInputElement);

        public event PropertyChangedEventHandler PropertyChanged;

        [UsedImplicitly]
        public ObservableCollection<ContextAction> Actions { get; }

        [UsedImplicitly]
        public string BackgroundImageUrl
        {
            get => backgroundImageUrl;
            set
            {
                backgroundImageUrl = value;
                OnPropertyChanged(nameof(BackgroundImageUrl));
            }
        }

        [UsedImplicitly]
        public double BackgroundSize => SizeValue * 1920;

        [UsedImplicitly]
        public ICommand ChangeBackgroundCommand { get; }

        [UsedImplicitly]
        public ICommand GetContextMenuPositionCommand { get; }

        public IClientServer ClientServer { private get; set; }

        public string ImageName
        {
            private get => imageName;
            set
            {
                imageName = value;
                var fullPictureUrl = PictureHelper.GetFullPictureUrl(ImageName);
                if (!File.Exists(fullPictureUrl))
                {
                    SendData("PictureRequest", ImageName);
                    return;
                }
                BackgroundImageUrl = fullPictureUrl;
                OnPropertyChanged(nameof(ImageName));
            }
        }

        public ICommand NewTableElementCommand { get; }

        [UsedImplicitly]
        public ICommand SetGridSizeCommand { get; }

        [UsedImplicitly]
        public double SizeValue
        {
            get => sizeValue;
            set
            {
                sizeValue = value;
                OnPropertyChanged(nameof(SizeValue));
                OnPropertyChanged(nameof(BackgroundSize));
                foreach (var tableElement in TableElements)
                {
                    tableElement.ZoomMultiplier = SizeValue;
                }
            }
        }

        public ObservableCollection<TableElement> TableElements { get; private set; } = new ObservableCollection<TableElement>();

        [UsedImplicitly]
        public ICommand SetGridSizeReceiveMouseClickCommand { get; }

        public void SetTableElementPosition(Point position, TableElement changedElement)
        {
            changedElement.X = position.X / SizeValue;
            changedElement.Y = position.Y / SizeValue;
        }

        public void Add(TableElementData tableElementData)
        {
            var newTableElement = TableElementData.ConvertToTableElement(tableElementData);
            DispatcherHelper.Invoke(() => AddTableElement(newTableElement));
        }

        public void ChangeTableElement(TableElementPropertyChangedData tepcd)
        {
            var tableElement = TableElements[tepcd.Index];
            lock (this)
            {
                if (tepcd.PropertyName == nameof(TableElement.ImageName))
                {
                    var pictureName = tepcd.Value as string;
                    var fileName = PictureHelper.GetFullPictureUrl(pictureName);
                    if (!File.Exists(fileName))
                    {
                        SendData("TableElementPictureRequest",
                            new TableElementPictureRequest { PictureName = pictureName, Index = tepcd.Index });
                        return;
                    }
                }
                triggerOnPropertyChanged = false;
                tableElement.ChangeProperty(tepcd);
                triggerOnPropertyChanged = true;
            }
        }

        public TableViewData GetData()
        {
            var tableElementDatas = from element in TableElements
                                    select TableElementData.ConvertFromTableElement(element);
            return new TableViewData
            {
                ImageName = ImageName,
                Elements = tableElementDatas.ToList()
            };
        }

        public void ReceivePictureData(byte[] bytes)
        {
            PictureHelper.SavePicture(bytes, imageName);
            ImageName = imageName;
        }

        public void SetData(TableViewData tvm)
        {
            ImageName = tvm.ImageName;
            OnPropertyChanged(nameof(BackgroundImageUrl));
            var newTableElements = from elementData in tvm.Elements
                                   select TableElementData.ConvertToTableElement(elementData);
            var newTableElementsCollection = newTableElements.ToList();
            foreach (var element in newTableElementsCollection)
                element.PropertyChanged += Element_PropertyChanged;
            TableElements = new ObservableCollection<TableElement>(newTableElementsCollection);
        }

        public void SetTableElementPicture(TableElementPictureResponse tableElementPictureResponse)
        {
            PictureHelper.SavePicture(tableElementPictureResponse.PictureData, tableElementPictureResponse.PictureName);
            TableElements[tableElementPictureResponse.Index].ImageName = tableElementPictureResponse.PictureName;
        }

        private void AddTableElement(TableElement tableElement)
        {
            tableElement.PropertyChanged += Element_PropertyChanged;
            TableElements.Add(tableElement);
        }

        private void ChangeBackgoundMethod(object obj)
        {
            var (imageSelected, pictureName) = PictureHelper.ChangePictureUrl();
            if (!imageSelected) return;
            ImageName = pictureName;
            SendData("BackgroundChanged", ImageName);
        }

        private void Element_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            lock (this)
                if (triggerOnPropertyChanged && TableElement.IsPropertySendable(e.PropertyName))
                {
                    var tableElement = sender as TableElement;
                    var index = TableElements.IndexOf(tableElement);
                    if (index < 0) return;
                    var value = typeof(TableElement).GetProperty(e.PropertyName)?.GetValue(tableElement, null);
                    SendData("TableElementChanged", new TableElementPropertyChangedData { Index = index, PropertyName = e.PropertyName, Value = value });
                }
        }

        private void NewTableElementMethod(object parameter)
        {
            var tableElement = new TableElement() { X = rightMouseClickPosition.X / sizeValue, Y = rightMouseClickPosition.Y / sizeValue };
            AddTableElement(tableElement);
            var newtableElementData = TableElementData.ConvertFromTableElement(tableElement);
            ClientServer?.SendData(new DataHolder { Tag = TagAddCircle, Data = newtableElementData });
        }

        [NotifyPropertyChangedInvocator]
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void SendData(string tag, object data) => ClientServer?.SendData(new DataHolder { Tag = tag, Data = data });

        private void SetGridSizeCommandMethod(object obj)
        {
            MessageBox.Show("Click on the upper left corner of a tile and then on the bottom right corner of it");
            setGridSizeActiv = true;
            upperLeftCorner = null;
        }

        private void SetGridSizeReceiveMouseClickMethod(object obj)
        {
            if (obj == null || !setGridSizeActiv) return;

            var mousePosition = Mouse.GetPosition(obj as IInputElement);
            if (upperLeftCorner == null)
            {
                upperLeftCorner = mousePosition;
            }
            else
            {
                setGridSizeActiv = false;
                var bottomRightCorner = mousePosition;
                var sizeDifference = Point.Subtract(bottomRightCorner, (Vector)upperLeftCorner);
                var size = Math.Abs(GetSmallerCoordinate(sizeDifference));
                var unzoomedSize = size / sizeValue;
                foreach (var tableElement in TableElements)
                {
                    tableElement.BaseSize = unzoomedSize;
                }
            }
        }

        private static double GetSmallerCoordinate(Point sizeDifference)
        {
            return sizeDifference.X < sizeDifference.Y ? sizeDifference.X : sizeDifference.Y;
        }
    }
}