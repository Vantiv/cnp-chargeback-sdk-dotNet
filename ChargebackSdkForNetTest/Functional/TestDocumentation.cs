using System;
using System.Collections.Generic;
using System.IO;
using ChargebackSdkForNet;
using ChargebackSdkForNet.Properties;
using Xunit;

namespace ChargebackSdkForNetTest.Functional
{
    public class TestDocumentation
    {
        private ChargebackDocumentationRequest _docRequest;

        public TestDocumentation()
        {
            _docRequest = new ChargebackDocumentationRequest();
        }
        
        [Fact]
        public void TestRetrieveDocument()
        {
            var docResponse = _docRequest.RetrieveDocument(1000, "doc.tiff");
            Assert.NotNull(docResponse);
            Assert.True(0 < docResponse.Count);
        }
        
        [Fact]
        public void TestRetrieveDocument_DocumentNotFound_009()
        {
            Assert.Throws<ChargebackDocumentException>(() => _docRequest.RetrieveDocument(10009, "testDoc.tiff"));
        }
        
        [Fact]
        public void TestListDocument()
        {
            chargebackDocumentUploadResponse docResponse = _docRequest.ListDocuments(1000);
            Assert.NotNull(docResponse);
            Assert.Equal("000", docResponse.responseCode);
            Assert.Equal("Success".ToLower(), docResponse.responseMessage.ToLower());
        }
        
        [Fact]
        public void TestDeleteDocument()
        {
            chargebackDocumentUploadResponse docResponse = _docRequest.DeleteDocument(1000, "logo.tiff");
            Assert.NotNull(docResponse);
            Assert.Equal("000", docResponse.responseCode);
            Assert.Equal("Success".ToLower(), docResponse.responseMessage.ToLower());
        }

        [Fact]
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
                Assert.Equal("000", docResponse.responseCode);
                Assert.Equal("Success".ToLower(), docResponse.responseMessage.ToLower());
            }

            catch (Exception e)
            {
                Assert.True(false, "Upload Test failed" + e);
            }
            finally
            {
                File.Delete(tiffFilePath);
            }
        }

        [Fact]
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
                Assert.Equal("000", docResponse.responseCode);
                Assert.Equal("Success".ToLower(), docResponse.responseMessage.ToLower());
            }

            catch (Exception e)
            {
                Assert.True(false, "Upload Test failed" + e);
            }
            finally
            {
                File.Delete(tiffFilePath);
            }
        }
    }
}
