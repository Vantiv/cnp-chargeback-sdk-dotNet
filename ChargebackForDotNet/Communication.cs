using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ChargebackForDotNet.Properties;
using System.Net;
using System.Security.Authentication;
using System.Text;
using System.Threading;

namespace ChargebackForDotNet
{

    public class Communication
    {
        private static HttpWebRequest createHttpRequest(Configuration config, string urlRoute)
        {
            // make http post to this config.
            string url = config.getConfig("host") + urlRoute;
            Console.WriteLine("Making a request to " + url);
            // For TLS1.2
            ServicePointManager.SecurityProtocol = (SecurityProtocolType) 3072;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            string username = config.getConfig("username");
            string password = config.getConfig("password");
            string merchantId = config.getConfig("merchantId");
            string encoded = Convert.ToBase64String(System.Text.Encoding.GetEncoding("utf-8")
                .GetBytes(username + ":" + password));
            Console.WriteLine("Encoded is: " + encoded);
            request.Headers.Add("Authorization", "Basic " + encoded);
            request.ContentType = "application/com.vantivcnp.services-v2+xml";
            request.Accept = "application/com.vantivcnp.services-v2+xml";
            request.Proxy = new WebProxy(config.getConfig("proxyHost"), Int32.Parse(config.getConfig("proxyPort")));
            return request;
        }

        public static string readBytes(HttpWebResponse httpResponse, List<byte> receivingbytes)
        {
            string contentType = httpResponse.ContentType;
            
            Stream responseStream = httpResponse.GetResponseStream();
            int b = responseStream.ReadByte();
            while (b != -1)
            {
                receivingbytes.Add((byte)b);
                b = responseStream.ReadByte();
            }
            responseStream.Close();
            httpResponse.Close();
            return contentType;
        }
        
        public static string get(Configuration config, string urlRoute, List<byte> receivingbytes)
        {
            HttpWebRequest request = createHttpRequest(config, urlRoute);
            request.Method = "GET";
            
            HttpWebResponse response = (HttpWebResponse) request.GetResponse();
            return readBytes(response, receivingbytes);
        }

        public static string put(Configuration config, string urlRoute, List<byte> sendingBytes, List<byte> receivingbytes)
        {
            HttpWebRequest request = createHttpRequest(config, urlRoute);
            request.Method = "PUT";
            Stream inStream = request.GetRequestStream();
            foreach (var b in sendingBytes)
            {
                inStream.WriteByte(b);
            }
            inStream.Close();
            
            HttpWebResponse response = (HttpWebResponse) request.GetResponse();
            return readBytes(response, receivingbytes);
        }
        
        public static string post(Configuration config, string urlRoute, List<byte> sendingBytes, List<byte> receivingbytes)
        {
            HttpWebRequest request = createHttpRequest(config, urlRoute);
            request.Method = "POST";
            Stream inStream = request.GetRequestStream();
            foreach (var b in sendingBytes)
            {
                inStream.WriteByte(b);
            }
            inStream.Close();
            HttpWebResponse response = (HttpWebResponse) request.GetResponse();
            return readBytes(response, receivingbytes);
        }
        
        public static string delete(Configuration config, string urlRoute, List<byte> receivingbytes)
        {
            HttpWebRequest request = createHttpRequest(config, urlRoute);
            request.Method = "DELETE";
            
            HttpWebResponse response = (HttpWebResponse) request.GetResponse();
            return readBytes(response, receivingbytes);
        }
    }
}