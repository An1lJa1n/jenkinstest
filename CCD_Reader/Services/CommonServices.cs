using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Xml.Serialization;

namespace Exscribe.Common
{
    public static class CommonServices
    {
        public static string FormatDate(string ProvidedDate, string format)
        {
            if (!String.IsNullOrEmpty(ProvidedDate))
            {
                string year = ProvidedDate.Substring(0, 4);
                string mm = ProvidedDate.Substring(4, 2);
                string dd = ProvidedDate.Substring(6, 2);

                DateTime dt = new DateTime(Convert.ToInt16(year), Convert.ToInt16(mm), Convert.ToInt16(dd));

                ProvidedDate = dt.ToString(format);
            }

            return ProvidedDate;
        }
        public static void Serialize<T>(T value, string pathName)
        {
            using (TextWriter writer = new StreamWriter(pathName))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(T));
                serializer.Serialize(writer, value);
            }
        }
        public static T Deserialize<T>(Stream pathName)
        {
            using (TextReader reader = new StreamReader(pathName))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(T));
                return (T)serializer.Deserialize(reader);
            }
        }
        public static Stream GetStreamFromString(string s)
        {
            MemoryStream stream = new MemoryStream();
            StreamWriter writer = new StreamWriter(stream);
            writer.Write(s);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }
        public static string UppercaseWords(string value)
        {
            List<string> articles = new List<string>() { "and", "for", "to", "of" };
            string[] words = value.Split(' ');
            string final = string.Empty;

            foreach (var word in words)
            {
                if (!articles.Contains(word))
                {
                    final += CultureInfo.CurrentCulture.TextInfo.ToTitleCase(word) + " ";
                }
                else
                {
                    final += word + " ";
                }
            }
            return final.Trim();
        }
    }
}