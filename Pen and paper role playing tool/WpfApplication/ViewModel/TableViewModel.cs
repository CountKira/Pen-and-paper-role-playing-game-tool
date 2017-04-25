using MVVM_Framework;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using TCP_Framework;

namespace WpfApplication.ViewModel
{
    public class TableViewModel
    {
        private const string TagTableElementLocationChanged = "TableElementLocation changed";
        private const string TagAddCircle = "Add Circle";
        private readonly IClientServer clientServer;
        public ICommand NewCircleCommand { get; }

        public TableViewModel(IClientServer clientServer)
        {
            this.clientServer = clientServer;
            if (clientServer != null)
                clientServer.DataReceivedEvent += DataReception;
            NewCircleCommand = new ActionCommand(NewCircleMethod);
        }

        public ObservableCollection<TableElement> TableElements { get; } = new ObservableCollection<TableElement>();

        private void NewCircleMethod(object parameter)
        {
            var tableElement = new TableElement();
            TableElements.Add(tableElement);
            clientServer?.SendData(new DataHolder { Tag = TagAddCircle, Data = tableElement });
        }

        public void DataReception(object sender, DataReceivedEventArgs data)
        {
            var dataholder = data.Dataholder;
            var transmitedData = dataholder.Data;
            if (transmitedData == null) return;

            switch (transmitedData)
            {
                case TableElement tableElement:
                    {
                        DispatcherHelper.Invoke(() => TableElements.Add(tableElement));
                    }
                    break;
                case LocationChangedData locationChangedData:
                    {
                        var tableElement = TableElements[locationChangedData.Index];
                        tableElement.X = locationChangedData.X;
                        tableElement.Y = locationChangedData.Y;
                    }
                    break;
            }
        }

        public void SetTableElementPosition(Point position, TableElement changedElement)
        {
            changedElement.X = position.X;
            changedElement.Y = position.Y;
            var index = TableElements.IndexOf(changedElement);
            var locationChangedData = new LocationChangedData { X = changedElement.X, Y = changedElement.Y, Index = index };
            clientServer?.SendData(new DataHolder { Tag = TagTableElementLocationChanged, Data = locationChangedData });
        }
    }
}