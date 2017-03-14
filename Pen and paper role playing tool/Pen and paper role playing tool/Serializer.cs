using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Pen_and_paper_role_playing_tool
{
    internal static class Serializer
    {
        public static byte[] Serialize(object anySerializableObject)
        {
            using (var memoryStream = new MemoryStream())
            {
                new BinaryFormatter().Serialize(memoryStream, anySerializableObject);
                return memoryStream.ToArray();
            }
        }

        public static T Deserialize<T>(byte[] message)
        {
            using (var memoryStream = new MemoryStream(message))
                return (T)new BinaryFormatter().Deserialize(memoryStream);
        }
    }
}