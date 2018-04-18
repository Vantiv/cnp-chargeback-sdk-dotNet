using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Xml.Serialization;
using ChargebackForDotNet.Properties;

namespace ChargebackForDotNet
{
    public abstract partial class chargebackUpdateRequest
    {
    
        protected activityType activityType;
    
        protected string assignedTo;
    
        protected string note;
    
        protected long representedAmount;
    
        protected bool representedAmountFieldSpecified;
        
        private Configuration configurationField;

        private long caseId;

        public chargebackUpdateRequest(long caseId)
        {
            this.caseId = caseId;
        }

        public Configuration config
        {
            get
            {
                if (configurationField == null)
                {
                    // load from file
                    return null;
                }
                else
                {
                    return this.configurationField;
                }
            }
            set { this.configurationField = value; }
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

        public chargebackUpdateResponse sendUpdateRequest()    
        {
            string xml = this.Serialize();
            try
            {
                List<byte> bytes = new List<byte>();
                Communication.put(config, "/chargebacks/" + caseId,
                    Utils.stringToBytes(xml), bytes);
                String xmlResponse = Utils.bytesToString(bytes);
                Console.WriteLine(xmlResponse);
                
                return Utils.DeserializeResponse<chargebackUpdateResponse>(xmlResponse);
            }
            catch (WebException we)
            {
                HttpWebResponse errorResponse = (HttpWebResponse) we.Response;
                throw new ChargebackException(
                    String.Format("Update Failed - HTTP {0} Error", (int)errorResponse.StatusCode), errorResponse);
            }
        }
    }

    public class UserAssignRequest:chargebackUpdateRequest
    {
        public UserAssignRequest(long caseId, string assignedTo = null, string note = null):base(caseId)
        {
            base.activityType = activityType.ASSIGN_TO_USER;
            base.assignedTo = assignedTo;
            base.note = note;
        }
    }

    public class NoteRequest:chargebackUpdateRequest
    {
        public NoteRequest(long caseId, string note = null):base(caseId)
        {
            base.activityType = activityType.ADD_NOTE;
            base.note = note;
        }
    }

    public class MerchantAcceptsLiability:chargebackUpdateRequest
    {
        public MerchantAcceptsLiability(long caseId, string note = null):base(caseId)
        {
            base.activityType = activityType.MERCHANT_ACCEPTS_LIABILITY;
            base.note = note;
        }
    }

    public class MerchantRepresent : chargebackUpdateRequest
    {
        public MerchantRepresent(long caseId, long representedAmount, string note = null):base(caseId)
        {
            base.activityType = activityType.MERCHANT_REPRESENT;
            base.note = note;
            base.representedAmount = representedAmount;
            base.representedAmountFieldSpecified = true;
        }
        
        public MerchantRepresent(long caseId, string note = null):base(caseId)
        {
            base.activityType = activityType.MERCHANT_REPRESENT;
            base.note = note;
        }
    }

    public class MerchantResponse : chargebackUpdateRequest
    {
        public MerchantResponse(long caseId, string note = null):base(caseId)
        {
            base.activityType = activityType.MERCHANT_RESPOND;
            base.note = note;
        }
    }

    public class MerchantRequestsArbitration : chargebackUpdateRequest
    {
        public MerchantRequestsArbitration(long caseId, string note = null):base(caseId)
        {
            base.activityType = activityType.MERCHANT_REQUESTS_ARBITRATION;
            base.note = note;
        }
    }
     
    public partial class chargebackUpdateResponse
    {
        // Additional implementation for chargebackUpdateResponse
        // should be in here.
    }
}