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
        private Configuration configurationField;
        private Communication communication;

        private const string SERVICE_ROUTE = "/services/chargebacks";

        public Configuration config
        {
            get { return configurationField ?? (configurationField = new Configuration()); }
            set { configurationField = value; }
        }

        public ChargebackDocumentationRequest()
        {
            communication = new Communication();
        }

        public ChargebackDocumentationRequest(Communication comm)
        {
            communication = comm;
        }

        public ChargebackDocumentationRequest(Configuration config)
        {
            this.configurationField = config;
            communication = new Communication();
        }

        public chargebackDocumentUploadResponse uploadDocument(long caseId, string filePath)
        {
            List<byte> fileBytes = File.ReadAllBytes(filePath).ToList();
            string documentId = Path.GetFileName(filePath);
            try
            {
                SetUpCommunicationForUpload();
                var responseTuple = communication.post(
                    SERVICE_ROUTE + "/upload/" + caseId + "/" + documentId, fileBytes);
                return HandleResponse(responseTuple);
            }
            catch (WebException we)
            {
                HttpWebResponse errorResponse = (HttpWebResponse) we.Response;
                throw new ChargebackException(
                    string.Format("Upload Failed - HTTP {0} Error" + we, (int) errorResponse.StatusCode) , errorResponse);
            }
        }

        public chargebackDocumentUploadResponse replaceDocument(long caseId, string documentId, string filePath)
        {
            List<byte> fileBytes = File.ReadAllBytes(filePath).ToList();
            try
            {
                SetUpCommunicationForUpload();
                
                var responseTuple = communication.put(
                    SERVICE_ROUTE + "/replace/" + caseId + "/" + documentId, fileBytes);
                return HandleResponse(responseTuple);
            }
            catch (WebException we)
            {
                HttpWebResponse errorResponse = (HttpWebResponse) we.Response;
                throw new ChargebackException(
                    string.Format("Replace Failed - HTTP {0} Error" + we, (int) errorResponse.StatusCode) , errorResponse);
            }
        }

        public IDocumentResponse retrieveDocument(long caseId, string documentId)
        {
            IDocumentResponse docResponse = null;
            try
            {
                SetUpCommunication();
                
                var responseTuple = communication.get(
                    string.Format(SERVICE_ROUTE+"/retrieve/{0}/{1}", caseId, documentId));
                var contentType = (string) responseTuple[0];
                var responseBytes = (List<byte>) responseTuple[1];
                if ("image/tiff".Equals(contentType))
                {
                    var downloadDiectory = config.getConfig("downloadDirectory");
                    string filePath = Path.Combine(downloadDiectory, documentId);
                    if (!Directory.Exists(downloadDiectory))
                    {
                        Directory.CreateDirectory(downloadDiectory);
                    }
                    string retrievedFilePath = Utils.bytesToFile(responseBytes, filePath);
                    chargebackDocumentReceivedResponse fileReceivedResponse = new chargebackDocumentReceivedResponse();
                    fileReceivedResponse.retrievedFilePath = retrievedFilePath;
                    docResponse = fileReceivedResponse;
                }
                else
                {
                    return HandleResponse(responseTuple);
                }
            }
            catch (WebException we)
            {
                HttpWebResponse errorResponse = (HttpWebResponse) we.Response;
                throw new ChargebackException(
                    string.Format("Retrieve Failed - HTTP {0} Error" + we, (int) errorResponse.StatusCode) , errorResponse);
            }
            return docResponse;
        }

        public chargebackDocumentUploadResponse deleteDocument(long caseId, string documentId)
        {
            try
            {
                SetUpCommunication();
                
                var responseTuple = communication.delete(
                    string.Format(SERVICE_ROUTE+"/remove/{0}/{1}", caseId, documentId));
                return HandleResponse(responseTuple);
                              
            }
            catch (WebException we)
            {
                HttpWebResponse errorResponse = (HttpWebResponse) we.Response;
                throw new ChargebackException(
                    string.Format("Delete Failed - HTTP {0} Error" + we, (int) errorResponse.StatusCode) , errorResponse);
            }
        }

        public chargebackDocumentUploadResponse listDocuments(long caseId)
        {
            try
            {
                SetUpCommunication();
                
                var responseTuple = communication.get(
                    SERVICE_ROUTE + "/list/" + caseId);
                return HandleResponse(responseTuple);

            }
            catch (WebException we)
            {
                HttpWebResponse errorResponse = (HttpWebResponse) we.Response;
                throw new ChargebackException(
                    string.Format("List documents Failed - HTTP {0} Error" + we, (int) errorResponse.StatusCode) , errorResponse);
            }
        }

        
        private chargebackDocumentUploadResponse HandleResponse(ArrayList responseTuple)
        {
            var contentType = (string) responseTuple[0];
            var responseBytes = (List<byte>) responseTuple[1];
            if (contentType.Contains("application/com.vantivcnp.services-v2+xml"))
            {
                string xmlResponse = Utils.bytesToString(responseBytes);
                Console.WriteLine(xmlResponse);
                chargebackDocumentUploadResponse docResponse
                    = Utils.DeserializeResponse<chargebackDocumentUploadResponse>(xmlResponse);
                return docResponse;
            }
            string stringResponse = Utils.bytesToString(responseBytes);
            throw new ChargebackException(
                string.Format("Unexpected returned Content-Type: {0}. Call Vantiv immediately!" +
                              "\nTrying to read the response as raw text:" +
                              "\n{1}", contentType, stringResponse));
        }

        
        private void SetUpCommunication()
        {
            communication.setHost(config.getConfig("host"));
            string encoded = Utils.encode64(
                config.getConfig("username") + ":" + config.getConfig("password"), "utf-8");
            communication.addToHeader("Authorization", "Basic " + encoded);
            communication.setProxy(config.getConfig("proxyHost"), int.Parse(config.getConfig("proxyPort")));
            communication.setContentType(null);
        }
        
        private void SetUpCommunicationForUpload()
        {           
            SetUpCommunication();
            communication.setContentType("image/tiff");
        }
        
    }
    
    public interface IDocumentResponse
    {
        // The response of retrieving documents can return
        // either a file or an xml string for error. Thus, returning an
        // interface works for both cases.
    }

    public class chargebackDocumentReceivedResponse : IDocumentResponse
    {
        public string retrievedFilePath { get; set; }
    }
    
    public partial class chargebackDocumentUploadResponse : IDocumentResponse
    {
        // Additional implementation for chargebackDocumentUploadResponse class
        // should be written here.
    }
}