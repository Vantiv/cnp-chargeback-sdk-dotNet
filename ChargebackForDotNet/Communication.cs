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
        private HttpWebRequest httpRequest;
        private WebHeaderCollection headers;
        private WebProxy webProxy;
        private string contentType;
        private string accept;
        private string host;
        
        public Communication(string host)
        {
            this.host = host;
            headers = new WebHeaderCollection();
        }

        public void addToHeader(string key, string value)
        {
            headers.Add(key, value);
        }

        public void setProxy(string host, int port)
        {
            webProxy = new WebProxy(host, port);
        }

        public void setContentType(string contentType)
        {
            this.contentType = contentType;
        }

        public void setAccept(string accept)
        {
            this.accept = accept;
        }
        
        private void createHttpRequest(string urlRoute)
        {
            string url = host + urlRoute;
            Console.WriteLine("Making a request to " + url);
            // For TLS1.2.
            ServicePointManager.SecurityProtocol = (SecurityProtocolType) 3072;
            httpRequest = (HttpWebRequest)WebRequest.Create(url);
            httpRequest.Headers.Add(headers);
            if (webProxy != null)
            {
                httpRequest.Proxy = webProxy;
            }

            if (contentType != null)
            {
                httpRequest.ContentType = contentType;
            }

            if (accept != null)
            {
                httpRequest.Accept = accept;
            }
        }

        public string readBytes(HttpWebResponse httpResponse, List<byte> receivingbytes)
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
        
        public string get(string urlRoute, List<byte> receivingbytes)
        {
            createHttpRequest(urlRoute);
            httpRequest.Method = "GET";
            HttpWebResponse response = (HttpWebResponse) httpRequest.GetResponse();
            return readBytes(response, receivingbytes);
        }

        public string put(string urlRoute, List<byte> sendingBytes, List<byte> receivingbytes)
        {
            createHttpRequest(urlRoute);
            httpRequest.Method = "PUT";
            Stream inStream = httpRequest.GetRequestStream();
            foreach (var b in sendingBytes)
            {
                inStream.WriteByte(b);
            }
            inStream.Close();
            
            HttpWebResponse response = (HttpWebResponse) httpRequest.GetResponse();
            return readBytes(response, receivingbytes);
        }
        
        public string post(string urlRoute, List<byte> sendingBytes, List<byte> receivingbytes)
        {
            createHttpRequest(urlRoute);
            httpRequest.Method = "POST";
            Stream inStream = httpRequest.GetRequestStream();
            foreach (var b in sendingBytes)
            {
                inStream.WriteByte(b);
            }
            inStream.Close();
            HttpWebResponse response = (HttpWebResponse) httpRequest.GetResponse();
            return readBytes(response, receivingbytes);
        }
        
        public string delete(string urlRoute, List<byte> receivingbytes)
        {
            createHttpRequest(urlRoute);
            httpRequest.Method = "DELETE";
            HttpWebResponse response = (HttpWebResponse) httpRequest.GetResponse();
            return readBytes(response, receivingbytes);
        }
    }
}