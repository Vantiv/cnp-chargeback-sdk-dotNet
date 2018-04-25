using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using ChargebackForDotNet;
using ChargebackForDotNet.Properties;
using NUnit.Framework;

namespace ChargebackForDotNetTest.Functional
{
    [TestFixture]
    public class TestDocumentation
    {
        private ChargebackDocumentationRequest docRequest;

        [SetUp]
        public void SetUp()
        {
            Dictionary<string, string> configDict = new Dictionary<string, string>();
            configDict["username"] = "dotnet";
            configDict["password"] = "dotnet";
            configDict["merchantId"] = "101";
            configDict["host"] = "https://www.testvantivcnp.com/sandbox/new";
            configDict["downloadDirectory"] = "C:\\Vantiv\\chargebacks";
            configDict["printXml"] = "true";
            configDict["proxyHost"] = "websenseproxy";
            configDict["proxyPort"] = "8080";
            Configuration config = new Configuration(configDict);
            
            if (!Directory.Exists(configDict["downloadDirectory"]))
            {
                Directory.CreateDirectory(configDict["downloadDirectory"]);
            }
            
            docRequest = new ChargebackDocumentationRequest();
            docRequest.config = config;
        }
        
        [Test]
        public void TestRetrieveDocument()
        {
            chargebackDocumentReceivedResponse docResponse = 
                (chargebackDocumentReceivedResponse)docRequest.RetrieveDocument(1000, "doc.tiff");
            Assert.NotNull(docResponse);
            Assert.AreEqual(docRequest.config.Get("downloadDirectory") + "\\doc.tiff", docResponse.retrievedFilePath);
            Assert.True(File.Exists(docResponse.retrievedFilePath));
            File.Delete(docResponse.retrievedFilePath);
        }
        
        [Test]
        public void TestRetrieveDocument_DocumentNotFound_009()
        {
            chargebackDocumentUploadResponse docResponse
                = (chargebackDocumentUploadResponse)docRequest.RetrieveDocument(10009, "testDoc.tiff");
            Assert.NotNull(docResponse);
            Assert.AreEqual("009", docResponse.responseCode);
            Assert.AreEqual("Document Not Found".ToLower(), docResponse.responseMessage.ToLower());
        }
        
        [Test]
        public void TestListDocument()
        {
            chargebackDocumentUploadResponse docResponse = docRequest.ListDocuments(1000);
            Assert.NotNull(docResponse);
            Assert.AreEqual("000", docResponse.responseCode);
            Assert.AreEqual("Success".ToLower(), docResponse.responseMessage.ToLower());
        }
        
        [Test]
        public void TestDeleteDocument()
        {
            chargebackDocumentUploadResponse docResponse = docRequest.DeleteDocument(1000, "logo.tiff");
            Assert.NotNull(docResponse);
            Assert.AreEqual("000", docResponse.responseCode);
            Assert.AreEqual("Success".ToLower(), docResponse.responseMessage.ToLower());
        }

        [Test]
        public void TestUploadDocument()
        {
            const string tiffFilename = "uploadTest.tiff";
            var tiffFilePath = Path.Combine(docRequest.config.Get("downloadDirectory"), tiffFilename);
            var writer = new StreamWriter(File.Create(tiffFilePath));
            writer.WriteLine("Prototype a file.");
            writer.Close();

            try
            {
                var docResponse = docRequest.UploadDocument(1000, tiffFilePath);
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
            const string tiffFilename = "uploadTest.tiff";
            var tiffFilePath = Path.Combine(docRequest.config.Get("downloadDirectory"), tiffFilename);
            var writer = new StreamWriter(File.Create(tiffFilePath));
            writer.WriteLine("Prototype a file.");
            writer.Close();

            try
            {
                var docResponse = docRequest.ReplaceDocument(1000, tiffFilename, tiffFilePath);
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

        [TearDown]
        public void TearDown()
        {
            Directory.Delete(docRequest.config.Get("downloadDirectory"));
        }
    }
}