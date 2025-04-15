using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Xml.Serialization;
using YamlDotNet;

namespace Server
{
    public class Serializer 
    {
        public static string SerializeToJson<T>(T obj) => JsonSerializer.Serialize(obj);

        public static T DeserializeFromJson<T>(string json) => JsonSerializer.Deserialize<T>(json);

        public static string SerializeToYaml<T>(T obj)
        {
            var serializer = new YamlDotNet.Serialization.Serializer();
            return serializer.Serialize(obj);
        }

        public static T DeserializeFromYaml<T>(string yaml)
        {
            var deserializer = new YamlDotNet.Serialization.Deserializer();
            return deserializer.Deserialize<T>(yaml);
        }

        public static string SerializeToXml<T>(T obj)
        {
            var serializer = new XmlSerializer(typeof(T));
            using var stringWriter = new StringWriter();
            serializer.Serialize(stringWriter, obj);
            return stringWriter.ToString();
        }

        public static T DeserializeFromXml<T>(string xml)
        {
            var serializer = new XmlSerializer(typeof(T));
            using var stringReader = new StringReader(xml);
            return (T)serializer.Deserialize(stringReader);
        }
        private static T DeserializeData<T>(string data)
        {
            if (data.TrimStart().StartsWith("{") || data.TrimStart().StartsWith("["))
            {
                return Serializer.DeserializeFromJson<T>(data);
            }
            else if (data.TrimStart().StartsWith("<?xml") || data.TrimStart().StartsWith("<"))
            {
                return Serializer.DeserializeFromXml<T>(data);
            }
            else
            {
                return Serializer.DeserializeFromYaml<T>(data);
            }
        }

    }
}

