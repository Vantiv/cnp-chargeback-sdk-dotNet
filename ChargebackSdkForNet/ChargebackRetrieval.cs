using System;
using System.Net;
using ChargebackSdkForNet.Properties;

namespace ChargebackSdkForNet
{
    
    public class ChargebackRetrievalRequest
    {
        private Configuration _configurationField;

        public Configuration Config
        {
            get
            {
                if (_configurationField == null)
                {
                    // Load from Settings.
                    _configurationField = new Configuration();
                }
                return _configurationField;
            }
            set { _configurationField = value; }
        }

        private readonly Communication _communication;

        private const string ServiceRoute = "/chargebacks";

        public ChargebackRetrievalRequest()
        {
            _communication = new Communication();
        }
        
        public ChargebackRetrievalRequest(Communication comm)
        {
            _communication = comm;
        }
        
        public chargebackRetrievalResponse RetrieveByActivityDate(DateTime date)
        {
            var queryDate = date.ToString("yyyy-MM-dd");
            var xmlResponse = SendRetrievalRequest(ServiceRoute + "/?date=" + queryDate);
            return ChargebackUtils.DeserializeResponse<chargebackRetrievalResponse>(xmlResponse);
        }
        
        public chargebackRetrievalResponse RetrieveByActivityDateWithImpact(DateTime date, bool financialImpact)
        {
            var queryDate = date.ToString("yyyy-MM-dd");
            var queryFinancialImpact = financialImpact.ToString();
            var xmlResponse = SendRetrievalRequest(string.Format(ServiceRoute+"/?date={0}&financialOnly={1}",
                queryDate, queryFinancialImpact));
            return ChargebackUtils.DeserializeResponse<chargebackRetrievalResponse>(xmlResponse);
        }
        
        public chargebackRetrievalResponse RetrieveByActionable(bool actionable)
        {
            var xmlResponse = SendRetrievalRequest(
                string.Format(ServiceRoute+"/?actionable={0}", actionable));
            return ChargebackUtils.DeserializeResponse<chargebackRetrievalResponse>(xmlResponse);
        }
        
        public chargebackRetrievalResponse RetrieveByCaseId(long caseId)
        {
            var xmlResponse = SendRetrievalRequest(
                string.Format(ServiceRoute+"/{0}", caseId));
            return ChargebackUtils.DeserializeResponse<chargebackRetrievalResponse>(xmlResponse);
        }
        
        public chargebackRetrievalResponse RetrieveByToken(string token)
        {
            var xmlResponse = SendRetrievalRequest(
                string.Format(ServiceRoute+"/?token={0}", token));
            return ChargebackUtils.DeserializeResponse<chargebackRetrievalResponse>(xmlResponse);
        }
        
        public chargebackRetrievalResponse RetrieveByCardNumber(string cardNumber, int month, int year)
        {
            var expirationDate = new DateTime(year, month, 1);
            var queryExpirationDate = expirationDate.ToString("MMyy");
            var xmlResponse = SendRetrievalRequest(
                string.Format(ServiceRoute+"/?cardNumber={0}&expirationDate={1}",
                cardNumber, queryExpirationDate));
            return ChargebackUtils.DeserializeResponse<chargebackRetrievalResponse>(xmlResponse);
        }
        
        public chargebackRetrievalResponse RetrieveByArn(string arn)
        {
            var xmlResponse = SendRetrievalRequest(string.Format(ServiceRoute+"/?arn={0}",
                arn));
            return ChargebackUtils.DeserializeResponse<chargebackRetrievalResponse>(xmlResponse);
        }

        private string SendRetrievalRequest(string urlRoute)
        {
            try
            {
                ConfigureCommunication();
                var responseContent = _communication.Get(urlRoute);
                var receivedBytes = responseContent.GetByteData();
                var xmlResponse = ChargebackUtils.BytesToString(receivedBytes);
                ChargebackUtils.PrintXml(xmlResponse, Config.Get("printXml"), Config.Get("neuterXml"));
                return xmlResponse;
            }
            catch (WebException we)
            {
                throw ChargebackRetrievalWebException(we);
            }
        }
        
        private void ConfigureCommunication()
        {
            _communication.SetHost(Config.Get("host"));
            var encoded = ChargebackUtils.Encode64(Config.Get("username") + ":" + Config.Get("password"), "utf-8");
            _communication.AddToHeader("Authorization", "Basic " + encoded);
            _communication.SetContentType("application/com.vantivcnp.services-v2+xml");
            _communication.SetAccept("application/com.vantivcnp.services-v2+xml");
            if (!String.IsNullOrEmpty(Config.Get("proxyHost")) && !String.IsNullOrEmpty(Config.Get("proxyPort")))
            {
                _communication.SetProxy(Config.Get("proxyHost"), int.Parse(Config.Get("proxyPort")));
            }
        }       
        
        private ChargebackWebException ChargebackRetrievalWebException(WebException we)
        {
            var webErrorResponse = (HttpWebResponse) we.Response;
            var httpStatusCode = (int) webErrorResponse.StatusCode;
            var rawResponse = ChargebackUtils.GetResponseXml(webErrorResponse);
            if (!webErrorResponse.ContentType.Contains("application/com.vantivcnp.services-v2+xml"))
            {
                return new ChargebackWebException(string.Format("Retrieval Failed - HTTP {0} Error", httpStatusCode), httpStatusCode, rawResponse);
            }
            ChargebackUtils.PrintXml(rawResponse, Config.Get("printXml"), Config.Get("neuterXml"));
            var errorResponse = ChargebackUtils.DeserializeResponse<errorResponse>(rawResponse);
            var errorMessages = errorResponse.errors;
            return new ChargebackWebException(string.Format("Retrieval Failed - HTTP {0} Error", httpStatusCode), httpStatusCode, rawResponse, errorMessages);
        }
    }
}
