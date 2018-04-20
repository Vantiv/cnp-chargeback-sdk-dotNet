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
        
        private Configuration configurationField;

        public chargebackUpdateRequest()
        {
        }

        public chargebackUpdateRequest(Configuration config)
        {
            this.configurationField = config;
        }

        public Configuration config
        {
            get
            {
                if (configurationField == null)
                {
                    // load from file
                    return new Configuration();
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
            clearVariables();
            try
            {
                List<byte> bytes = new List<byte>();

                Communication c = createCommunication();
                c.put("/chargebacks/" + caseId, Utils.stringToBytes(xml), bytes);
                
                String xmlResponse = Utils.bytesToString(bytes);
                Console.WriteLine(xmlResponse);
                return Utils.DeserializeResponse<chargebackUpdateResponse>(xmlResponse);
            }
            catch (WebException we)
            {
                HttpWebResponse errorResponse = (HttpWebResponse) we.Response;
                throw new ChargebackException(
                    String.Format("Update Failed - HTTP {0} Error", (int) errorResponse.StatusCode), errorResponse);
            }
        }

        private Communication createCommunication()
        {
            Communication c= new Communication(config.getConfig("host"));
            string encoded = Utils.encode64(config.getConfig("username") + ":" + config.getConfig("password"), "utf-8");
            c.addToHeader("Authorization", "Basic " + encoded);
            c.setContentType("application/com.vantivcnp.services-v2+xml");
            c.setAccept("application/com.vantivcnp.services-v2+xml");
            c.setProxy(config.getConfig("proxyHost"), Int32.Parse(config.getConfig("proxyPort")));
            return c;
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