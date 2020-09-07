using System.Text;
using System.Xml.Serialization;
using System.IO;
using System;
using System.Xml;

namespace Orders.Business.Helper
{
    /// <summary>
    /// SerializationHelper
    /// </summary>
    public class SerializationHelper
    {
        /// <summary>
        /// xml Serialization
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string xmlSerialize<T>(T obj)
        {
            try
            {
                var serializer = new XmlSerializer(typeof(T));
                var stringBuilder = new StringBuilder();
                var swWriter = new StringWriter(stringBuilder);
                serializer.Serialize(swWriter, obj);
                return stringBuilder.ToString();

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// xml Deserialization
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="xmlString"></param>
        /// <returns></returns>
        public static T xmlDeserialize<T>(string xmlString)
        {
            try
            {
                byte[] bufXML = ASCIIEncoding.UTF8.GetBytes(xmlString);
                MemoryStream ms1 = new MemoryStream(bufXML);

                // Deserialize to object
                XmlSerializer serializer = new XmlSerializer(typeof(T));

                using (XmlReader reader = new XmlTextReader(ms1))
                {
                    T deserializedXML = (T)serializer.Deserialize(reader);
                    return deserializedXML;
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
