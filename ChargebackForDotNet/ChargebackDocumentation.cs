using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Policy;
using System.Xml.Serialization;
using ChargebackForDotNet.Properties;

namespace ChargebackForDotNet
{
    public class ChargebackDocumentationRequest
    {
        private Configuration _configurationField;
        private readonly Communication _communication;
        private const string ServiceRoute = "/services/chargebacks";

        public Configuration Config
        {
            get { return _configurationField ?? (_configurationField = new Configuration()); }
            set { _configurationField = value; }
        }

        public ChargebackDocumentationRequest()
        {
            _communication = new Communication();
        }

        public ChargebackDocumentationRequest(Communication comm)
        {
            _communication = comm;
        }

        public chargebackDocumentUploadResponse UploadDocument(long caseId, string filePath)
        {
            List<byte> fileBytes = File.ReadAllBytes(filePath).ToList();
            string documentId = Path.GetFileName(filePath);
            try
            {
                ConfigureCommunicationForUpload(filePath);
                var responseTuple = _communication.Post(
                    ServiceRoute + "/upload/" + caseId + "/" + documentId, fileBytes);
                return HandleResponse(responseTuple);
            }
            catch (WebException we)
            {
                throw ChargebackDocumentWebException(we, "Upload");
            }
        }

        public chargebackDocumentUploadResponse ReplaceDocument(long caseId, string documentId, string filePath)
        {
            var fileBytes = File.ReadAllBytes(filePath).ToList();
            try
            {
                ConfigureCommunicationForUpload(filePath);
                
                var responseTuple = _communication.Put(
                    ServiceRoute + "/replace/" + caseId + "/" + documentId, fileBytes);
                return HandleResponse(responseTuple);
            }
            catch (WebException we)
            {
                throw ChargebackDocumentWebException(we, "Replace");
            }
        }

        public List<byte> RetrieveDocument(long caseId, string documentId)
        {
            try
            {
                ConfigureCommunication();
                var responseContent = _communication.Get(
                    string.Format(ServiceRoute+"/retrieve/{0}/{1}", caseId, documentId));
                var contentType = responseContent.GetContentType();
                var responseBytes = responseContent.GetByteData();
                if (!"image/tiff".Equals(contentType))
                {
                    var responseString = ChargebackUtils.BytesToString(responseBytes);
                    var docErrorResponse
                        = ChargebackUtils.DeserializeResponse<chargebackDocumentUploadResponse>(responseString);
                    throw new ChargebackDocumentException(docErrorResponse.responseMessage, docErrorResponse.responseCode, responseString);                    
                }

                return responseBytes;
            }
            catch (WebException we)
            {
                throw ChargebackDocumentWebException(we, "Retrieve");
            }
        }

        public chargebackDocumentUploadResponse DeleteDocument(long caseId, string documentId)
        {
            try
            {
                ConfigureCommunication();
                var responseContent = _communication.Delete(
                    string.Format(ServiceRoute+"/remove/{0}/{1}", caseId, documentId));
                return HandleResponse(responseContent);
            }
            catch (WebException we)
            {
                throw ChargebackDocumentWebException(we, "Delete");
            }
        }

        public chargebackDocumentUploadResponse ListDocuments(long caseId)
        {
            try
            {
                ConfigureCommunication();
                var responseContent = _communication.Get(
                    ServiceRoute + "/list/" + caseId);
                return HandleResponse(responseContent);
            }
            catch (WebException we)
            {
                throw ChargebackDocumentWebException(we, "List");
            }
        }
        
        private chargebackDocumentUploadResponse HandleResponse(ResponseContent responseContent)
        {
            var contentType = responseContent.GetContentType();
            var responseBytes = responseContent.GetByteData();
            if (!contentType.Contains("application/com.vantivcnp.services-v2+xml"))
            {
                var stringResponse = ChargebackUtils.BytesToString(responseBytes);
                throw new ChargebackException(
                    string.Format("Unexpected returned Content-Type: {0}. Call Vantiv immediately!" +
                                  "\nAttempting to read the response as raw text:" +
                                  "\n{1}", contentType, stringResponse));
            }
            var xmlResponse = ChargebackUtils.BytesToString(responseBytes);
            if (bool.Parse(Config.Get("printXml")))
            {
                Console.WriteLine(xmlResponse);
            }
            var docResponse
                = ChargebackUtils.DeserializeResponse<chargebackDocumentUploadResponse>(xmlResponse);
            return docResponse;
        }
        
        private void ConfigureCommunication()
        {
            _communication.SetHost(Config.Get("host"));
            string encoded = ChargebackUtils.Encode64(
                Config.Get("username") + ":" + Config.Get("password"), "utf-8");
            _communication.AddToHeader("Authorization", "Basic " + encoded);
            _communication.SetProxy(Config.Get("proxyHost"), int.Parse(Config.Get("proxyPort")));
            _communication.SetContentType(null);
        }
        
        private void ConfigureCommunicationForUpload(string filePath)
        {           
            ConfigureCommunication();
            _communication.SetContentType(ChargebackUtils.GetMimeMapping(Path.GetFileName(filePath)));
        }
        
        private ChargebackWebException ChargebackDocumentWebException(WebException we, string action)
        {
            var webErrorResponse = (HttpWebResponse) we.Response;
            var httpStatusCode = (int) webErrorResponse.StatusCode;
            var rawResponse = ChargebackUtils.GetResponseXml(webErrorResponse);
            if (!webErrorResponse.ContentType.Contains("application/com.vantivcnp.services-v2+xml"))
            {
                return new ChargebackWebException(string.Format("Document {0} Failed - HTTP {1} Error.", action, httpStatusCode), 
                    httpStatusCode, rawResponse);
            }
            if (bool.Parse(Config.Get("printXml")))
            {
                Console.WriteLine(rawResponse);
            }
            var errorResponse = ChargebackUtils.DeserializeResponse<errorResponse>(rawResponse);
            var errorMessages = errorResponse.errors;
            return new ChargebackWebException(string.Format("Document{0} Failed - HTTP {1} Error", action, httpStatusCode), 
                httpStatusCode, rawResponse, errorMessages);
        }
        
    }
    
    public partial class chargebackDocumentUploadResponse
    {
        // Additional implementation for chargebackDocumentUploadResponse class
        // should be written here.
    }
}