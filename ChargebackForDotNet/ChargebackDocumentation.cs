using System;
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

        public ChargebackDocumentationRequest()
        {
        }

        public ChargebackDocumentationRequest(Configuration config)
        {
            this.configurationField = config;
        }

        public chargebackDocumentUploadResponse uploadDocument(long caseId, string filePath)
        {
            List<byte> fileBytes = File.ReadAllBytes(filePath).ToList();
            List<byte> responseBytes = new List<byte>();
            string documentId = Path.GetFileName(filePath);
            try
            {

                Communication c = createUploadCommunication();
                c.setContentLength(fileBytes.Count);
                string contentType = c.post(
                    "/services/chargebacks/upload/" + caseId + "/" + documentId, fileBytes, responseBytes);
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
                    String.Format("Unexpected returned Content-Type: {0}. Call Vantiv immediately!" +
                                  "\nTrying to read the response as raw text:" +
                                  "\n{1}", contentType, stringResponse));
            }
            catch (WebException we)
            {
                HttpWebResponse httpResponse = (HttpWebResponse) we.Response;
                
                throw new ChargebackException("Call Vantiv. HTTP Status Code:" 
                                              + httpResponse.StatusCode
                                              + "\n" + we.Message + "\n" + we.StackTrace);
            }
        }

        public chargebackDocumentUploadResponse replaceDocument(long caseId, string documentId, string filePath)
        {
            List<byte> fileBytes = File.ReadAllBytes(filePath).ToList();
            List<byte> responseBytes = new List<byte>();
            try
            {
                Communication c = createUploadCommunication();
                c.setContentLength(fileBytes.Count);
                string contentType = c.put(
                    "/services/chargebacks/replace/" + caseId + "/" + documentId, fileBytes, responseBytes);
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
                    String.Format("Unexpected returned Content-Type: {0}. Call Vantiv immediately!" +
                                  "\nTrying to read the response as raw text:" +
                                  "\n{1}", contentType, stringResponse));
            }
            catch (WebException we)
            {
                throw new ChargebackException("Call Vantiv. \n" + we.StackTrace);
            }
        }

        public IDocumentResponse retrieveDocument(long caseId, string documentId)
        {
            List<byte> bytes = new List<byte>();
            IDocumentResponse docResponse = null;
            try
            {
                Communication c = createCommunication();
                
                string contentType = c.get(
                    String.Format("/services/chargebacks/retrieve/{0}/{1}", caseId, documentId), bytes);
                if ("image/tiff".Equals(contentType))
                {
                    string filePath = Path.Combine(config.getConfig("downloadDirectory"), documentId);
                    string retrievedFilePath = Utils.bytesToFile(bytes, filePath);
                    chargebackDocumentReceivedResponse fileReceivedResponse = new chargebackDocumentReceivedResponse();
                    fileReceivedResponse.retrievedFilePath = retrievedFilePath;
                    docResponse = fileReceivedResponse;
                }
                else if (contentType.Contains("application/com.vantivcnp.services-v2+xml"))
                {
                    string xmlResponse = Utils.bytesToString(bytes);
                    Console.WriteLine(xmlResponse);
                    docResponse 
                        = Utils.DeserializeResponse<chargebackDocumentUploadResponse>(xmlResponse);
                }
                else
                {
                    string stringResponse = Utils.bytesToString(bytes);
                    throw new ChargebackException(
                        String.Format("Unexpected returned Content-Type: {0}. Call Vantiv immediately!" +
                                      "\nTrying to read the response as raw text:" +
                                      "\n{1}", contentType, stringResponse));
                }
            }
            catch (WebException we)
            {
                throw new ChargebackException("Call Vantiv. \n" + we.StackTrace);
            }
            return docResponse;
        }

        public chargebackDocumentUploadResponse deleteDocument(long caseId, string documentId)
        {
            try
            {
                List<byte> bytes = new List<byte>();

                Communication c = createCommunication();
                
                string contentType = c.delete(string.Format("/services/chargebacks/remove/{0}/{1}", caseId, documentId), bytes);
                if (contentType.Contains("application/com.vantivcnp.services-v2+xml"))
                {
                    string xmlResponse = Utils.bytesToString(bytes);
                    Console.WriteLine(xmlResponse);
                    chargebackDocumentUploadResponse docResponse
                        = Utils.DeserializeResponse<chargebackDocumentUploadResponse>(xmlResponse);
                    return docResponse;
                }
                string stringResponse = Utils.bytesToString(bytes);
                throw new ChargebackException(
                    String.Format("Unexpected returned Content-Type: {0}. Call Vantiv immediately!" +
                                  "\nTrying to read the response as raw text:" +
                                  "\n{1}", contentType, stringResponse));
                              
            }
            catch (WebException we)
            {
                throw new ChargebackException("Call Vantiv. \n" + we.StackTrace);
            }
        }

        public chargebackDocumentUploadResponse listDocuments(long caseId)
        {
            try
            {
                List<byte> bytes = new List<byte>();

                Communication c = createCommunication();
                
                string contentType = c.get("/services/chargebacks/list/" + caseId, bytes);
                if (contentType.Contains("application/com.vantivcnp.services-v2+xml"))
                {
                    string xmlResponse = Utils.bytesToString(bytes);
                    Console.WriteLine(xmlResponse);
                    chargebackDocumentUploadResponse docResponse
                        = Utils.DeserializeResponse<chargebackDocumentUploadResponse>(xmlResponse);
                    return docResponse;
                }
                string stringResponse = Utils.bytesToString(bytes);
                throw new ChargebackException(
                    String.Format("Unexpected returned Content-Type: {0}. Call Vantiv immediately!" +
                                  "\nTrying to read the response as raw text:" +
                                  "\n{1}", contentType, stringResponse));
                              
            }
            catch (WebException we)
            {
                throw new ChargebackException("Call Vantiv. \n" + we.StackTrace);
            }
        }

        
        private Communication createCommunication()
        {
            Communication c= new Communication(config.getConfig("host"));
            string encoded = Utils.encode64(config.getConfig("username") + ":" + config.getConfig("password"), "utf-8");
            c.addToHeader("Authorization", "Basic " + encoded);
            c.setProxy(config.getConfig("proxyHost"), Int32.Parse(config.getConfig("proxyPort")));
            return c;
        }
        
        private Communication createUploadCommunication()
        {
            Communication c = createCommunication();
            c.setContentType("image/tiff");
            return c;
        }
    }
}