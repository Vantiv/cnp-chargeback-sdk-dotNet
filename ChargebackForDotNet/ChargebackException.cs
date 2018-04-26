using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Xml.Serialization;
using ChargebackForDotNet.Properties;

namespace ChargebackForDotNet
{
    public class ChargebackException: Exception
    {
        public string Message;

        public ChargebackException(string message) : base(message)
        {
            Message = message;
        }
    }
    
    public class ChargebackWebException: Exception
    {
        public string Message;
        public string rawReponse;

        public ChargebackWebException(string message, string raw) : base(message)
        {
            Message = message;
            rawReponse = raw;
        }
    }

    public class ChargebackDocumentException: Exception
    {
        public string Message;
        public string rawReponse;

        public ChargebackDocumentException(string message, string raw) : base(message)
        {
            Message = message;
            rawReponse = raw;
        }
    }

    public partial class errorResponse
    {
        // Additional implementation for errorResponse class
        // should be written here.
    }
}