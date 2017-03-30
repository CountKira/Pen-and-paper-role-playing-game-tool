using NUnit.Framework;
using System.ComponentModel;
using TCP_Framework;

namespace WpfApplication.Test
{
    [TestFixture]
    public class TableElementTest
    {
        private TableElement tableElement;
        private object sender;
        private PropertyChangedEventArgs e;

        private void Setup()
        {
            tableElement = new TableElement();
            void TableElement_PropertyChanged(object sender, PropertyChangedEventArgs e)
            {
                this.sender = sender;
                this.e = e;
            }
            tableElement.PropertyChanged += TableElement_PropertyChanged;
        }

        [TearDown]
        public void TearDown()
        {
            sender = null;
            e = null;
        }

        [Test]
        public void SettingX_NewValue_PropertyChangedFired()
        {
            Setup();
            tableElement.X = 1;
            Assert.AreEqual("X", e?.PropertyName);
            Assert.AreEqual(tableElement, sender);
        }

        [Test]
        public void SettingY_NewValue_PropertyChangedFired()
        {
            Setup();
            tableElement.Y = 1;
            Assert.AreEqual("Y", e?.PropertyName);
            Assert.AreEqual(tableElement, sender);
        }

        [Test]
        public void SettingX_SameValue_PropertyChangedNotFired()
        {
            Setup();
            tableElement.X = 0;
            Assert.AreEqual(null, e?.PropertyName);
            Assert.AreEqual(null, sender);
        }

        [Test]
        public void SettingY_SameValue_PropertyChangedNotFired()
        {
            Setup();
            tableElement.Y = 0;
            Assert.AreEqual(null, e?.PropertyName);
            Assert.AreEqual(null, sender);
        }

        [Test]
        public void SettingY_AnyValue_SavesValue()
        {
            Setup();
            var n = 4;
            tableElement.Y = n;
            Assert.AreEqual(n, tableElement.Y);
        }

        [Test]
        public void SettingX_AnyValue_SavesValue()
        {
            Setup();
            var n = 4;
            tableElement.X = n;
            Assert.AreEqual(n, tableElement.X);
        }

        [Test]
        public void Equality_EqualElement()
        {
            //Arrange
            var tableElementA = new TableElement { X = 3, Y = 10 };
            var tableElementB = new TableElement { X = 3, Y = 10 };
            //Act
            var EqualityAtoB = tableElementA.Equals(tableElementB);
            var EqualityBtoA = tableElementA.Equals(tableElementB);
            var EqualityAtoA = tableElementA.Equals(tableElementA);
            //Assert
            Assert.AreEqual(true, EqualityAtoB);
            Assert.AreEqual(true, EqualityBtoA);
            Assert.AreEqual(true, EqualityAtoA);
        }

        [Test]
        public void Equality_UnequalElement()
        {
            //Arrange
            var tableElementA = new TableElement { X = 4, Y = 10 };
            var tableElementB = new TableElement { X = 3, Y = 10 };
            //Act
            var EqualityAtoB = tableElementA.Equals(tableElementB);
            var EqualityBtoA = tableElementA.Equals(tableElementB);
            var EqualityAtoA = tableElementA.Equals(tableElementA);
            //Assert
            Assert.AreEqual(false, EqualityAtoB);
            Assert.AreEqual(false, EqualityBtoA);
            Assert.AreEqual(true, EqualityAtoA);
        }

        [Test]
        public void SerializeDeserialize()
        {
            //Arrange
            Setup();
            //Act
            var bytes = Serializer.Serialize(tableElement);
            var deserialized = Serializer.Deserialize<TableElement>(bytes);
            //Assert
            Assert.AreEqual(tableElement, deserialized);
        }
    }
}