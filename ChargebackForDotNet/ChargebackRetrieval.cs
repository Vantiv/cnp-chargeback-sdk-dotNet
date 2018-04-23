using System;
using System.CodeDom;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.IO;
using System.Net;
using System.Xml.Serialization;
using ChargebackForDotNet.Properties;

namespace ChargebackForDotNet
{
    public partial class chargebackApiActivity
    {
    }

    public partial class chargebackApiCase
    {
    }
    
    public class ChargebackRetrievalRequest
    {
        private Configuration configurationField;

        private Communication communication;

        public Configuration config
        {
            get
            {
                if (configurationField == null)
                {
                    // Load from Settings.
                     configurationField = new Configuration();
                }
                return configurationField;
            }
            set { configurationField = value; }
        }

        public ChargebackRetrievalRequest()
        {
            communication = new Communication();
        }

        public ChargebackRetrievalRequest(Configuration config)
        {
            this.configurationField = config;
            communication = new Communication();
        }
        
        public void setCommunication(Communication comm)
        {
            communication = comm;
        }

        private const string SERVICE_ROUTE = "/chargebacks";

        private string sendRequest(string urlRoute)
        {
            // Handle exception.
            try
            {
                SetUpCommunication();
                var responseTuple = communication.get(urlRoute);
                var receivedBytes = (List<byte>) responseTuple[1];
                String xmlResponse = Utils.bytesToString(receivedBytes);
                Console.WriteLine(xmlResponse);
                return xmlResponse;
            }
            catch (WebException we)
            {
                HttpWebResponse errorResponse = (HttpWebResponse) we.Response;
                throw new ChargebackException(
                    String.Format("Retrieval Failed - HTTP {0} Error", (int)errorResponse.StatusCode), errorResponse);
            }
        }
        
        private void SetUpCommunication()
        {
            communication.setHost(config.getConfig("host"));
            string encoded = Utils.encode64(config.getConfig("username") + ":" + config.getConfig("password"), "utf-8");
            communication.addToHeader("Authorization", "Basic " + encoded);
            communication.setContentType("application/com.vantivcnp.services-v2+xml");
            communication.setAccept("application/com.vantivcnp.services-v2+xml");
            communication.setProxy(config.getConfig("proxyHost"), Int32.Parse(config.getConfig("proxyPort")));
        }
        
        public chargebackRetrievalResponse retrieveByActivityDate(DateTime date)
        {
            string queryDate = date.ToString("yyyy-MM-dd");
            string xmlResponse = sendRequest(SERVICE_ROUTE + "/?date=" + queryDate);
            return Utils.DeserializeResponse<chargebackRetrievalResponse>(xmlResponse);
        }
        
        public chargebackRetrievalResponse retrieveByActivityDateWithImpact(DateTime date, bool financialImpact)
        {
            string queryDate = date.ToString("yyyy-MM-dd");
            string queryFinancialImpact = financialImpact.ToString();
            string xmlResponse = sendRequest(string.Format(SERVICE_ROUTE+"/?date={0}&financialOnly={1}",
                queryDate, queryFinancialImpact));
            return Utils.DeserializeResponse<chargebackRetrievalResponse>(xmlResponse);
        }
        
        public chargebackRetrievalResponse retrieveActionable(bool actionable)
        {
            string queryActionable = actionable.ToString().ToLower();
            string xmlResponse = sendRequest(
                string.Format(SERVICE_ROUTE+"/?actionable={0}", queryActionable));
            return Utils.DeserializeResponse<chargebackRetrievalResponse>(xmlResponse);
        }
        
        public chargebackRetrievalResponse retrieveByCaseId(long caseId)
        {
            string xmlResponse = sendRequest(
                string.Format(SERVICE_ROUTE+"/{0}", caseId));
            return Utils.DeserializeResponse<chargebackRetrievalResponse>(xmlResponse);
        }
        
        public chargebackRetrievalResponse retrieveByToken(string token)
        {
            string xmlResponse = sendRequest(
                string.Format(SERVICE_ROUTE+"/?token={0}", token));
            return Utils.DeserializeResponse<chargebackRetrievalResponse>(xmlResponse);
        }
        
        public chargebackRetrievalResponse retrieveByCardNumber(string cardNumber, DateTime expirationDate)
        {
            string queryExpirationDate = expirationDate.ToString("MMyy");
            string xmlResponse = sendRequest(
                string.Format(SERVICE_ROUTE+"/?cardNumber={0}&expirationDate={1}",
                cardNumber, queryExpirationDate));
            return Utils.DeserializeResponse<chargebackRetrievalResponse>(xmlResponse);
        }
        
        public chargebackRetrievalResponse retrieveByArn(string arn)
        {
            string xmlResponse = sendRequest(string.Format(SERVICE_ROUTE+"/?arn={0}",
                arn));
            return Utils.DeserializeResponse<chargebackRetrievalResponse>(xmlResponse);
        }
    }
    
    public partial class chargebackRetrievalResponse
    {
        // Additional implementation for chargebackRetrievalResponse
        // should be in here.
    }
}