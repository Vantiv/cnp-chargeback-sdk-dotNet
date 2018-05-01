using System;

namespace ChargebackSdkForNet
{
    [Serializable]
    public class ChargebackException: Exception
    {
        public string ErrorMessage;

        public ChargebackException(string errorMessage) : base(errorMessage)
        {
            ErrorMessage = errorMessage;
        }
    }

    [Serializable]
    public class ChargebackWebException: Exception
    {
        public readonly string ErrorMessage;
        public readonly string RawReponse;
        public readonly int HttpStatusCode;
        public readonly string[] ErrorMessages;

        public ChargebackWebException(string errorMessage, string raw) : base(errorMessage)
        {
            ErrorMessage = errorMessage;
            RawReponse = raw;
        }

        public ChargebackWebException(string errorMessage, int httpStatusCode, string rawReponse):base(errorMessage)
        {
            ErrorMessage = errorMessage;
            HttpStatusCode = httpStatusCode;
            RawReponse = rawReponse;  
        }

        public ChargebackWebException(string errorMessage, int httpStatusCode, string errResponseXml, string[] errorMessages) 
            : base(errorMessage + ExtractErrorMessages(errorMessages))
        {
            ErrorMessage = errorMessage;
            HttpStatusCode = httpStatusCode;
            ErrorMessages = errorMessages;
            RawReponse = errResponseXml;
        }

        private static string ExtractErrorMessages(string[] errorMessages)
        {
            var errString = "";
            foreach (var err in errorMessages)
            {
                errString += "\n" + err;
            }
            return errString;
        }
    }

    [Serializable]
    public class ChargebackDocumentException: Exception
    {
        public string ErrorMessage;
        public string RawReponse;
        public string ResponseCode; 

        public ChargebackDocumentException(string errorMessage, string responseCode, string raw) : base(errorMessage)
        {
            ErrorMessage = errorMessage;
            ResponseCode = responseCode;
            RawReponse = raw;
        }
    }
}