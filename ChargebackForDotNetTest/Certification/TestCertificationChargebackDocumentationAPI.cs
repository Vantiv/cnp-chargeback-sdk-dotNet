using System;
using System.IO;
using System.Linq;
using ChargebackForDotNet;
using NUnit.Framework;

namespace ChargebackForDotNetTest.Certification
{
    public class TestCertificationChargebackDocumentationAPI
    {
        
        [Test]
        public void TestCase1()
        {
            // Step 1. Test uploading document.
            string tiffFilename = "TestCase1.tiff";
            StreamWriter writer = new StreamWriter(File.Create(tiffFilename));
            writer.WriteLine("Prototype a file.");
            writer.Close();
            
            string pdfFilename = "TestCase1.pdf";
            writer = new StreamWriter(File.Create(pdfFilename));
            writer.WriteLine("Prototype a file.");
            writer.Close();
            
            string gifFilename = "TestCase1.gif";
            writer = new StreamWriter(File.Create(gifFilename));
            writer.WriteLine("Prototype a file.");
            writer.Close();
            
            string jpgFilename = "TestCase1.jpg";
            writer = new StreamWriter(File.Create(jpgFilename));
            writer.WriteLine("Prototype a file.");
            writer.Close();
            
            ChargebackDocumentationRequest docRequest
                = new ChargebackDocumentationRequest();
//            docRequest.config.setConfigValue("host", "https://www.testvantivcnp.com/sandbox/new");
            string test1Directory = Path.Combine(Directory.GetCurrentDirectory(), "TestCase1");
            Directory.CreateDirectory(test1Directory);
            docRequest.config.setConfigValue("downloadDirectory", test1Directory);
            long caseId = Int32.Parse(docRequest.config.getConfig("merchantId") + "000");
            chargebackDocumentUploadResponse tiffResponse
                = docRequest.uploadDocument(caseId, tiffFilename);
            chargebackDocumentUploadResponse pdfResponse
                = docRequest.uploadDocument(caseId, pdfFilename);
            chargebackDocumentUploadResponse gifResponse
                = docRequest.uploadDocument(caseId, gifFilename);
            chargebackDocumentUploadResponse jpgResponse
                = docRequest.uploadDocument(caseId, jpgFilename);
            Assert.AreEqual("000", tiffResponse.responseCode);
            Assert.AreEqual("000", pdfResponse.responseCode);
            Assert.AreEqual("000", gifResponse.responseCode);
            Assert.AreEqual("000", jpgResponse.responseCode);
            
            // Step 2. List documents to check success of the uploaded documents.
            var listDocResponse = docRequest.listDocuments(caseId);
            listDocResponse.documentId.Contains(tiffFilename);
            listDocResponse.documentId.Contains(pdfFilename);
            listDocResponse.documentId.Contains(gifFilename);
            listDocResponse.documentId.Contains(jpgFilename);
            
            // Step 3. Verify your code can retrieve documents.
            var retrieveDocResponse = docRequest.retrieveDocument(caseId, tiffFilename);
            Assert.True(retrieveDocResponse is chargebackDocumentReceivedResponse);
            retrieveDocResponse = docRequest.retrieveDocument(caseId, pdfFilename);
            Assert.True(retrieveDocResponse is chargebackDocumentReceivedResponse);
            retrieveDocResponse = docRequest.retrieveDocument(caseId, gifFilename);
            Assert.True(retrieveDocResponse is chargebackDocumentReceivedResponse);
            retrieveDocResponse = docRequest.retrieveDocument(caseId, jpgFilename);
            Assert.True(retrieveDocResponse is chargebackDocumentReceivedResponse);
            
            // Step 4. Verify your code can replace a document.
            string jpgReplacingFilename = "TestCase1Replace.jpg";
            writer = new StreamWriter(File.Create(jpgReplacingFilename));
            writer.WriteLine("Prototype a file.");
            writer.Close();
            var chargebackDocumentUploadResponse = docRequest.replaceDocument(caseId, "TestCase1.jpg", jpgReplacingFilename);
            Assert.AreEqual("000", chargebackDocumentUploadResponse.responseCode);

            // Step 5. Try to retrieve the replaced file.
            retrieveDocResponse = docRequest.retrieveDocument(caseId, jpgReplacingFilename);
            Assert.True(retrieveDocResponse is chargebackDocumentReceivedResponse);
            
            // Step 6. Verify that your code can delete documents.
            chargebackDocumentUploadResponse = docRequest.deleteDocument(caseId, tiffFilename);
            Assert.AreEqual("000", chargebackDocumentUploadResponse.responseCode);
            
            // Step 7. Verify the successful deletion by listing documents.
            chargebackDocumentUploadResponse = docRequest.listDocuments(caseId);
            Assert.False(chargebackDocumentUploadResponse.documentId.Contains(tiffFilename));
            
            /*Remove all files created and uploaded for tests.*/
            // Local files.
            string[] fileNames = Directory.GetFiles(Directory.GetCurrentDirectory());
            foreach (var file in fileNames)
            {
                if (".tiff".Equals(Path.GetExtension(file)) || ".pdf".Equals(Path.GetExtension(file))
                    || ".gif".Equals(Path.GetExtension(file)) || ".jpg".Equals(Path.GetExtension(file)))
                {
                    File.Delete(file);
                }
            }
            Directory.Delete(test1Directory, true);
            // Remote files.
            chargebackDocumentUploadResponse = docRequest.deleteDocument(caseId, pdfFilename);
            Assert.AreEqual("000", chargebackDocumentUploadResponse.responseCode);
            chargebackDocumentUploadResponse = docRequest.deleteDocument(caseId, gifFilename);
            Assert.AreEqual("000", chargebackDocumentUploadResponse.responseCode);
            chargebackDocumentUploadResponse = docRequest.deleteDocument(caseId, jpgFilename);
            Assert.AreEqual("000", chargebackDocumentUploadResponse.responseCode);
        }
        
