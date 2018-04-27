using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mime;
using System.Xml.Serialization;
using ChargebackForDotNet.Properties;

namespace ChargebackForDotNet
{
    public partial class chargebackUpdateRequest
    {
        private const string SERVICE_ROUTE = "/chargebacks";
        
        private Configuration configurationField;

        public Configuration Config
        {
            get
            {
                if (configurationField == null)
                {
                    // load from file
                    configurationField = new Configuration();
                }
                return configurationField;
            }
            set { configurationField = value; }
        }

        private Communication communication;

        public chargebackUpdateRequest()
        {
            communication = new Communication();
        }
        
        public chargebackUpdateRequest(Communication comm)
        {
            communication = comm;
        }
        
        public chargebackUpdateResponse AssignToUser(long caseId, string assignedTo , string note = null)
        {
            // what if user is null here
            string xmlBody = "";
            xmlBody += serializeActivityType(activityType.ASSIGN_TO_USER);
            xmlBody += serializeAssignedTo(assignedTo);
            if (note != null)
            {
                xmlBody += serializeNote(note);
            }            
            return sendUpdateRequest(caseId, xmlBody);
        }

        
        public chargebackUpdateResponse AddNote(long caseId, string note)
        {
            string xmlBody = "";
            xmlBody += serializeActivityType(activityType.ADD_NOTE);
            xmlBody += serializeNote(note);
            return sendUpdateRequest(caseId, xmlBody);
        }
        
        public chargebackUpdateResponse AcceptLiability(long caseId, string note = null)
        {
            string xmlBody = "";
            xmlBody += serializeActivityType(activityType.MERCHANT_ACCEPTS_LIABILITY);
            if (note != null)
            {
                xmlBody += serializeNote(note);
            }
            return sendUpdateRequest(caseId, xmlBody);
        }
        
        public chargebackUpdateResponse Represent(long caseId, long representedAmount, string note = null)
        {
            string xmlBody = "";
            xmlBody += serializeActivityType(activityType.MERCHANT_REPRESENT);
            if (note != null)
            {
                xmlBody += serializeNote(note);
            }
            xmlBody += serializeRepresentedAmount(representedAmount);
            return sendUpdateRequest(caseId, xmlBody);
        }

        
        public chargebackUpdateResponse Represent(long caseId, string note = null)
        {
            string xmlBody = "";
            xmlBody += serializeActivityType(activityType.MERCHANT_REPRESENT);
            if (note != null)
            {
                xmlBody += serializeNote(note);
            }
            return sendUpdateRequest(caseId, xmlBody);
        }
        
        public chargebackUpdateResponse Respond(long caseId, string note = null)
        {
            string xmlBody = "";
            xmlBody += serializeActivityType(activityType.MERCHANT_RESPOND);
            if (note != null)
            {
                xmlBody += serializeNote(note);
            }
            return sendUpdateRequest(caseId, xmlBody);
        }
        
        public chargebackUpdateResponse RequestArbitration(long caseId, string note = null)
        {
            string xmlBody = "";
            xmlBody += serializeActivityType(activityType.MERCHANT_REQUESTS_ARBITRATION);
            if (note != null)
            {
                xmlBody += serializeNote(note);
            }
            return sendUpdateRequest(caseId, xmlBody);
        }

        private string serialize(string xmlBody)
        {
            string xmlHeader = "<?xml version=\"1.0\" encoding=\"UTF-8\" standalone=\"yes\"?>" +
                            "\n<chargebackUpdateRequest xmlns=\"http://www.vantivcnp.com/chargebacks\">";
            string xmlFooter = "\n</chargebackUpdateRequest>";
            return xmlHeader + xmlBody + xmlFooter;
        }
        
        private string serializeNote(string note)
        {
            return "\n<note>" + note + "</note>";
        }

        private string serializeActivityType(activityType activityType)
        {
            return "\n<activityType>" + activityType + "</activityType>";
        }
        
        private string serializeRepresentedAmount(long representedAmount)
        {
            return "\n<representedAmount>" + representedAmount + "</representedAmount>";
        }
        
        private string serializeAssignedTo(string assignedTo)
        {
            return "\n<assignedTo>" + assignedTo + "</assignedTo>";
        }

        
        private chargebackUpdateResponse sendUpdateRequest(long caseId, string xmlBody)
        {
            string xmlRequest = serialize(xmlBody);
            if (Boolean.Parse(Config.Get("printXml")))
            {
                Console.WriteLine("Request is:");
                Console.WriteLine(xmlRequest);
            }
            try
            {
                configureCommunication();
                var responseContent = communication.Put(SERVICE_ROUTE + "/" + caseId, ChargebackUtils.StringToBytes(xmlRequest));
                var receivedBytes = responseContent.GetByteData();
                string xmlResponse = ChargebackUtils.BytesToString(receivedBytes);
                if (Boolean.Parse(Config.Get("printXml")))
                {
                    Console.WriteLine(xmlResponse);
                }
                return ChargebackUtils.DeserializeResponse<chargebackUpdateResponse>(xmlResponse);
            }
            catch (WebException we)
            {
                throw ChargebackUpdateWebException(we);
            }
        }
        

        private void configureCommunication()
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
            if (bool.Parse(Config.Get("printXml")))
            {
                Console.WriteLine(rawResponse);
            }
            var errorResponse = ChargebackUtils.DeserializeResponse<errorResponse>(rawResponse);
            var errorMessages = errorResponse.errors;
            return new ChargebackWebException(string.Format("Update Failed - HTTP {0} Error", httpStatusCode), httpStatusCode, rawResponse, errorMessages);
        }
    }
     
    public partial class chargebackUpdateResponse
    {
        // Additional implementation for chargebackUpdateResponse
        // should be in here.
    }
}