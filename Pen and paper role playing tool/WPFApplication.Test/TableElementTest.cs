using NUnit.Framework;
using System.ComponentModel;

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
            void TableElementPropertyChanged(object senderElement, PropertyChangedEventArgs eventArgs)
            {
                sender = senderElement;
                e = eventArgs;
            }
            tableElement.PropertyChanged += TableElementPropertyChanged;
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
            const int n = 4;
            tableElement.Y = n;
            Assert.AreEqual(n, tableElement.Y);
        }

        [Test]
        public void SettingX_AnyValue_SavesValue()
        {
            Setup();
            const int n = 4;
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
            var equalityAtoB = tableElementA.Equals(tableElementB);
            var equalityBtoA = tableElementA.Equals(tableElementB);
            var equalityAtoA = tableElementA.Equals(tableElementA);
            //Assert
            Assert.AreEqual(true, equalityAtoB);
            Assert.AreEqual(true, equalityBtoA);
            Assert.AreEqual(true, equalityAtoA);
        }

        [Test]
        public void Equality_UnequalElement()
        {
            //Arrange
            var tableElementA = new TableElement { X = 4, Y = 10 };
            var tableElementB = new TableElement { X = 3, Y = 10 };
            //Act
            var equalityAtoB = tableElementA.Equals(tableElementB);
            var equalityBtoA = tableElementA.Equals(tableElementB);
            var equalityAtoA = tableElementA.Equals(tableElementA);
            //Assert
            Assert.AreEqual(false, equalityAtoB);
            Assert.AreEqual(false, equalityBtoA);
            Assert.AreEqual(true, equalityAtoA);
        }
    }
}