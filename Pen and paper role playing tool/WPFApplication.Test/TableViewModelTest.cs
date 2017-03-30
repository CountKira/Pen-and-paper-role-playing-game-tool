﻿using Moq;
using NUnit.Framework;
using System;
using System.Windows;
using TCP_Framework;

namespace WpfApplication.ViewModel.Test
{
    [TestFixture]
    internal class TableViewModelTest
    {
        private const string addCircleTag = "Add Circle";
        private const string tag_TableElementLocationChanged = "TableElementLocation changed";
        private Mock<IClientServer> mock;
        private TableViewModel tableViewModel;
        private DataHolder dataHolder;

        public void SetupWithMock()
        {
            mock = new Mock<IClientServer>();
            mock.Setup(cs => cs.SendData(It.IsAny<DataHolder>())).Callback<DataHolder>(dh => dataHolder = dh);
            tableViewModel = new TableViewModel(mock.Object);
        }

        [TearDown]
        public void TearDown()
        {
            dataHolder = null;
        }

        [Test]
        public void NewCircleCommand()
        {
            //Arrange
            SetupWithMock();
            //Act
            tableViewModel.NewCircleCommand.Execute(null);
            //Assert
            int tableElementsCount = tableViewModel.TableElements.Count;
            Assert.AreEqual(1, tableElementsCount);
            mock.Verify(cs => cs.SendData(It.IsAny<DataHolder>()), Times.Once);
            Assert.AreEqual(addCircleTag, dataHolder.Tag);
            Assert.AreEqual(new TableElement(), dataHolder.Data);
        }

        [Test]
        public void DataReception_ReceivesNewFigureData_AddElementToCollection()
        {
            //Arrange
            tableViewModel = new TableViewModel(null);
            var tableElement = new TableElement();
            var sentData = new DataHolder { Tag = addCircleTag, Data = tableElement };
            //Act
            tableViewModel.DataReception(null, new DataReceivedEventArgs(sentData));
            //Assert
            Assert.AreEqual(tableElement, tableViewModel.TableElements[0]);
        }

        [Test]
        public void SetTableElementPosition_ChangesElement()
        {
            //Arrange
            SetupWithMock();
            tableViewModel.NewCircleCommand.Execute(null);
            var tableElement = tableViewModel.TableElements[0];
            //Act
            tableViewModel.SetTableElementPosition(new Point { X = 4, Y = 3 }, tableElement);
            //Assert
            var actualTableElement = tableViewModel.TableElements[0];
            Assert.AreEqual(4, actualTableElement.X);
            Assert.AreEqual(3, actualTableElement.Y);
        }

        [Test]
        public void SetTableElementPosition_SendsChangeToConnectedDevices()
        {
            //Arrange
            SetupWithMock();
            tableViewModel.NewCircleCommand.Execute(null);
            var tableElement = tableViewModel.TableElements[0];
            //Act
            tableViewModel.SetTableElementPosition(new Point { X = 4, Y = 3 }, tableElement);
            //Assert
            Assert.AreEqual(tag_TableElementLocationChanged, dataHolder.Tag);
            var data = dataHolder.Data as LocationChangedData;
            Assert.AreEqual(4, data.X);
            Assert.AreEqual(3, data.Y);
            Assert.AreEqual(0, data.Index);
        }

        [Test]
        public void DataReception_ReceiveTableElementLocationChanged()
        {
            //Arrange
            tableViewModel = new TableViewModel(null);
            var locationChangedData = new LocationChangedData { X = 3, Y = 4, Index = 1 };
            var sentData = new DataHolder { Tag = tag_TableElementLocationChanged, Data = locationChangedData };
            tableViewModel.TableElements.Add(new TableElement());
            tableViewModel.TableElements.Add(new TableElement());
            //Act
            tableViewModel.DataReception(null, new DataReceivedEventArgs(sentData));
            //Assert
            TableElement tableElement = tableViewModel.TableElements[1];
            Assert.AreEqual(locationChangedData.X, tableElement.X);
            Assert.AreEqual(locationChangedData.Y, tableElement.Y);
        }
    }
}