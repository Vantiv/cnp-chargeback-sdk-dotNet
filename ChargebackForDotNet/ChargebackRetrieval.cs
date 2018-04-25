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

        private Communication communication;

        private const string SERVICE_ROUTE = "/chargebacks";

        public ChargebackRetrievalRequest()
        {
            communication = new Communication();
        }
        
        public ChargebackRetrievalRequest(Communication comm)
        {
            communication = comm;
        }
        
        public chargebackRetrievalResponse RetrieveByActivityDate(DateTime date)
        {
            string queryDate = date.ToString("yyyy-MM-dd");
            string xmlResponse = sendRetrievalRequest(SERVICE_ROUTE + "/?date=" + queryDate);
            return ChargebackUtils.DeserializeResponse<chargebackRetrievalResponse>(xmlResponse);
        }
        
        public chargebackRetrievalResponse RetrieveByActivityDateWithImpact(DateTime date, bool financialImpact)
        {
            string queryDate = date.ToString("yyyy-MM-dd");
            string queryFinancialImpact = financialImpact.ToString();
            string xmlResponse = sendRetrievalRequest(string.Format(SERVICE_ROUTE+"/?date={0}&financialOnly={1}",
                queryDate, queryFinancialImpact));
            return ChargebackUtils.DeserializeResponse<chargebackRetrievalResponse>(xmlResponse);
        }
        
        public chargebackRetrievalResponse RetrieveByActionable(bool actionable)
        {
            string xmlResponse = sendRetrievalRequest(
                string.Format(SERVICE_ROUTE+"/?actionable={0}", actionable));
            return ChargebackUtils.DeserializeResponse<chargebackRetrievalResponse>(xmlResponse);
        }
        
        public chargebackRetrievalResponse RetrieveByCaseId(long caseId)
        {
            string xmlResponse = sendRetrievalRequest(
                string.Format(SERVICE_ROUTE+"/{0}", caseId));
            return ChargebackUtils.DeserializeResponse<chargebackRetrievalResponse>(xmlResponse);
        }
        
        public chargebackRetrievalResponse RetrieveByToken(string token)
        {
            string xmlResponse = sendRetrievalRequest(
                string.Format(SERVICE_ROUTE+"/?token={0}", token));
            return ChargebackUtils.DeserializeResponse<chargebackRetrievalResponse>(xmlResponse);
        }
        
        public chargebackRetrievalResponse RetrieveByCardNumber(string cardNumber, int month, int year)
        {
            var expirationDate = new DateTime(year, month, 1);
            string queryExpirationDate = expirationDate.ToString("MMyy");
            string xmlResponse = sendRetrievalRequest(
                string.Format(SERVICE_ROUTE+"/?cardNumber={0}&expirationDate={1}",
                cardNumber, queryExpirationDate));
            return ChargebackUtils.DeserializeResponse<chargebackRetrievalResponse>(xmlResponse);
        }
        
        public chargebackRetrievalResponse RetrieveByArn(string arn)
        {
            string xmlResponse = sendRetrievalRequest(string.Format(SERVICE_ROUTE+"/?arn={0}",
                arn));
            return ChargebackUtils.DeserializeResponse<chargebackRetrievalResponse>(xmlResponse);
        }

        private string sendRetrievalRequest(string urlRoute)
        {
            // Handle exception.
            try
            {
                configureCommunication();
                var responseTuple = communication.Get(urlRoute);
                var receivedBytes = (List<byte>) responseTuple[1];
                String xmlResponse = ChargebackUtils.BytesToString(receivedBytes);
                if (Boolean.Parse(config.Get("printXml")))
                {
                    Console.WriteLine(xmlResponse);
                }
                return xmlResponse;
            }
            catch (WebException we)
            {
                HttpWebResponse errorResponse = (HttpWebResponse) we.Response;
                string errString = ChargebackUtils.ListErrors(errorResponse);
                throw new ChargebackException(
                    String.Format("Retrieval Failed - HTTP {0} Error", (int)errorResponse.StatusCode)+errString);
            }
        }
        
        private void configureCommunication()
        {
            Console.WriteLine("Called");
            communication.SetHost(config.Get("host"));
            string encoded = ChargebackUtils.Encode64(config.Get("username") + ":" + config.Get("password"), "utf-8");
            communication.AddToHeader("Authorization", "Basic " + encoded);
            communication.SetContentType("application/com.vantivcnp.services-v2+xml");
            communication.SetAccept("application/com.vantivcnp.services-v2+xml");
            communication.SetProxy(config.Get("proxyHost"), Int32.Parse(config.Get("proxyPort")));
        }
    }
    
    public partial class chargebackRetrievalResponse
    {
        // Additional implementation for chargebackRetrievalResponse
        // should be in here.
    }
}