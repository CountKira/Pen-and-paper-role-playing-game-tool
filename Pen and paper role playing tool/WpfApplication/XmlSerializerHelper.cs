using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace WpfApplication
{
    internal static class XmlSerializerHelper
    {
        public static T ReadXml<T>(string filename)
        {
            var xmlDocument = new XmlDocument();
            xmlDocument.Load(filename);
            var xmlString = xmlDocument.OuterXml;

            T tvm;
            using (var read = new StringReader(xmlString))
            {
                var outType = typeof(T);

                var serializer = new XmlSerializer(outType);
                using (var reader = new XmlTextReader(read))
                {
                    tvm = (T)serializer.Deserialize(reader);
                    reader.Close();
                }

                read.Close();
            }
            return tvm;
        }

        public static void SaveXml<T>(T serializedObject, string fileName)
        {
            var serializer = new XmlSerializer(serializedObject.GetType());
            using (var stream = new MemoryStream())
            {
                var xmlDocument = new XmlDocument();
                serializer.Serialize(stream, serializedObject);
                stream.Position = 0;
                xmlDocument.Load(stream);
                xmlDocument.Save(fileName);
                stream.Close();
            }
        }
    }
}