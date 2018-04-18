using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Xml.Serialization;

namespace ChargebackForDotNet
{
    public class ChargebackException: Exception
    {
        private HttpWebResponse errResponse;
        public string Message;
        public string[] errors;

        private ChargebackException(string message, string[] errors) : base(message)
        {
            this.errors = errors;
            this.Message = message;
        }

        public ChargebackException(string message) : base(message)
        {
            this.Message = base.Message + "\n" + message;
        }

        public ChargebackException(string message, HttpWebResponse errorResponse)
        {
            this.Message = base.Message+"\n"+message;
            this.errResponse = errorResponse;
            deserialize();
            readErrorMessages();
            throw new ChargebackException(Message, errors);
        }

        private void deserialize()
        {
            StreamReader reader = new StreamReader(errResponse.GetResponseStream());
            string xmlResponse = reader.ReadToEnd().Trim();
            errors = Utils.DeserializeResponse<errorResponse>(xmlResponse).errors;
            Console.WriteLine("Length of errors:" + errors.Length);
            Console.WriteLine(xmlResponse);
        }

        private void readErrorMessages()
        {
            foreach (var e in errors)
            {
                Message += "\n" + e;
            }
        }
    }
}