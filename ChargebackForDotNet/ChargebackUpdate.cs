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

        private Communication communication;

        public chargebackUpdateRequest()
        {
            communication = new Communication();
        }

        public chargebackUpdateRequest(Configuration config)
        {
            this.configurationField = config;
            communication = new Communication();
        }
        
        public void setCommunication(Communication comm)
        {
            communication = comm;
        }

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

        private string Serialize()
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

        private void clearVariables()
        {
            assignedTo = null;
            note = null;
            representedAmount = 0;
            representedAmountFieldSpecified = false;
        }

        private chargebackUpdateResponse sendUpdateRequest()    
        {
            string xml = this.Serialize();
            Console.WriteLine("Request is:");
            Console.WriteLine(xml);
            clearVariables();
            try
            {
                SetUpCommunication();
                var responseTuple = communication.put(SERVICE_ROUTE + "/" + caseId, Utils.stringToBytes(xml));
                var receivedBytes = (List<byte>) responseTuple[1];
                string xmlResponse = Utils.bytesToString(receivedBytes);
                Console.WriteLine(xmlResponse);
                return Utils.DeserializeResponse<chargebackUpdateResponse>(xmlResponse);
            }
            catch (WebException we)
            {
                HttpWebResponse errorResponse = (HttpWebResponse) we.Response;
                throw new ChargebackException(
                    string.Format("Update Failed - HTTP {0} Error", (int) errorResponse.StatusCode), errorResponse);
            }
        }

        private void SetUpCommunication()
        {
            communication.setHost(config.getConfig("host"));
            string encoded = Utils.encode64(config.getConfig("username") + ":" + config.getConfig("password"), "utf-8");
            communication.addToHeader("Authorization", "Basic " + encoded);
            communication.setContentType("application/com.vantivcnp.services-v2+xml");
            communication.setAccept("application/com.vantivcnp.services-v2+xml");
            communication.setProxy(config.getConfig("proxyHost"), int.Parse(config.getConfig("proxyPort")));
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
    }
     
    public partial class chargebackUpdateResponse
    {
        // Additional implementation for chargebackUpdateResponse
        // should be in here.
    }
}