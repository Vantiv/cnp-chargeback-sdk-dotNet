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

        public Communication()
        {
            headers = new WebHeaderCollection();
        }

        public void AddToHeader(string key, string value)
        {
            if (headers.AllKeys.Contains(key))
            {
                headers[key] = value;
            }
            else
            {
                headers.Add(key, value);
            }
        }

        public void SetHost(string host)
        {
            this.host = host;
        }

        public void SetProxy(string host, int port)
        {
            webProxy = new WebProxy(host, port);
        }

        public void SetContentType(string contentType)
        {
            this.contentType = contentType;
        }

        public void SetAccept(string accept)
        {
            this.accept = accept;
        }

        public virtual ArrayList Get(string urlRoute)
        {
            createNewHttpRequest(urlRoute);
            httpRequest.Method = "GET";
            return receiveResponse();
        }

        public virtual ArrayList Delete(string urlRoute)
        {
            createNewHttpRequest(urlRoute);
            httpRequest.Method = "DELETE";
            return receiveResponse();
        }

        public virtual ArrayList Post(string urlRoute, List<byte> sendingBytes)
        {
            createNewHttpRequest(urlRoute);
            httpRequest.Method = "POST";
            writeBytesToRequestStream(sendingBytes);
            return receiveResponse();
        }

        public virtual ArrayList Put(string urlRoute, List<byte> sendingBytes)
        {
            createNewHttpRequest(urlRoute);
            httpRequest.Method = "PUT";
            writeBytesToRequestStream(sendingBytes);
            return receiveResponse();
        }

        private void writeBytesToRequestStream(List<byte> sendingBytes)
        {
            httpRequest.ContentLength = sendingBytes.Count;
            Stream inStream = httpRequest.GetRequestStream();
            inStream.Write(sendingBytes.ToArray(), 0, sendingBytes.Count);
            inStream.Close();
        }

        private void createNewHttpRequest(string urlRoute)
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
        }

        private ArrayList receiveResponse()
        {
            HttpWebResponse httpResponse = (HttpWebResponse) httpRequest.GetResponse();
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
    }
}