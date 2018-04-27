using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Web;
using System.Xml.Serialization;

namespace ChargebackForDotNet
{
    public class ChargebackUtils
    {
        public static string BytesToString(List<byte> bytes)
        {
            return Encoding.UTF8.GetString(bytes.ToArray());
        }

        public static List<byte> StringToBytes(string s)
        {
            return Encoding.ASCII.GetBytes(s).ToList();
        }

        public static string BytesToFile(List<byte> bytes, string filePath)
        {
            if (File.Exists(filePath))
            {
                var fileName = Path.GetFileNameWithoutExtension(filePath);
                var fileExt = Path.GetExtension(filePath);
                var fileParentDir = Path.GetDirectoryName(filePath);
                fileName += String.Format("[{0}]", DateTime.Now.ToString("yyyy-MM-dd[HH-mm-ss]"));
                var newFilePath = Path.Combine(fileParentDir,fileName + fileExt);
                filePath = newFilePath;
            }
            Console.WriteLine("File will be stored at " + filePath);
            var fs = File.Create(filePath);
            var bytesArray = bytes.ToArray();
            fs.Write(bytesArray, 0, bytesArray.Length);
            fs.Close();
            return filePath;
        }

        public static string Encode64(string s, string encode)
        {
            return Convert.ToBase64String(System.Text.Encoding.GetEncoding(encode)
                .GetBytes(s));
        }

        public static DateTime ParseDate(string date)
        {
            var splits = date.Split('-');
            if (splits.Length != 3) return new DateTime(0,0,0);
            var year = int.Parse(splits[0]);
            var month = int.Parse(splits[1]);
            var day = int.Parse(splits[2]);
            return new DateTime(year, month, day);
        }

        public static T DeserializeResponse<T>(string xmlResponse)
        {
            return (T) (new XmlSerializer(typeof(T))).Deserialize(new StringReader(xmlResponse));
        }


        public static string GetResponseXml(HttpWebResponse we)
        {
            var reader = new StreamReader(we.GetResponseStream());
            var xmlResponse = reader.ReadToEnd().Trim();
            reader.Close();
            return xmlResponse;
        }

        public static string GetMimeMapping(string fileName)
        {
            var assembly = Assembly.GetAssembly(typeof(HttpApplication));
            var mimeMappingType = assembly.GetType("System.Web.MimeMapping");
            var getMimeMappingMethod = mimeMappingType.GetMethod("GetMimeMapping", 
                BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public |
                BindingFlags.NonPublic | BindingFlags.FlattenHierarchy);
            return (string)getMimeMappingMethod.Invoke(null /*static method*/, new[] { fileName });
        }
    }
}