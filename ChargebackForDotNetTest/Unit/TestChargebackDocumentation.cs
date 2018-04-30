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
            var headTemplate = 
                "<?xml version=\"1.0\" encoding=\"UTF-8\" standalone=\"yes\"?>" +
                "<chargebackDocumentUploadResponse xmlns=\"http://www.vantivcnp.com/chargebacks\">" +
                "    <merchantId>101</merchantId>" +
                "    <caseId>{0}</caseId>";
            var footTemplate =
                "    <responseCode>{0}</responseCode>" +
                "    <responseMessage>{1}</responseMessage>" +
                "</chargebackDocumentUploadResponse>";
            var head = string.Format(headTemplate, caseId);
            var foot = string.Format(footTemplate, responseCode, responseMessage);
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
        
        private string GenerateErrorXmlResponse(string[] errorMessages)
        {
            const string head = "<?xml version=\"1.0\" encoding=\"UTF-8\" standalone=\"yes\"?>" +
                                "<errorResponse>" +
                                "<errors>";
            const string foot = "</errors>" +
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
            const string expectedFileContent = "To test document retrieval";
            var expectedRetrievedFileContent = ChargebackUtils.StringToBytes(expectedFileContent);
            var expectedResponseContent = new ResponseContent(
                "image/tiff", expectedRetrievedFileContent);
            var commMock = new Mock<Communication>();
            commMock.Setup(c => c.Get(string.Format("/services/chargebacks/retrieve/{0}/{1}", caseId, documentId)))
                .Returns(expectedResponseContent);
            var docRequest = new ChargebackDocumentationRequest(commMock.Object);
            var docResponse = docRequest.RetrieveDocument(caseId, documentId);
            Assert.Less(0, docResponse.Count);
        }
        
        
        
        [TestCase(1009, "test1.tif", new[] {"test1.tif"}, "009", "Document Not Found")]
        [TestCase(1003, "test2.tif", new[] {"test2.tif"}, "003", "Case Not Found")]
        [ExpectedException(typeof(ChargebackDocumentException))]
        public void TestRetrieveDocumentFailure(long caseId, string documentId, string[] expectedDocumentIds, 
            string expectedResponseCode, string expectedResponseMessage)
        {
            var expectedXmlResponse = generateXmlResponse(caseId, expectedDocumentIds,
                expectedResponseCode, expectedResponseMessage);
            var expectedResponseContent = new ResponseContent(
                "application/com.vantivcnp.services-v2+xml",
                ChargebackUtils.StringToBytes(expectedXmlResponse));
            var commMock = new Mock<Communication>();
            commMock.Setup(c => c.Get(string.Format("/services/chargebacks/retrieve/{0}/{1}", caseId, documentId)))
                .Returns(expectedResponseContent);
            var docRequest = new ChargebackDocumentationRequest(commMock.Object);
            docRequest.RetrieveDocument(caseId, documentId);
        }
        
        [TestCase(1000, new[] {"test1Doc1.tif", "test1Doc2.tif"}, "000", "Success")]
        [TestCase(1009, null, "009", "Document Not Found")]
        [TestCase(1003, null, "003", "Case Not Found")]
        public void TestListDocument(long caseId, string[] expectedDocumentIds, 
            string expectedResponseCode, string expectedResponseMessage)
        {
            var expectedXmlResponse = generateXmlResponse(caseId, expectedDocumentIds,
                expectedResponseCode, expectedResponseMessage);
            var expectedResponseContent
                = new ResponseContent(
                    "application/com.vantivcnp.services-v2+xml",
                    ChargebackUtils.StringToBytes(expectedXmlResponse));
            var commMock = new Mock<Communication>();
            commMock.Setup(c => c.Get(string.Format("/services/chargebacks/list/{0}", caseId)))
                .Returns(expectedResponseContent);
            var docRequest = new ChargebackDocumentationRequest(commMock.Object);
            var docUploadResponse = docRequest.ListDocuments(caseId);            
            Assert.AreEqual(caseId, docUploadResponse.caseId);
            Assert.AreEqual(expectedDocumentIds, docUploadResponse.documentId);
            Assert.AreEqual(expectedResponseCode, docUploadResponse.responseCode);
            Assert.AreEqual(expectedResponseMessage, docUploadResponse.responseMessage);
        }
        
        [TestCase(1000, "test1.tif", new[] {"test1.tif"}, "000", "Success")]
        [TestCase(1009, "test2.tif", new[] {"test2.tif"}, "009", "Document Not Found")]
        [TestCase(1003, "test3.tif", new[] {"test3.tif"}, "003", "Case Not Found")]
        public void TestDeleteDocument(long caseId, string documentId, string[] expectedDocumentIds, 
            string expectedResponseCode, string expectedResponseMessage)
        {
            var expectedXmlResponse = generateXmlResponse(caseId, expectedDocumentIds,
                expectedResponseCode, expectedResponseMessage);
            var expectedResponseContent
                = new ResponseContent("application/com.vantivcnp.services-v2+xml",
                    ChargebackUtils.StringToBytes(expectedXmlResponse));
            var commMock = new Mock<Communication>();
            commMock.Setup(c => c.Delete(string.Format("/services/chargebacks/remove/{0}/{1}", caseId, documentId)))
                .Returns(expectedResponseContent);
            var docRequest = new ChargebackDocumentationRequest(commMock.Object);
            var docUploadResponse = docRequest.DeleteDocument(caseId, documentId);            
            Assert.AreEqual(caseId, docUploadResponse.caseId);
            Assert.AreEqual(expectedDocumentIds, docUploadResponse.documentId);
            Assert.AreEqual(expectedResponseCode, docUploadResponse.responseCode);
            Assert.AreEqual(expectedResponseMessage, docUploadResponse.responseMessage);
        }
        
        [TestCase(1000, "test1.tif", new[] {"test1.tif"}, "000", "Success")]
        [TestCase(1003, "test3.tif", new[] {"test3.tif"}, "003", "Case Not Found")]
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
            var expectedResponseContent = new ResponseContent(
                "application/com.vantivcnp.services-v2+xml",
                ChargebackUtils.StringToBytes(expectedXmlResponse));
            var commMock = new Mock<Communication>();
            commMock.Setup(c => c.Post(string.Format("/services/chargebacks/upload/{0}/{1}", caseId, documentId), 
                    sendingBytes)).Returns(expectedResponseContent);
            var docRequest = new ChargebackDocumentationRequest(commMock.Object);
            var docUploadResponse = docRequest.UploadDocument(caseId, tiffFilePath);            
            Assert.AreEqual(caseId, docUploadResponse.caseId);
            Assert.AreEqual(expectedDocumentIds, docUploadResponse.documentId);
            Assert.AreEqual(expectedResponseCode, docUploadResponse.responseCode);
            Assert.AreEqual(expectedResponseMessage, docUploadResponse.responseMessage);
        }
        
        
        [TestCase(1000, "test1.tif", new[] {"test1.tif"}, "000", "Success")]
        [TestCase(1009, "test2.tif", new[] {"test2.tif"}, "009", "Document Not Found")]
        [TestCase(1003, "test3.tif", new[] {"test3.tif"}, "003", "Case Not Found")]
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
            var expectedResponseContent
                = new ResponseContent(
                    "application/com.vantivcnp.services-v2+xml",
                    ChargebackUtils.StringToBytes(expectedXmlResponse));
            var commMock = new Mock<Communication>();
            commMock.Setup(c => c.Put(string.Format("/services/chargebacks/replace/{0}/{1}", caseId, documentId), 
                sendingBytes)).Returns(expectedResponseContent);
            var docRequest = new ChargebackDocumentationRequest(commMock.Object);
            var docUploadResponse = docRequest.ReplaceDocument(caseId, documentId, tiffFilePath);            
            Assert.AreEqual(caseId, docUploadResponse.caseId);
            Assert.AreEqual(expectedDocumentIds, docUploadResponse.documentId);
            Assert.AreEqual(expectedResponseCode, docUploadResponse.responseCode);
            Assert.AreEqual(expectedResponseMessage, docUploadResponse.responseMessage);
        }

        [TearDown]
        public void RemoveTestFiles()
        {
            var fileNames = Directory.GetFiles(Directory.GetCurrentDirectory());
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