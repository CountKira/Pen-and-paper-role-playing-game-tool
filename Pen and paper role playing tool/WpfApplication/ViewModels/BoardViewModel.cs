using System;
using System.Windows;
using MVVM_Framework;
using System.ComponentModel;
using TCP_Framework;
using System.Windows.Input;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Threading;

namespace WpfApplication.ViewModel
{
    internal class BoardViewModel : INotifyPropertyChanged
    {
        private IClientServer clientServer;
        public ICommand NewCircleCommand { get; set; }
        public ObservableCollection<RectItem> RectItems { get; set; } = new ObservableCollection<RectItem>();

        public event PropertyChangedEventHandler PropertyChanged;

        public BoardViewModel(IClientServer clientServer)
        {
            this.clientServer = clientServer;
            NewCircleCommand = new ActionCommand(CommandMethod);
            RectItems.CollectionChanged += RectItems_CollectionChanged;
        }

        private void RectItems_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            var nc = outstandingChanges.Find(nccea => e.Action == nccea.Action);
            if (nc != null)
            {
                outstandingChanges.Remove(nc);
            }
            else
            {
                var data = new CollectionChangedData() { Action = NotifyCollectionChangedAction.Add, NewItem = e.NewItems[0] as RectItem };
                clientServer?.SendData(new DateHolder { Tag = $"{nameof(RectItems)}_changed", Data = data });
            }
        }

        internal void SetNewPosition(object dataContext, Point endposition)
        {
            var rectItem = dataContext as RectItem;
            rectItem.X = endposition.X;
            rectItem.Y = endposition.Y;
            var index = RectItems.IndexOf(rectItem);
            clientServer?.SendData(new DateHolder { Tag = "RectItem_changed", Data = new RectItemChange { Index = index, RectItem = endposition } });
        }

        private void CommandMethod(object parameter)
        {
            RectItems.Add(new RectItem());
        }

        private void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        private List<CollectionChangedData> outstandingChanges = new List<CollectionChangedData>();

        internal void DataReception(object sender, DataReceivedEventArgs data)
        {
            var dataholder = data.Dataholder;
            if (dataholder.Tag == $"{nameof(RectItems)}_changed")
            {
                var changes = dataholder.Data as CollectionChangedData;
                outstandingChanges.Add(changes);
                switch (changes.Action)
                {
                    case NotifyCollectionChangedAction.Add:
                        var newItem = changes.NewItem as RectItem;
                        Application.Current.Dispatcher.Invoke(delegate { RectItems.Add(newItem); });
                        break;

                    case NotifyCollectionChangedAction.Remove:
                        break;

                    case NotifyCollectionChangedAction.Replace:
                        break;

                    case NotifyCollectionChangedAction.Move:
                        break;

                    case NotifyCollectionChangedAction.Reset:
                        break;

                    default:
                        break;
                }
            }
            else if (dataholder.Tag == "RectItem_changed")
            {
                var rectItemChange = dataholder.Data as RectItemChange;
                var oldRectItem = RectItems[rectItemChange.Index];
                var newRectItem = rectItemChange.RectItem;
                oldRectItem.X = newRectItem.X;
                oldRectItem.Y = newRectItem.Y;
            }
        }
    }

    [Serializable]
    internal class CollectionChangedData
    {
        public NotifyCollectionChangedAction Action;
        public RectItem NewItem;
    }

    [Serializable]
    internal class RectItemChange
    {
        public int Index { get; set; }
        public Point RectItem { get; set; }
    }
}