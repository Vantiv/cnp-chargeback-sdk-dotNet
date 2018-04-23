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
        private bool contentLengthSet;
        private long contentLength;

        public Communication()
        {
            headers = new WebHeaderCollection();
        }

        public void setHost(string host)
        {
            this.host = host;
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
            httpRequest = (HttpWebRequest) WebRequest.Create(url);
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

            if (contentLengthSet)
            {
                httpRequest.ContentLength = contentLength;
            }
        }

        public ArrayList readBytes(HttpWebResponse httpResponse)
        {
            var receivingbytes = new List<byte>();
            string contentType = httpResponse.ContentType;

            Stream responseStream = httpResponse.GetResponseStream();
            int b = responseStream.ReadByte();
            while (b != -1)
            {
                receivingbytes.Add((byte) b);
                b = responseStream.ReadByte();
            }

            responseStream.Close();
            httpResponse.Close();
            var tuple = new ArrayList();
            tuple.Add(contentType);
            tuple.Add(receivingbytes);
            return tuple;
        }


        public ArrayList get(string urlRoute)
        {
            createHttpRequest(urlRoute);
            httpRequest.Method = "GET";
            HttpWebResponse response = (HttpWebResponse) httpRequest.GetResponse();
            return readBytes(response);
        }

        public ArrayList delete(string urlRoute)
        {
            createHttpRequest(urlRoute);
            httpRequest.Method = "DELETE";
            HttpWebResponse response = (HttpWebResponse) httpRequest.GetResponse();
            return readBytes(response);
        }

        public ArrayList put(string urlRoute, List<byte> sendingBytes)
        {
            createHttpRequest(urlRoute);
            httpRequest.Method = "PUT";
            httpRequest.ContentLength = sendingBytes.Count;
            Stream inStream = httpRequest.GetRequestStream();
            inStream.Write(sendingBytes.ToArray(), 0, sendingBytes.Count);
            inStream.Close();

            HttpWebResponse response = (HttpWebResponse) httpRequest.GetResponse();
            return readBytes(response);
        }

        public ArrayList post(string urlRoute, List<byte> sendingBytes)
        {
            createHttpRequest(urlRoute);
            httpRequest.Method = "POST";
            httpRequest.ContentLength = sendingBytes.Count;
            Stream inStream = httpRequest.GetRequestStream();
            inStream.Write(sendingBytes.ToArray(), 0, sendingBytes.Count);
            inStream.Close();
            Console.WriteLine("Finish writing bytes to Request Stream.");
            HttpWebResponse response = (HttpWebResponse) httpRequest.GetResponse();
            return readBytes(response);
        }
    }
}