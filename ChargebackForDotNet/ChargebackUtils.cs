using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Xml.Serialization;

namespace ChargebackForDotNet
{
    public class ChargebackUtils
    {
        public static string BytesToString(List<byte> bytes)
        {
            return System.Text.Encoding.UTF8.GetString(bytes.ToArray());
        }

        public static List<byte> StringToBytes(string s)
        {
            return Encoding.ASCII.GetBytes(s).ToList();
        }

        public static string BytesToFile(List<byte> bytes, string filePath)
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

        public static string Encode64(string s, string encode)
        {
            return Convert.ToBase64String(System.Text.Encoding.GetEncoding(encode)
                .GetBytes(s));
        }

        public static DateTime ParseDate(string date)
        {
            string[] splits = date.Split('-');
            if (splits.Length != 3) return new DateTime(0,0,0);
            int year = Int16.Parse(splits[0]);
            int month = Int16.Parse(splits[1]);
            int day = Int16.Parse(splits[2]);
            return new DateTime(year, month, day);
        }

        public static T DeserializeResponse<T>(string xmlResponse)
        {
            return (T) (new XmlSerializer(typeof(T))).Deserialize(new StringReader(xmlResponse));
        }

        public static string ExtractErrorMessages(string xmlResponse)
        {
            var errResponse = DeserializeResponse<errorResponse>(xmlResponse);
            string errString = "";
            foreach (var err in errResponse.errors)
            {
                errString += "\n" + err;
            }

            return errString;
        }

        public static string GetResponseXml(HttpWebResponse we)
        {
            StreamReader reader = new StreamReader(we.GetResponseStream());
            string xmlResponse = reader.ReadToEnd().Trim();
            reader.Close();
            return xmlResponse;
        }
    }
}