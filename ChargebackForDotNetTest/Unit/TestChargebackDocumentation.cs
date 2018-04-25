using System;
using System.Collections;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using ChargebackForDotNet;
using Moq;
using NUnit.Framework;

namespace ChargebackForDotNetTest.Unit
{
    [TestFixture]
    public class TestChargebackDocumentation
    {
        [SetUp]
        public void SetUp()
        {
            
        }
        
        private string generateXmlResponse(long caseId, string[] documentIds, 
            string responseCode, string responseMessage)
        {
            string headTemplate = 
                "<?xml version=\"1.0\" encoding=\"UTF-8\" standalone=\"yes\"?>" +
                "<chargebackDocumentUploadResponse xmlns=\"http://www.vantivcnp.com/chargebacks\">" +
                "    <merchantId>101</merchantId>" +
                "    <caseId>{0}</caseId>";
            string footTemplate =
                "    <responseCode>{0}</responseCode>" +
                "    <responseMessage>{1}</responseMessage>" +
                "</chargebackDocumentUploadResponse>";
            string head = string.Format(headTemplate, caseId);
            string foot = string.Format(footTemplate, responseCode, responseMessage);
            var xmlResponse = new StringBuilder(head);

            if (documentIds != null)
            {
                foreach (var documentId in documentIds)
                {
                    xmlResponse.Append("<documentId>" + documentId + "</documentId>");
                } 
            }           
            xmlResponse.Append(foot);

            return xmlResponse.ToString();
        }
        
        private string generateErrorXmlResponse(string[] errorMessages)
        {
            string head =
                "<?xml version=\"1.0\" encoding=\"UTF-8\" standalone=\"yes\"?>" +
                "<errorResponse>" +
                "<errors>";
            string foot =
                "</errors>" +
                "</errorResponse>";
            var xmlResponse = new StringBuilder(head);
            foreach (var error in errorMessages)
            {
                xmlResponse.Append("<error>" + error + "</error>");
            }

            xmlResponse.Append(foot);
            return xmlResponse.ToString();
        }


        [TestCase(1000, "test1.tif")]
        public void TestRetrieveDocument(long caseId, string documentId)
        {
            string expectedFileContent = "To test document retrieval";
            var expectedRetrievedFileContent = ChargebackUtils.StringToBytes(expectedFileContent);
            var expectedResponseTuple = new ArrayList();
            expectedResponseTuple.Add("image/tiff");
            expectedResponseTuple.Add(expectedRetrievedFileContent);
            Mock<Communication> commMock = new Mock<Communication>();
            commMock.Setup(c => c.Get(string.Format("/services/chargebacks/retrieve/{0}/{1}", caseId, documentId)))
                .Returns(expectedResponseTuple);
            var docRequest = new ChargebackDocumentationRequest(commMock.Object);
            docRequest.config.setConfigValue("downloadDirectory", Directory.GetCurrentDirectory());
            var docResponse = docRequest.RetrieveDocument(caseId, documentId);
            
            Assert.True(docResponse is chargebackDocumentReceivedResponse);
            var docReceivedResponse = (chargebackDocumentReceivedResponse) docResponse;
            Assert.NotNull(docReceivedResponse.retrievedFilePath);
            var retrievedFilepath = docReceivedResponse.retrievedFilePath;
            Assert.AreEqual(documentId, Path.GetFileName(docReceivedResponse.retrievedFilePath));
            Assert.True(File.Exists(retrievedFilepath));
            var retrievedFilecontent = File.ReadAllBytes(retrievedFilepath).ToList();
            Assert.AreEqual(expectedRetrievedFileContent, retrievedFilecontent);
            File.Delete(retrievedFilepath);
        }
        
        
        
