using Moq;
using NUnit.Framework;
using System.Windows;
using TCP_Framework;
using WpfApplication.ViewModel;

namespace WpfApplication.Test.ViewModel
{
    [TestFixture]
    internal class TableViewModelTest
    {
        private const string AddCircleTag = "Add Circle";
        private const string TagTableElementLocationChanged = "TableElementLocation changed";
        private Mock<IClientServer> mock;
        private TableViewModel tableViewModel;
        private DataHolder dataHolder;

        private void SetupWithMock()
        {
            mock = new Mock<IClientServer>();
            mock.Setup(cs => cs.SendData(It.IsAny<DataHolder>())).Callback<DataHolder>(dh => dataHolder = dh);
            tableViewModel = new TableViewModel(null);
        }

        [TearDown]
        public void TearDown()
        {
            dataHolder = null;
        }

        [Test]
        public void NewTableElementCommand()
        {
            //Arrange
            SetupWithMock();
            //Act
            tableViewModel.NewTableElementCommand.Execute(null);
            //Assert
            var tableElementsCount = tableViewModel.TableElements.Count;
            Assert.AreEqual(1, tableElementsCount);
        }

        [Test]
        public void SetTableElementPosition_ChangesElement()
        {
            //Arrange
            SetupWithMock();
            tableViewModel.NewTableElementCommand.Execute(null);
            var tableElement = tableViewModel.TableElements[0];
            //Act
            tableViewModel.SetTableElementPosition(new Point { X = 4, Y = 3 }, tableElement);
            //Assert
            var actualTableElement = tableViewModel.TableElements[0];
            Assert.AreEqual(4, actualTableElement.X);
            Assert.AreEqual(3, actualTableElement.Y);
        }

        //TODO: Since the ClientServerDataRecption has be moved to MainWindowViewModel this has to be tested there
        //mock.Verify(cs => cs.SendData(It.IsAny<DataHolder>()), Times.Once);
        //Assert.AreEqual(AddCircleTag, dataHolder.Tag);
        //Assert.AreEqual(new TableElement(), dataHolder.Data);
        //TODO: Move to MainWindowViewModelTest
        //[Test]
        //public void SetTableElementPosition_SendsChangeToConnectedDevices()
        //{
        //    //Arrange
        //    SetupWithMock();
        //    tableViewModel.NewTableElementCommand.Execute(null);
        //    var tableElement = tableViewModel.TableElements[0];
        //    //Act
        //    tableViewModel.SetTableElementPosition(new Point { X = 4, Y = 3 }, tableElement);
        //    //Assert
        //    Assert.AreEqual(TagTableElementLocationChanged, dataHolder.Tag);
        //    var data = dataHolder.Data as LocationChangedData ?? throw new NullReferenceException();
        //    Assert.AreEqual(4, data.X);
        //    Assert.AreEqual(3, data.Y);
        //    Assert.AreEqual(0, data.Index);
        //}
        //[Test]
        //public void DataReception_ReceivesNewFigureData_AddElementToCollection()
        //{
        //    //Arrange
        //    tableViewModel = new TableViewModel();
        //    var tableElement = new TableElement();
        //    var sentData = new DataHolder { Tag = AddCircleTag, Data = tableElement };
        //    //Act
        //    tableViewModel.DataReception(null, new DataReceivedEventArgs(sentData));
        //    //Assert
        //    Assert.AreEqual(tableElement, tableViewModel.TableElements[0]);
        //}
        //[Test]
        //public void DataReception_ReceiveTableElementLocationChanged()
        //{
        //    //Arrange
        //    tableViewModel = new TableViewModel();
        //    var locationChangedData = new LocationChangedData { X = 3, Y = 4, Index = 1 };
        //    var sentData = new DataHolder { Tag = TagTableElementLocationChanged, Data = locationChangedData };
        //    tableViewModel.TableElements.Add(new TableElement());
        //    tableViewModel.TableElements.Add(new TableElement());
        //    //Act
        //    tableViewModel.DataReception(null, new DataReceivedEventArgs(sentData));
        //    //Assert
        //    var tableElement = tableViewModel.TableElements[1];
        //    Assert.AreEqual(locationChangedData.X, tableElement.X);
        //    Assert.AreEqual(locationChangedData.Y, tableElement.Y);
        //}
    }
}