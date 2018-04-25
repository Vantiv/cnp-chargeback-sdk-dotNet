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

        public chargebackDocumentUploadResponse UploadDocument(long caseId, string filePath)
        {
            List<byte> fileBytes = File.ReadAllBytes(filePath).ToList();
            string documentId = Path.GetFileName(filePath);
            try
            {
                configureCommunicationForUpload();
                var responseTuple = communication.Post(
                    SERVICE_ROUTE + "/upload/" + caseId + "/" + documentId, fileBytes);
                return handleResponse(responseTuple);
            }
            catch (WebException we)
            {
                HttpWebResponse errorResponse = (HttpWebResponse) we.Response;
                string errString = ChargebackUtils.ListErrors(errorResponse);
                throw new ChargebackException(
                    string.Format("Upload Failed - HTTP {0} Error" + we, (int) errorResponse.StatusCode) + errString);
            }
        }

        public chargebackDocumentUploadResponse ReplaceDocument(long caseId, string documentId, string filePath)
        {
            List<byte> fileBytes = File.ReadAllBytes(filePath).ToList();
            try
            {
                configureCommunicationForUpload();
                
                var responseTuple = communication.Put(
                    SERVICE_ROUTE + "/replace/" + caseId + "/" + documentId, fileBytes);
                return handleResponse(responseTuple);
            }
            catch (WebException we)
            {
                HttpWebResponse errorResponse = (HttpWebResponse) we.Response;
                string errString = ChargebackUtils.ListErrors(errorResponse);
                throw new ChargebackException(
                    string.Format("Replace Failed - HTTP {0} Error" + we, (int) errorResponse.StatusCode) + errString);
            }
        }

        public IDocumentResponse RetrieveDocument(long caseId, string documentId)
        {
            IDocumentResponse docResponse = null;
            try
            {
                configureCommunication();
                
                var responseTuple = communication.Get(
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
                    string retrievedFilePath = ChargebackUtils.BytesToFile(responseBytes, filePath);
                    chargebackDocumentReceivedResponse fileReceivedResponse = new chargebackDocumentReceivedResponse();
                    fileReceivedResponse.retrievedFilePath = retrievedFilePath;
                    docResponse = fileReceivedResponse;
                }
                else
                {
                    return handleResponse(responseTuple);
                }
            }
            catch (WebException we)
            {
                HttpWebResponse errorResponse = (HttpWebResponse) we.Response;
                string errString = ChargebackUtils.ListErrors(errorResponse);
                throw new ChargebackException(
                    string.Format("Retrieve Failed - HTTP {0} Error" + we, (int) errorResponse.StatusCode) + errString);
            }
            return docResponse;
        }

        public chargebackDocumentUploadResponse DeleteDocument(long caseId, string documentId)
        {
            try
            {
                configureCommunication();
                
                var responseTuple = communication.Delete(
                    string.Format(SERVICE_ROUTE+"/remove/{0}/{1}", caseId, documentId));
                return handleResponse(responseTuple);
                              
            }
            catch (WebException we)
            {
                HttpWebResponse errorResponse = (HttpWebResponse) we.Response;
                string errString = ChargebackUtils.ListErrors(errorResponse);
                throw new ChargebackException(
                    string.Format("Delete Failed - HTTP {0} Error" + we, (int) errorResponse.StatusCode) + errString);
            }
        }

        public chargebackDocumentUploadResponse ListDocuments(long caseId)
        {
            try
            {
                configureCommunication();
                
                var responseTuple = communication.Get(
                    SERVICE_ROUTE + "/list/" + caseId);
                return handleResponse(responseTuple);

            }
            catch (WebException we)
            {
                HttpWebResponse errorResponse = (HttpWebResponse) we.Response;
                string errString = ChargebackUtils.ListErrors(errorResponse);
                throw new ChargebackException(
                    string.Format("List documents Failed - HTTP {0} Error" + we, (int) errorResponse.StatusCode) + errString);
            }
        }
        
        private chargebackDocumentUploadResponse handleResponse(ArrayList responseTuple)
        {
            var contentType = (string) responseTuple[0];
            var responseBytes = (List<byte>) responseTuple[1];
            if (contentType.Contains("application/com.vantivcnp.services-v2+xml"))
            {
                string xmlResponse = ChargebackUtils.BytesToString(responseBytes);
                if (Boolean.Parse(config.getConfig("printXml")))
                {
                    Console.WriteLine(xmlResponse);
                }
                chargebackDocumentUploadResponse docResponse
                    = ChargebackUtils.DeserializeResponse<chargebackDocumentUploadResponse>(xmlResponse);
                return docResponse;
            }
            string stringResponse = ChargebackUtils.BytesToString(responseBytes);
            throw new ChargebackException(
                string.Format("Unexpected returned Content-Type: {0}. Call Vantiv immediately!" +
                              "\nTrying to read the response as raw text:" +
                              "\n{1}", contentType, stringResponse));
        }
        
        private void configureCommunication()
        {
            communication.SetHost(config.getConfig("host"));
            string encoded = ChargebackUtils.Encode64(
                config.getConfig("username") + ":" + config.getConfig("password"), "utf-8");
            communication.AddToHeader("Authorization", "Basic " + encoded);
            communication.SetProxy(config.getConfig("proxyHost"), int.Parse(config.getConfig("proxyPort")));
            communication.SetContentType(null);
        }
        
        private void configureCommunicationForUpload()
        {           
            configureCommunication();
            communication.SetContentType("image/tiff");
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