        [TestCase(1009, "test1.tif", new string[] {"test1.tif"}, "009", "Document Not Found")]
        [TestCase(1003, "test2.tif", new string[] {"test2.tif"}, "003", "Case Not Found")]
        public void TestRetrieveDocumentFailure(long caseId, string documentId, string[] expectedDocumentIds, 
            string expectedResponseCode, string expectedResponseMessage)
        {
            var expectedXmlResponse = generateXmlResponse(caseId, expectedDocumentIds,
                expectedResponseCode, expectedResponseMessage);
            var expectedResponseTuple = new ArrayList();
            expectedResponseTuple.Add("application/com.vantivcnp.services-v2+xml");
            expectedResponseTuple.Add(ChargebackUtils.StringToBytes(expectedXmlResponse));
            Mock<Communication> commMock = new Mock<Communication>();
            commMock.Setup(c => c.Get(string.Format("/services/chargebacks/retrieve/{0}/{1}", caseId, documentId)))
                .Returns(expectedResponseTuple);
            var docRequest = new ChargebackDocumentationRequest(commMock.Object);
            var docResponse = docRequest.RetrieveDocument(caseId, documentId);
            Assert.True(docResponse is chargebackDocumentUploadResponse);
            var docUploadResponse = (chargebackDocumentUploadResponse) docResponse;
            Assert.AreEqual(caseId, docUploadResponse.caseId);
            Assert.AreEqual(expectedDocumentIds[0], docUploadResponse.documentId[0]);
            Assert.AreEqual(expectedResponseCode, docUploadResponse.responseCode);
            Assert.AreEqual(expectedResponseMessage, docUploadResponse.responseMessage);
        }
        
        [TestCase(1000, new string[] {"test1Doc1.tif", "test1Doc2.tif"}, "000", "Success")]
        [TestCase(1009, null, "009", "Document Not Found")]
        [TestCase(1003, null, "003", "Case Not Found")]
        public void TestListDocument(long caseId, string[] expectedDocumentIds, 
            string expectedResponseCode, string expectedResponseMessage)
        {
            var expectedXmlResponse = generateXmlResponse(caseId, expectedDocumentIds,
                expectedResponseCode, expectedResponseMessage);
            var expectedResponseTuple = new ArrayList();
            expectedResponseTuple.Add("application/com.vantivcnp.services-v2+xml");
            expectedResponseTuple.Add(ChargebackUtils.StringToBytes(expectedXmlResponse));
            Mock<Communication> commMock = new Mock<Communication>();
            commMock.Setup(c => c.Get(string.Format("/services/chargebacks/list/{0}", caseId)))
                .Returns(expectedResponseTuple);
            var docRequest = new ChargebackDocumentationRequest(commMock.Object);
            var docUploadResponse = docRequest.ListDocuments(caseId);            
            Assert.AreEqual(caseId, docUploadResponse.caseId);
            Assert.AreEqual(expectedDocumentIds, docUploadResponse.documentId);
            Assert.AreEqual(expectedResponseCode, docUploadResponse.responseCode);
            Assert.AreEqual(expectedResponseMessage, docUploadResponse.responseMessage);
        }
        
        [TestCase(1000, "test1.tif", new string[] {"test1.tif"}, "000", "Success")]
        [TestCase(1009, "test2.tif", new string[] {"test2.tif"}, "009", "Document Not Found")]
        [TestCase(1003, "test3.tif", new string[] {"test3.tif"}, "003", "Case Not Found")]
        public void TestDeleteDocument(long caseId, string documentId, string[] expectedDocumentIds, 
            string expectedResponseCode, string expectedResponseMessage)
        {
            var expectedXmlResponse = generateXmlResponse(caseId, expectedDocumentIds,
                expectedResponseCode, expectedResponseMessage);
            var expectedResponseTuple = new ArrayList();
            expectedResponseTuple.Add("application/com.vantivcnp.services-v2+xml");
            expectedResponseTuple.Add(ChargebackUtils.StringToBytes(expectedXmlResponse));
            Mock<Communication> commMock = new Mock<Communication>();
            commMock.Setup(c => c.Delete(string.Format("/services/chargebacks/remove/{0}/{1}", caseId, documentId)))
                .Returns(expectedResponseTuple);
            var docRequest = new ChargebackDocumentationRequest(commMock.Object);
            var docUploadResponse = docRequest.DeleteDocument(caseId, documentId);            
            Assert.AreEqual(caseId, docUploadResponse.caseId);
            Assert.AreEqual(expectedDocumentIds, docUploadResponse.documentId);
            Assert.AreEqual(expectedResponseCode, docUploadResponse.responseCode);
            Assert.AreEqual(expectedResponseMessage, docUploadResponse.responseMessage);
        }
        
