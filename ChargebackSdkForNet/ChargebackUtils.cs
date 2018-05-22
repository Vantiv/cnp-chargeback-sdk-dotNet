using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Xml.Serialization;

namespace ChargebackSdkForNet
{
    public class ChargebackUtils
    {
        private static String defaultType = "application/octet-stream";
        private static Dictionary<string, string> ExtensionMap = new Dictionary<string, string>();

        static ChargebackUtils()
        {
            ExtensionMap.Add("tiff", "image/tiff");
            ExtensionMap.Add("png", "image/png");
            ExtensionMap.Add("jpg", "image/jpeg");
            ExtensionMap.Add("jpeg", "image/jpeg");
            ExtensionMap.Add("jpe", "image/jpeg");
            ExtensionMap.Add("gif", "image/gif");
            ExtensionMap.Add("pdf", "application/pdf");
        }

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
                fileName += string.Format("[{0}]", DateTime.Now.ToString("yyyy-MM-dd[HH-mm-ss]"));
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

        public static void PrintXml(string xml, string printXml, string neuterXml)
        {
            if (bool.Parse(printXml))
            {
                if (bool.Parse(neuterXml))
                {
                    xml = NeuterXml(xml);
                }
                Console.WriteLine("\nXml : \n" + xml + "\n\n");
            }
        }

        private static string NeuterXml(string xml)
        {
            const string pattern1 = "(?i)<cardNumberLast4>.*?</cardNumberLast4>";
            const string pattern2 = "(?i)<token>.*?</token>";
            
            var rgx1 = new Regex(pattern1);
            var rgx2 = new Regex(pattern2);
            xml = rgx1.Replace(xml, "<cardNumberLast4>xxxx</cardNumberLast4>");
            xml = rgx2.Replace(xml, "<token>xxxxxxxxxx</token>");
            return xml;
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
            int dot_pos = fileName.LastIndexOf(".");
            if (dot_pos < 0)
            {
                return defaultType;
            }
            else
            {
                String file_ext = fileName.Substring(dot_pos + 1);
                if (file_ext.Count() == 0)
                {
                    return defaultType;
                }
                else
                {
                    if (ExtensionMap.ContainsKey(file_ext))
                        return ExtensionMap[file_ext];
                    else
                        return defaultType;
                }
            }
        }


    }
}