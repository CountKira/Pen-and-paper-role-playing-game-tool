using Moq;
using NUnit.Framework;
using TCP_Framework;
using WpfApplication.ViewModel;

namespace WpfApplication.Test.ViewModel
{
    [TestFixture]
    internal class MainWindowViewModelTest
    {
        [Test]
        public void SendMessageMethod_SendMessage()
        {
            var mock = new Mock<IClientServer>();
            var dataHolder = new DataHolder();
            mock.Setup(cs => cs.SendData(It.IsAny<DataHolder>())).Callback<DataHolder>(dh => dataHolder = dh);
            var mainWindowViewModel = new MainWindowViewModel(null)
            {
                MessageInput = "Hallo Welt",
                ClientServer = mock.Object
            };

            mainWindowViewModel.SendMessageCommand.Execute(null);

            var actualInput = mainWindowViewModel.MessageInput;
            var actualOutput = mainWindowViewModel.MessageOutput;
            Assert.AreEqual("", actualInput);
            Assert.AreEqual(": Hallo Welt\r\n", actualOutput);
            mock.Verify(foo => foo.SendData(It.IsAny<DataHolder>()), Times.Once());
            Assert.AreEqual("Text", dataHolder.Tag);
            Assert.AreEqual(": Hallo Welt", dataHolder.Data);
        }
    }
}