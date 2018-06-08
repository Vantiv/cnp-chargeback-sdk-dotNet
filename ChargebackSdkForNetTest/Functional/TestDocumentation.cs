using System;
using System.Collections.Generic;
using System.IO;
using ChargebackSdkForNet;
using ChargebackSdkForNet.Properties;
using NUnit.Framework;

namespace ChargebackSdkForNetTest.Functional
{
    [TestFixture]
    public class TestDocumentation
    {
        private ChargebackDocumentationRequest _docRequest;

        [SetUp]
        public void SetUp()
        {
            var configDict = new Dictionary<string, string>();
            configDict["username"] = "dotnet";
            configDict["password"] = "dotnet";
            configDict["merchantId"] = "101";
            configDict["host"] = "https://www.testvantivcnp.com/spring";
            configDict["printXml"] = "true";
            configDict["neuterXml"] = "false";
            configDict["proxyHost"] = "websenseproxy";
            configDict["proxyPort"] = "8080";
            Configuration config = new Configuration(configDict);

            _docRequest = new ChargebackDocumentationRequest {Config = config};
        }
        
        [Test]
        public void TestRetrieveDocument()
        {
            var docResponse = _docRequest.RetrieveDocument(1000, "doc.tiff");
            Assert.NotNull(docResponse);
            Assert.Less(0, docResponse.Count);
        }
        
        [Test]
        [ExpectedException(typeof(ChargebackDocumentException))]
        public void TestRetrieveDocument_DocumentNotFound_009()
        {
            _docRequest.RetrieveDocument(10009, "testDoc.tiff");
        }
        
        [Test]
        public void TestListDocument()
        {
            chargebackDocumentUploadResponse docResponse = _docRequest.ListDocuments(1000);
            Assert.NotNull(docResponse);
            Assert.AreEqual("000", docResponse.responseCode);
            Assert.AreEqual("Success".ToLower(), docResponse.responseMessage.ToLower());
        }
        
        [Test]
        public void TestDeleteDocument()
        {
            chargebackDocumentUploadResponse docResponse = _docRequest.DeleteDocument(1000, "logo.tiff");
            Assert.NotNull(docResponse);
            Assert.AreEqual("000", docResponse.responseCode);
            Assert.AreEqual("Success".ToLower(), docResponse.responseMessage.ToLower());
        }

        [Test]
        public void TestUploadDocument()
        {
            const string documentId = "uploadTest.tiff";
            var tiffFilePath = Path.Combine(Directory.GetCurrentDirectory(), documentId);
            var writer = new StreamWriter(File.Create(tiffFilePath));
            writer.WriteLine("Prototype a file.");
            writer.Close();

            try
            {
                var docResponse = _docRequest.UploadDocument(1000, tiffFilePath);
                Assert.NotNull(docResponse);
                Assert.AreEqual("000", docResponse.responseCode);
                Assert.AreEqual("Success".ToLower(), docResponse.responseMessage.ToLower());
            }

            catch (Exception e)
            {
                Assert.Fail("Upload Test failed" + e);
            }
            finally
            {
                File.Delete(tiffFilePath);
            }
        }

        [Test]
        public void TestReplaceDocument()
        {
            const string documentId = "uploadTest.tiff";
            var tiffFilePath = Path.Combine(Directory.GetCurrentDirectory(), documentId);
            var writer = new StreamWriter(File.Create(tiffFilePath));
            writer.WriteLine("Prototype a file.");
            writer.Close();

            try
            {
                var docResponse = _docRequest.ReplaceDocument(1000, documentId, tiffFilePath);
                Assert.NotNull(docResponse);
                Assert.AreEqual("000", docResponse.responseCode);
                Assert.AreEqual("Success".ToLower(), docResponse.responseMessage.ToLower());
            }

            catch (Exception e)
            {
                Assert.Fail("Upload Test failed" + e);
            }
            finally
            {
                File.Delete(tiffFilePath);
            }
        }
    }
}
