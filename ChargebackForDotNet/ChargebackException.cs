using System;
using System.IO;
using System.Linq;
using System.Management.Instrumentation;
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
        public readonly string Message;
        public readonly string rawReponse;
        public readonly int httpStatusCode;
        public readonly string[] errorMessages;

        public ChargebackWebException(string message, string raw) : base(message)
        {
            Message = message;
            rawReponse = raw;
        }

        public ChargebackWebException(string message, int httpStatusCode, string rawReponse)
        {
            Message = message;
            this.httpStatusCode = httpStatusCode;
            this.rawReponse = rawReponse;  
        }

        public ChargebackWebException(string message, int httpStatusCode, string errResponseXml, string[] errorMessages) 
            : base(message + ExtractErrorMessages(errorMessages))
        {
            Message = message;
            this.httpStatusCode = httpStatusCode;
            this.errorMessages = errorMessages;
            rawReponse = errResponseXml;
        }

        private static string ExtractErrorMessages(string[] errorMessages)
        {
            string errString = "";
            foreach (var err in errorMessages)
            {
                errString += "\n" + err;
            }
            return errString;
        }
    }

    
    public class ChargebackDocumentException: Exception
    {
        public string Message;
        public string rawReponse;
        public string responseCode; 

        public ChargebackDocumentException(string message, string responseCode, string raw) : base(message)
        {
            Message = message;
            this.responseCode = responseCode;
            rawReponse = raw;
        }
    }

    public partial class errorResponse
    {
        // Additional implementation for errorResponse class
        // should be written here.
    }
}