        [TestCase(1000, "test1.tif", new string[] {"test1.tif"}, "000", "Success")]
        [TestCase(1003, "test3.tif", new string[] {"test3.tif"}, "003", "Case Not Found")]
        public void TestUploadDocument(long caseId, string documentId, string[] expectedDocumentIds, 
            string expectedResponseCode, string expectedResponseMessage)
        {
            var tiffFilePath = Path.Combine(Directory.GetCurrentDirectory(), documentId);
            var writer = new StreamWriter(File.Create(tiffFilePath));
            writer.WriteLine("Prototype an upload test file.");
            writer.Close();
            var sendingBytes = File.ReadAllBytes(tiffFilePath).ToList();
            var expectedXmlResponse = generateXmlResponse(caseId, expectedDocumentIds,
                expectedResponseCode, expectedResponseMessage);
            var expectedResponseTuple = new ArrayList();
            expectedResponseTuple.Add("application/com.vantivcnp.services-v2+xml");
            expectedResponseTuple.Add(ChargebackUtils.StringToBytes(expectedXmlResponse));
            Mock<Communication> commMock = new Mock<Communication>();
            commMock.Setup(c => c.Post(string.Format("/services/chargebacks/upload/{0}/{1}", caseId, documentId), 
                    sendingBytes)).Returns(expectedResponseTuple);
            var docRequest = new ChargebackDocumentationRequest(commMock.Object);
            var docUploadResponse = docRequest.UploadDocument(caseId, tiffFilePath);            
            Assert.AreEqual(caseId, docUploadResponse.caseId);
            Assert.AreEqual(expectedDocumentIds, docUploadResponse.documentId);
            Assert.AreEqual(expectedResponseCode, docUploadResponse.responseCode);
            Assert.AreEqual(expectedResponseMessage, docUploadResponse.responseMessage);
        }
        
        
        [TestCase(1000, "test1.tif", new string[] {"test1.tif"}, "000", "Success")]
        [TestCase(1009, "test2.tif", new string[] {"test2.tif"}, "009", "Document Not Found")]
        [TestCase(1003, "test3.tif", new string[] {"test3.tif"}, "003", "Case Not Found")]
        public void TestReplaceDocument(long caseId, string documentId, string[] expectedDocumentIds, 
            string expectedResponseCode, string expectedResponseMessage)
        {
            var tiffFilePath = Path.Combine(Directory.GetCurrentDirectory(), documentId);
            var writer = new StreamWriter(File.Create(tiffFilePath));
            writer.WriteLine("Prototype an upload test file.");
            writer.Close();
            var sendingBytes = File.ReadAllBytes(tiffFilePath).ToList();
            var expectedXmlResponse = generateXmlResponse(caseId, expectedDocumentIds,
                expectedResponseCode, expectedResponseMessage);
            var expectedResponseTuple = new ArrayList();
            expectedResponseTuple.Add("application/com.vantivcnp.services-v2+xml");
            expectedResponseTuple.Add(ChargebackUtils.StringToBytes(expectedXmlResponse));
            Mock<Communication> commMock = new Mock<Communication>();
            commMock.Setup(c => c.Put(string.Format("/services/chargebacks/replace/{0}/{1}", caseId, documentId), 
                sendingBytes)).Returns(expectedResponseTuple);
            var docRequest = new ChargebackDocumentationRequest(commMock.Object);
            var docUploadResponse = docRequest.ReplaceDocument(caseId, documentId, tiffFilePath);            
            Assert.AreEqual(caseId, docUploadResponse.caseId);
            Assert.AreEqual(expectedDocumentIds, docUploadResponse.documentId);
            Assert.AreEqual(expectedResponseCode, docUploadResponse.responseCode);
            Assert.AreEqual(expectedResponseMessage, docUploadResponse.responseMessage);
        }

        [TearDown]
        public void removeTestFiles()
        {
            string[] fileNames = Directory.GetFiles(Directory.GetCurrentDirectory());
            foreach (var fileName in fileNames)
            {
                if (fileName.Contains(".tif"))
                {
                    File.Delete(fileName);
                }
            }
        }
    }
}