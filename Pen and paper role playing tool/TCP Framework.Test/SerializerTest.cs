using NUnit.Framework;

namespace TCP_Framework.Test
{
    [TestFixture]
    public class SerializerTest
    {
        [TestCase(int.MinValue)]
        [TestCase(-1)]
        [TestCase(0)]
        [TestCase(1)]
        [TestCase(int.MaxValue)]
        public void SerializeDeserialize_Integers(int n)
        {
            var bytes = Serializer.Serialize(n);
            var actual = Serializer.Deserialize<int>(bytes);
            Assert.AreEqual(n, actual);
        }
    }
}