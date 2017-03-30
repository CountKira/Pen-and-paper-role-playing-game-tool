using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace TCP_Framework
{
    public static class Serializer
    {
        public static byte[] Serialize(object anySerializableObject)
        {
            using (var memoryStream = new MemoryStream())
            {
                new BinaryFormatter().Serialize(memoryStream, anySerializableObject);
                return memoryStream.ToArray();
            }
        }

        public static T Deserialize<T>(byte[] data)
        {
            using (var memoryStream = new MemoryStream(data))
                return (T)new BinaryFormatter().Deserialize(memoryStream);
        }
    }
}