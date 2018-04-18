using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace ChargebackForDotNet
{
    public class Utils
    {
        public static string bytesToString(List<byte> bytes)
        {
            return System.Text.Encoding.UTF8.GetString(bytes.ToArray());
        }

        public static List<byte> stringToBytes(string s)
        {
            return Encoding.ASCII.GetBytes(s).ToList();
        }

        public static string bytesToFile(List<byte> bytes, string filePath)
        {
            if (File.Exists(filePath))
            {
                string fileName = Path.GetFileNameWithoutExtension(filePath);
                string fileExt = Path.GetExtension(filePath);
                string fileParentDir = Path.GetDirectoryName(filePath);
                fileName += String.Format("[{0}]", DateTime.Now.ToString("yyyy-MM-dd[HH-mm-ss]"));
                string newFilePath = Path.Combine(fileParentDir,fileName + fileExt);
                filePath = newFilePath;
            }
            Console.WriteLine("File will be stored at " + filePath);
            FileStream fs = File.Create(filePath);
            byte[] bytesArray = bytes.ToArray();
            fs.Write(bytesArray, 0, bytesArray.Length);
            fs.Close();
            return filePath;
        }

        public static T DeserializeResponse<T>(string xmlResponse)
        {
            return (T) (new XmlSerializer(typeof(T))).Deserialize(new StringReader(xmlResponse));
        }
    }
}