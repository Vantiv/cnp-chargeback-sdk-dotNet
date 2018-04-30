using System.Net;
using ChargebackForDotNet.Properties;

namespace ChargebackForDotNet
{
    public class ChargebackUpdateRequest
    {
        private const string ServiceRoute = "/chargebacks";
        
        private Configuration _configurationField;

        public Configuration Config
        {
            get
            {
                if (_configurationField == null)
                {
                    // load from file
                    _configurationField = new Configuration();
                }
                return _configurationField;
            }
            set { _configurationField = value; }
        }

        private Communication communication;

        public ChargebackUpdateRequest()
        {
            communication = new Communication();
        }
        
        public ChargebackUpdateRequest(Communication comm)
        {
            communication = comm;
        }
        
        public chargebackUpdateResponse AssignToUser(long caseId, string assignedTo , string note = null)
        {
            var xmlBody = "";
            xmlBody += SerializeActivityType(activityType.ASSIGN_TO_USER);
            xmlBody += SerializeAssignedTo(assignedTo);
            if (note != null)
            {
                xmlBody += SerializeNote(note);
            }            
            return SendUpdateRequest(caseId, xmlBody);
        }

        
        public chargebackUpdateResponse AddNote(long caseId, string note)
        {
            string xmlBody = "";
            xmlBody += SerializeActivityType(activityType.ADD_NOTE);
            xmlBody += SerializeNote(note);
            return SendUpdateRequest(caseId, xmlBody);
        }
        
        public chargebackUpdateResponse AcceptLiability(long caseId, string note = null)
        {
            string xmlBody = "";
            xmlBody += SerializeActivityType(activityType.MERCHANT_ACCEPTS_LIABILITY);
            if (note != null)
            {
                xmlBody += SerializeNote(note);
            }
            return SendUpdateRequest(caseId, xmlBody);
        }
        
        public chargebackUpdateResponse Represent(long caseId, long representedAmount, string note = null)
        {
            string xmlBody = "";
            xmlBody += SerializeActivityType(activityType.MERCHANT_REPRESENT);
            if (note != null)
            {
                xmlBody += SerializeNote(note);
            }
            xmlBody += SerializeRepresentedAmount(representedAmount);
            return SendUpdateRequest(caseId, xmlBody);
        }

        
        public chargebackUpdateResponse Represent(long caseId, string note = null)
        {
            string xmlBody = "";
            xmlBody += SerializeActivityType(activityType.MERCHANT_REPRESENT);
            if (note != null)
            {
                xmlBody += SerializeNote(note);
            }
            return SendUpdateRequest(caseId, xmlBody);
        }
        
        public chargebackUpdateResponse Respond(long caseId, string note = null)
        {
            string xmlBody = "";
            xmlBody += SerializeActivityType(activityType.MERCHANT_RESPOND);
            if (note != null)
            {
                xmlBody += SerializeNote(note);
            }
            return SendUpdateRequest(caseId, xmlBody);
        }
        
        public chargebackUpdateResponse RequestArbitration(long caseId, string note = null)
        {
            var xmlBody = "";
            xmlBody += SerializeActivityType(activityType.MERCHANT_REQUESTS_ARBITRATION);
            if (note != null)
            {
                xmlBody += SerializeNote(note);
            }
            return SendUpdateRequest(caseId, xmlBody);
        }

        private static string Serialize(string xmlBody)
        {
            const string xmlHeader = "<?xml version=\"1.0\" encoding=\"UTF-8\" standalone=\"yes\"?>" +
                                     "\n<chargebackUpdateRequest xmlns=\"http://www.vantivcnp.com/chargebacks\">";
            const string xmlFooter = "\n</chargebackUpdateRequest>";
            return xmlHeader + xmlBody + xmlFooter;
        }
        
        private static string SerializeNote(string note)
        {
            return "\n<note>" + note + "</note>";
        }

        private static string SerializeActivityType(activityType activityType)
        {
            return "\n<activityType>" + activityType + "</activityType>";
        }
        
        private static string SerializeRepresentedAmount(long representedAmount)
        {
            return "\n<representedAmount>" + representedAmount + "</representedAmount>";
        }
        
        private static string SerializeAssignedTo(string assignedTo)
        {
            return "\n<assignedTo>" + assignedTo + "</assignedTo>";
        }

        
        private chargebackUpdateResponse SendUpdateRequest(long caseId, string xmlBody)
        {
            string xmlRequest = Serialize(xmlBody);
            ChargebackUtils.PrintXml(xmlRequest, Config.Get("printXml"), Config.Get("neuterXml"));
            try
            {
                ConfigureCommunication();
                var responseContent = communication.Put(ServiceRoute + "/" + caseId, ChargebackUtils.StringToBytes(xmlRequest));
                var receivedBytes = responseContent.GetByteData();
                var xmlResponse = ChargebackUtils.BytesToString(receivedBytes);
                ChargebackUtils.PrintXml(xmlResponse, Config.Get("printXml"), Config.Get("neuterXml"));
                return ChargebackUtils.DeserializeResponse<chargebackUpdateResponse>(xmlResponse);
            }
            catch (WebException we)
            {
                throw ChargebackUpdateWebException(we);
            }
        }
        

        private void ConfigureCommunication()
        {
            communication.SetHost(Config.Get("host"));
            string encoded = ChargebackUtils.Encode64(Config.Get("username") + ":" + Config.Get("password"), "utf-8");
            communication.AddToHeader("Authorization", "Basic " + encoded);
            communication.SetContentType("application/com.vantivcnp.services-v2+xml");
            communication.SetAccept("application/com.vantivcnp.services-v2+xml");
            communication.SetProxy(Config.Get("proxyHost"), int.Parse(Config.Get("proxyPort")));
        }


        private ChargebackWebException ChargebackUpdateWebException(WebException we)
        {
            var webErrorResponse = (HttpWebResponse) we.Response;
            var httpStatusCode = (int) webErrorResponse.StatusCode;
            var rawResponse = ChargebackUtils.GetResponseXml(webErrorResponse);
            if (!webErrorResponse.ContentType.Contains("application/com.vantivcnp.services-v2+xml"))
            {
                return new ChargebackWebException(string.Format("Update Failed - HTTP {0} Error", httpStatusCode), httpStatusCode, rawResponse);
            }
            ChargebackUtils.PrintXml(rawResponse, Config.Get("printXml"), Config.Get("neuterXml"));
            var errorResponse = ChargebackUtils.DeserializeResponse<errorResponse>(rawResponse);
            var errorMessages = errorResponse.errors;
            return new ChargebackWebException(string.Format("Update Failed - HTTP {0} Error", httpStatusCode), httpStatusCode, rawResponse, errorMessages);
        }
    }
}