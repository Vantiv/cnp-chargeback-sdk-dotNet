using System;
using System.IO;
using ChargebackForDotNet;
using NUnit.Framework;

namespace ChargebackForDotNetTest.Certification
{
    public class TestCertificationChargebackDocumentationAPI
    {
        
        [Test]
        public void TestCase1()
        {
            string randomFilename = "random.tiff";
            StreamWriter writer = new StreamWriter(File.Create(randomFilename));
            writer.WriteLine("Prototype a file.");
            writer.Close();
            
            ChargebackDocumentationRequest docRequest
                = new ChargebackDocumentationRequest();
            docRequest.config.setConfigValue("host", "https://www.testvantivcnp.com/sandbox/new");
            long caseId = Int32.Parse(docRequest.config.getConfig("merchantId") + "001");
            chargebackDocumentUploadResponse documentUploadResponse
                = docRequest.uploadDocument(caseId, randomFilename);
            Assert.AreEqual(documentUploadResponse.caseId, caseId);
        }
        
        [Test]
        public void TestCase2()
        {
            
        }
        
        [Test]
        public void TestCase3()
        {
            
        }
        
        [Test]
        public void TestCase4()
        {
            
        }
    }
}