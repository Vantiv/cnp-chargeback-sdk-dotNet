using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Xml.Serialization;
using ChargebackForDotNet.Properties;

namespace ChargebackForDotNet
{
    public partial class chargebackUpdateRequest
    {
    
        private activityType activityType;
        private string assignedTo;
        private string note;
        private long representedAmount;
        private bool representedAmountFieldSpecified;
        private long caseId;
        private const string SERVICE_ROUTE = "/chargebacks";
        
        private Configuration configurationField;

        public Configuration config
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
        
        public chargebackUpdateResponse AssignToUser(long caseId, string assignedTo = null, string note = null)
        {
            this.caseId = caseId;
            this.activityType = activityType.ASSIGN_TO_USER;
            this.assignedTo = assignedTo;
            this.note = note;
            return sendUpdateRequest();
        }
        
        public chargebackUpdateResponse AddNote(long caseId, string note = null)
        {
            this.caseId = caseId;
            this.activityType = activityType.ADD_NOTE;
            this.note = note;
            return sendUpdateRequest();
        }
        
        public chargebackUpdateResponse AcceptLiability(long caseId, string note = null)
        {
            this.caseId = caseId;
            this.activityType = activityType.MERCHANT_ACCEPTS_LIABILITY;
            this.note = note;
            return sendUpdateRequest();
        }
        
        public chargebackUpdateResponse Represent(long caseId, long representedAmount, string note = null)
        {
            this.caseId = caseId;
            this.activityType = activityType.MERCHANT_REPRESENT;
            this.note = note;
            this.representedAmount = representedAmount;
            this.representedAmountFieldSpecified = true;
            return sendUpdateRequest();
        }
        
        public chargebackUpdateResponse Represent(long caseId, string note = null)
        {
            this.caseId = caseId;
            this.activityType = activityType.MERCHANT_REPRESENT;
            this.note = note;
            return sendUpdateRequest();
        }
        
        public chargebackUpdateResponse Respond(long caseId, string note = null)
        {
            this.caseId = caseId;
            this.activityType = activityType.MERCHANT_RESPOND;
            this.note = note;
            return sendUpdateRequest();
        }
        
        public chargebackUpdateResponse RequestArbitration(long caseId, string note = null)
        {
            this.caseId = caseId;
            this.activityType = activityType.MERCHANT_REQUESTS_ARBITRATION;
            this.note = note;
            return sendUpdateRequest();
        }

        private string serialize()
        {
            string header = "<?xml version=\"1.0\" encoding=\"UTF-8\" standalone=\"yes\"?>" +
                            "\n<chargebackUpdateRequest xmlns=\"http://www.vantivcnp.com/chargebacks\">";
            string footer = "\n</chargebackUpdateRequest>";
            string body = "";
            
            body += string.Format("\n<activityType>{0}</activityType>", this.activityType);
            if (this.assignedTo != null)
            {
                body += string.Format("\n<assignedTo>{0}</assignedTo>", this.assignedTo);
            }

            if (this.note != null)
            {
                body += string.Format("\n<note>{0}</note>", this.note);
            }

            if (this.representedAmountFieldSpecified)
            {
                body += string.Format("\n<representedAmount>{0}</representedAmount>",this.representedAmount);
            }
            return header+body+footer;
        }

        private void unsetFields()
        {
            assignedTo = null;
            note = null;
            representedAmount = 0;
            representedAmountFieldSpecified = false;
        }

        private chargebackUpdateResponse sendUpdateRequest()
        {
            string xml = this.serialize();
            if (Boolean.Parse(config.Get("printXml")))
            {
                Console.WriteLine("Request is:");
                Console.WriteLine(xml);
            }
            unsetFields();
            try
            {
                configureCommunication();
                var responseTuple = communication.Put(SERVICE_ROUTE + "/" + caseId, ChargebackUtils.StringToBytes(xml));
                var receivedBytes = (List<byte>) responseTuple[1];
                string xmlResponse = ChargebackUtils.BytesToString(receivedBytes);
                if (Boolean.Parse(config.Get("printXml")))
                {
                    Console.WriteLine(xmlResponse);
                }
                return ChargebackUtils.DeserializeResponse<chargebackUpdateResponse>(xmlResponse);
            }
            catch (WebException we)
            {
                HttpWebResponse errorResponse = (HttpWebResponse) we.Response;
                string errString = ChargebackUtils.ListErrors(errorResponse);
                throw new ChargebackException(
                    string.Format("Update Failed - HTTP {0} Error", (int) errorResponse.StatusCode) + errString);
            }
        }

        private void configureCommunication()
        {
            communication.SetHost(config.Get("host"));
            string encoded = ChargebackUtils.Encode64(config.Get("username") + ":" + config.Get("password"), "utf-8");
            communication.AddToHeader("Authorization", "Basic " + encoded);
            communication.SetContentType("application/com.vantivcnp.services-v2+xml");
            communication.SetAccept("application/com.vantivcnp.services-v2+xml");
            communication.SetProxy(config.Get("proxyHost"), int.Parse(config.Get("proxyPort")));
        }
    }
     
    public partial class chargebackUpdateResponse
    {
        // Additional implementation for chargebackUpdateResponse
        // should be in here.
    }
}