using MVVM_Framework;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using TCP_Framework;
using System;

namespace WpfApplication.ViewModel
{
    public class TableViewModel : INotifyPropertyChanged
    {
        private const string tag_tableElementLocationChanged = "TableElementLocation changed";
        private const string tag_addCircle = "Add Circle";
        private IClientServer clientServer;
        public ICommand NewCircleCommand { get; set; }

        public TableViewModel(IClientServer clientServer)
        {
            this.clientServer = clientServer;
            if (clientServer != null)
                clientServer.DataReceivedEvent += DataReception;
            NewCircleCommand = new ActionCommand(NewCircleMethod);
        }

        public ObservableCollection<TableElement> TableElements { get; set; } = new ObservableCollection<TableElement>();

        private void NewCircleMethod(object parameter)
        {
            TableElement tableElement = new TableElement();
            TableElements.Add(tableElement);
            clientServer?.SendData(new DataHolder { Tag = tag_addCircle, Data = tableElement });
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        public void DataReception(object sender, DataReceivedEventArgs data)
        {
            var dataholder = data.Dataholder;
            if (dataholder.Tag == tag_addCircle)
            {
                var tableElement = dataholder.Data as TableElement;
                DispatcherHelper.Invoke(() => TableElements.Add(tableElement));
            }
            if (dataholder.Tag == tag_tableElementLocationChanged)
            {
                var locationChangedData = dataholder.Data as LocationChangedData;
                var tableElement = TableElements[locationChangedData.Index];
                tableElement.X = locationChangedData.X;
                tableElement.Y = locationChangedData.Y;
            }
        }

        public void SetTableElementPosition(Point position, TableElement changedElement)
        {
            changedElement.X = position.X;
            changedElement.Y = position.Y;
            var index = TableElements.IndexOf(changedElement);
            var locationChangedData = new LocationChangedData { X = changedElement.X, Y = changedElement.Y, Index = index };
            clientServer?.SendData(new DataHolder { Tag = tag_tableElementLocationChanged, Data = locationChangedData });
        }
    }
}