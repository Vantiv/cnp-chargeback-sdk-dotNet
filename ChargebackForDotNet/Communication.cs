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
        private HttpWebRequest _httpRequest;
        private WebHeaderCollection _headers;
        private WebProxy _webProxy;
        private string _contentType;
        private string _accept;
        private string _host;

        public Communication()
        {
            _headers = new WebHeaderCollection();
        }

        public void AddToHeader(string key, string value)
        {
            if (_headers.AllKeys.Contains(key))
            {
                _headers[key] = value;
            }
            else
            {
                _headers.Add(key, value);
            }
        }

        public void SetHost(string host)
        {
            _host = host;
        }

        public void SetProxy(string host, int port)
        {
            _webProxy = new WebProxy(host, port);
        }

        public void SetContentType(string contentType)
        {
            _contentType = contentType;
        }

        public void SetAccept(string accept)
        {
            _accept = accept;
        }

        public virtual ResponseContent Get(string urlRoute)
        {
            CreateNewHttpRequest(urlRoute);
            _httpRequest.Method = "GET";
            return ReceiveResponse();
        }

        public virtual ResponseContent Delete(string urlRoute)
        {
            CreateNewHttpRequest(urlRoute);
            _httpRequest.Method = "DELETE";
            return ReceiveResponse();
        }

        public virtual ResponseContent Post(string urlRoute, List<byte> sendingBytes)
        {
            CreateNewHttpRequest(urlRoute);
            _httpRequest.Method = "POST";
            WriteBytesToRequestStream(sendingBytes);
            return ReceiveResponse();
        }

        public virtual ResponseContent Put(string urlRoute, List<byte> sendingBytes)
        {
            CreateNewHttpRequest(urlRoute);
            _httpRequest.Method = "PUT";
            WriteBytesToRequestStream(sendingBytes);
            return ReceiveResponse();
        }

        private void WriteBytesToRequestStream(List<byte> sendingBytes)
        {
            _httpRequest.ContentLength = sendingBytes.Count;
            Stream inStream = _httpRequest.GetRequestStream();
            inStream.Write(sendingBytes.ToArray(), 0, sendingBytes.Count);
            inStream.Close();
        }

        private void CreateNewHttpRequest(string urlRoute)
        {
            string url = _host + urlRoute;
            Console.WriteLine("Making a request to " + url);
            // For TLS1.2.
            ServicePointManager.SecurityProtocol = (SecurityProtocolType) 3072;
            _httpRequest = (HttpWebRequest) WebRequest.Create(url);
            _httpRequest.Headers.Add(_headers);
            if (_webProxy != null)
            {
                _httpRequest.Proxy = _webProxy;
            }

            if (_contentType != null)
            {
                _httpRequest.ContentType = _contentType;
            }

            if (_accept != null)
            {
                _httpRequest.Accept = _accept;
            }
        }

        private ResponseContent ReceiveResponse()
        {
            var httpResponse = (HttpWebResponse) _httpRequest.GetResponse();
            var receivingbytes = new List<byte>();
            var contentType = httpResponse.ContentType;

            var responseStream = httpResponse.GetResponseStream();
            var b = responseStream.ReadByte();
            while (b != -1)
            {
                receivingbytes.Add((byte) b);
                b = responseStream.ReadByte();
            }

            responseStream.Close();
            httpResponse.Close();
            return new ResponseContent(contentType, receivingbytes);
        }
    }
}