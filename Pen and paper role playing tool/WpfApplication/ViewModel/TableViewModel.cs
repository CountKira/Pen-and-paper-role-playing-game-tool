﻿using MVVM_Framework;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using TCP_Framework;
using WpfApplication.Annotations;

namespace WpfApplication.ViewModel
{
    public sealed class TableViewModel : INotifyPropertyChanged
    {
        private const string TagTableElementLocationChanged = "TableElementLocation changed";
        private const string TagAddCircle = "Add Circle";
        private readonly IClientServer clientServer;
        public ICommand NewCircleCommand { get; }
        private string backgroundImageUrl = @"C:\Users\Riedmiller Stefan\Pictures\Doctors.jpg";
        public ICommand ChangeBackgroundCommand { get; }
        public ObservableCollection<TableElement> TableElements { get; } = new ObservableCollection<TableElement>();
        public ObservableCollection<ContextAction> Actions { get; }
        public string BackgroundImageUrl
        {
            get => backgroundImageUrl;
            set
            {
                if (value == backgroundImageUrl) return;
                backgroundImageUrl = value;
                OnPropertyChanged(nameof(BackgroundImageUrl));
            }
        }

        public TableViewModel(IClientServer clientServer)
        {
            this.clientServer = clientServer;
            if (clientServer != null)
                clientServer.DataReceivedEvent += DataReception;
            NewCircleCommand = new ActionCommand(NewCircleMethod);
            ChangeBackgroundCommand = new ActionCommand(ChangeBackgoundMethod);
            Actions = new ObservableCollection<ContextAction>
            {
                new ContextAction{Name = "Add Element",Action = new ActionCommand(NewCircleMethod)}
            };
        }

        private void ChangeBackgoundMethod(object obj)
        {
            var (imageSelected, imageUrl) = PictureChanger.ChangePictureUrl();
            if (imageSelected)
                BackgroundImageUrl = imageUrl;
        }


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

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}