        [Test]
        public void TestCase2()
        {
            // Step 1. Upload one file to the second test location.
            string tiffFilename = "TestCase1.tiff";
            StreamWriter writer = new StreamWriter(File.Create(tiffFilename));
            writer.WriteLine("Prototype a file.");
            writer.Close();
            
            ChargebackDocumentationRequest docRequest
                = new ChargebackDocumentationRequest();
            long caseId = Int32.Parse(docRequest.config.getConfig("merchantId") + "002");
            chargebackDocumentUploadResponse tiffResponse
                = docRequest.uploadDocument(caseId, tiffFilename);
            
            // Step 2. Verify that you receive the response code 010.
            Assert.AreEqual("010", tiffResponse.responseCode);
            Assert.AreEqual("Case not in valid cycle".ToLower(), tiffResponse.responseMessage.ToLower());
            
            File.Delete(tiffFilename);
        }
        
        [Test]
        public void TestCase3()
        {
            // Step 1. Upload one file to the second test location.
            string tiffFilename = "TestCase1.tiff";
            StreamWriter writer = new StreamWriter(File.Create(tiffFilename));
            writer.WriteLine("Prototype a file.");
            writer.Close();
            
            ChargebackDocumentationRequest docRequest
                = new ChargebackDocumentationRequest();
            long caseId = Int32.Parse(docRequest.config.getConfig("merchantId") + "003");
            chargebackDocumentUploadResponse tiffResponse
                = docRequest.uploadDocument(caseId, tiffFilename);
            
            // Step 2. Verify that you receive the response code 004.
            Assert.AreEqual("004", tiffResponse.responseCode);
            Assert.AreEqual("Case not in Merchant Queue".ToLower(), tiffResponse.responseMessage.ToLower());
            
            File.Delete(tiffFilename);
        }
        
        [Test]
        public void TestCase4()
        {
            ChargebackDocumentationRequest docRequest
                = new ChargebackDocumentationRequest();
//            docRequest.config.setConfigValue("host", "https://www.testvantivcnp.com/sandbox/new");
            
            // Step 1. Upload the file named maxsize.tif to the fourth test location.
            int tifSize = 1024; // 1024 bytes = 1KB.
            string tifFilename = "maxsize.tif";
            FileStream fileCreator = File.OpenWrite(tifFilename);
            byte[] fileBytes = new byte[tifSize];
            fileCreator.Write(fileBytes, 0, fileBytes.Length);
            fileCreator.Close();
            long caseId = Int32.Parse(docRequest.config.getConfig("merchantId") + "004");
            chargebackDocumentUploadResponse maxSizeResponse
                = docRequest.uploadDocument(caseId, tifFilename);
            File.Delete(tifFilename);
            
            // Step 2. Verify that you receive the response code 005.
            Assert.AreEqual("005", maxSizeResponse.responseCode);
            Assert.AreEqual("Document already exists".ToLower(), maxSizeResponse.responseMessage.ToLower());
            
            // Step 3. Upload a file with a size greather than 2MB.
            int hugeSize = 2 * 1024 * 1024 + 1024; // 2MB * 1024KB/MB * 1024byte/KB + 1024bytes; 
            string hugeFilename = "huge.tif";
            fileBytes = new byte[hugeSize];
            fileCreator = File.OpenWrite(hugeFilename);
            fileCreator.Write(fileBytes, 0, fileBytes.Length);
            fileCreator.Close();
            Console.WriteLine("Finish writing the file " + hugeFilename);
            chargebackDocumentUploadResponse hugeSizeResponse
                = docRequest.uploadDocument(caseId, hugeFilename);
            File.Delete(hugeFilename);
            
            // Step 4. Verify that you receive the response code 012.
            Assert.AreEqual("012", hugeSizeResponse.responseCode);
            Assert.IsTrue(hugeSizeResponse.responseMessage.ToLower().Contains("Filesize exceeds limit of".ToLower()));
            
            // Step 5. Upload a file greater than 100KB, but less than 1MB.
            int mediumSize = 512000; // 500KB * 1024byte/KB = 512000 bytes.
            string mediumFilename = "medium.tif";
            fileBytes = new byte[mediumSize];
            fileCreator = File.OpenWrite(mediumFilename);
            fileCreator.Write(fileBytes, 0, fileBytes.Length);
            fileCreator.Close();
            chargebackDocumentUploadResponse mediumSizeResponse
                = docRequest.uploadDocument(caseId, mediumFilename);
            File.Delete(mediumFilename);
            // Step 6. Verify that you receive the response code 008.
            Assert.AreEqual("008", mediumSizeResponse.responseCode);
            Assert.AreEqual("Max Document Limit Per Case Reached".ToLower(), mediumSizeResponse.responseMessage.ToLower());
            
        }
    }
}