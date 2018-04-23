using System;
using System.IO;
using System.Net.Mail;
using System.Security.Cryptography.X509Certificates;
using System.Xml.Serialization;
using ChargebackForDotNet.Properties;

namespace ChargebackForDotNet
{

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
    
    public partial class errorResponse
    {
        // Additional implementation for errorResponse class
        // should be written here.
    }
    
    
   
    
}