using NUnit.Framework;
using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace TCP_Framework.Test
{
    internal class ServerMock : IServer
    {
        public EventHandler<DataReceivedEventArgs> DataReceivedEvent { get; set; }

        public void SendData(DataHolder dataholder)
        {
            SendDataTimesCalled++;
            Dataholder = dataholder;
            WaitForTaskToFinish.Set();
        }

        public bool ReceiveDataThrowsIoException { private get; set; }
        public int SendDataTimesCalled { get; private set; }

        public Task<DataHolder> ReceiveData(CancellationToken token)
        {
            WaitForTaskToFinish.Set();
            return new TaskFactory().StartNew(() =>
            {
                if (ReceiveDataThrowsIoException)
                    throw new IOException();
                ServerReceivesData.WaitOne();
                if (token.IsCancellationRequested)
                {
                }
                WaitForTaskToFinish.Set();
                return new DataHolder();
            }, token);
        }

        public AutoResetEvent ServerReceivesData { get; } = new AutoResetEvent(false);
        public AutoResetEvent ServerConnects { get; } = new AutoResetEvent(false);
        public AutoResetEvent WaitForTaskToFinish { get; } = new AutoResetEvent(false);
        public DataHolder Dataholder { get; private set; }
        public TcpListener Listener { get; } = new TcpListener(new IPEndPoint(IPAddress.Any, 8888));

        public Task EstablishConnection(IServer server, CancellationToken cancellationToken)
        {
            return EstablishConnection(cancellationToken);
        }

        public Task EstablishConnection(int portNumber, CancellationToken cancellationToken)
        {
            return EstablishConnection(cancellationToken);
        }

        private Task EstablishConnection(CancellationToken cancellationToken)
        {
            return new TaskFactory().StartNew(() =>
            {
                ServerConnects.WaitOne();
            }, cancellationToken);
        }
    }

    [TestFixture]
    internal class MultiServerTest
    {
        private MultiServer multiServer;
        private ServerMock serverMock;

        private void Setup()
        {
            serverMock = new ServerMock();
            ServerFactory.Server = serverMock;
            multiServer = new MultiServer(8888);
        }

        [Test]
        public void SendData()
        {
            //Assign
            Setup();
            multiServer.Servers.Add(serverMock);
            var data = new DataHolder { Data = new object(), Tag = "test" };
            //Act
            multiServer.SendData(data);
            //Assert
            Assert.AreEqual(1, serverMock.SendDataTimesCalled);
            Assert.AreEqual(data, serverMock.Dataholder);
        }

        [Test]
        public void ReceiveDataAsync_DataReveivedEventIsCalledAndOnlyAnotherServerSendsData()
        {
            //Assign
            Setup();
            multiServer.AddServer(serverMock);
            var secondServer = new ServerMock();
            multiServer.AddServer(secondServer);
            var called = false;
            var receivedEventCompleted = new AutoResetEvent(false);
            multiServer.DataReceivedEvent += (sender, receivedData) =>
            {
                called = true;
                receivedEventCompleted.Set();
            };
            //Act
            serverMock.ServerReceivesData.Set();
            receivedEventCompleted.WaitOne();
            //Assert
            Assert.AreEqual(1, secondServer.SendDataTimesCalled);
            Assert.AreEqual(0, serverMock.SendDataTimesCalled);
            Assert.IsTrue(called);
        }

        [Test]
        public void ReceiveDataAsync_ServerThrowsException()
        {
            //Assign
            Setup();
            serverMock.ReceiveDataThrowsIoException = true;
            multiServer.AddServer(serverMock);

            var called = false;
            var receivedEventCompleted = new AutoResetEvent(false);
            DataHolder data = null;
            multiServer.DataReceivedEvent += (sender, receivedData) =>
            {
                called = true;
                data = receivedData.Dataholder;
                receivedEventCompleted.Set();
            };
            //Act
            serverMock.ServerReceivesData.Set();
            receivedEventCompleted.WaitOne();
            //Assert
            Assert.AreEqual("Exception", data.Tag);
            Assert.AreEqual(typeof(IOException), data.Data.GetType());
            Assert.AreEqual(0, serverMock.SendDataTimesCalled);
            Assert.IsTrue(called);
        }

        [Test]
        public void OpenNewServerAsync()
        {
            //Assign
            Setup();
            //Act
            serverMock.ServerConnects.Set();
            serverMock.WaitForTaskToFinish.WaitOne();
            //Assert
            Assert.AreEqual(1, multiServer.Servers.Count);
        }
    }
}