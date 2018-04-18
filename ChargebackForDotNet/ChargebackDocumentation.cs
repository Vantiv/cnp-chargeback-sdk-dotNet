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
                    return null;
                }
                return this.configurationField;
            }
            set { this.configurationField = value; }
        }

        public chargebackDocumentUploadResponse uploadDocument(long caseId, string filePath)
        {
            int maxSizeMB = 2;
            long maxSizeByte = 2 * 1024 * 1024;
            List<byte> fileBytes = File.ReadAllBytes(filePath).ToList();
            if (fileBytes.Count > maxSizeByte)
            {
                throw new ChargebackException("File size should not exceed 2 MBs.");
            }
            List<byte> responseBytes = new List<byte>();
            string documentId = Path.GetFileName(filePath);
            try
            {
                string contentType = Communication.post(
                    config, "/services/chargebacks/upload/" + caseId + "/" + documentId, fileBytes, responseBytes);
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
                string contentType = Communication.get(
                    config, String.Format("/services/chargebacks/retrieve/{0}/{1}", caseId, documentId), bytes);
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

        public chargebackDocumentUploadResponse replaceDocument(long caseId, long documentId, string filePath)
        {
            List<byte> fileBytes = File.ReadAllBytes(filePath).ToList();
            List<byte> responseBytes = new List<byte>();
            try
            {
                string contentType = Communication.put(
                    config, "/services/chargebacks/replace/" + caseId + "/" + documentId, fileBytes, responseBytes);
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

        public chargebackDocumentUploadResponse deleteDocument(long caseId, string documentId)
        {
            try
            {
                List<byte> bytes = new List<byte>();
                string contentType = Communication.delete(config, string.Format("/services/chargebacks/remove/{0}/{1}", caseId, documentId), bytes);
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
                string contentType = Communication.get(config, "/services/chargebacks/list/" + caseId, bytes);
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
    }
}