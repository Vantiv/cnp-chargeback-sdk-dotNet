using System;
using System.IO;
using System.Linq;
using ChargebackSdkForNet;
using ChargebackSdkForNet.Properties;
using NUnit.Framework;

namespace ChargebackSdkForNetTest.Certification
{
    public class TestCertificationChargebackDocumentationApi
    {
        private ChargebackDocumentationRequest _docRequest;

        [TestFixtureSetUp]
        public void SetUp()
        {

            var config = new Configuration();
            config.Set("host", "https://services.vantivprelive.com");
            _docRequest = new ChargebackDocumentationRequest { Config = config };

        }

        [Test]
        [Ignore("Cannot run multiple times on Prelive.")]
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
            
            string test1Directory = Path.Combine(Directory.GetCurrentDirectory(), "TestCase1");
            Directory.CreateDirectory(test1Directory);
            
            long caseId = int.Parse(_docRequest.Config.Get("merchantId") + "001");
            var tiffResponse
                = _docRequest.UploadDocument(caseId, tiffFilename);
            var pdfResponse
                = _docRequest.UploadDocument(caseId, pdfFilename);
            var gifResponse
                = _docRequest.UploadDocument(caseId, gifFilename);
            var jpgResponse
                = _docRequest.UploadDocument(caseId, jpgFilename);
            Assert.AreEqual("000", tiffResponse.responseCode);
            Assert.AreEqual("000", pdfResponse.responseCode);
            Assert.AreEqual("000", gifResponse.responseCode);
            Assert.AreEqual("000", jpgResponse.responseCode);
            
            // Step 2. List documents to check success of the uploaded documents.
            var listDocResponse = _docRequest.ListDocuments(caseId);
            Assert.True(listDocResponse.documentId.Contains(tiffFilename));
            Assert.True(listDocResponse.documentId.Contains(pdfFilename));
            Assert.True(listDocResponse.documentId.Contains(gifFilename));
            Assert.True(listDocResponse.documentId.Contains(jpgFilename));
            
            // Step 3. Verify your code can retrieve documents.
            var retrieveDocResponse = _docRequest.RetrieveDocument(caseId, tiffFilename);
            Assert.Less(0, retrieveDocResponse.Count);
            retrieveDocResponse = _docRequest.RetrieveDocument(caseId, pdfFilename);
            Assert.Less(0, retrieveDocResponse.Count);
            retrieveDocResponse = _docRequest.RetrieveDocument(caseId, gifFilename);
            Assert.Less(0, retrieveDocResponse.Count);
            retrieveDocResponse = _docRequest.RetrieveDocument(caseId, jpgFilename);
            Assert.Less(0, retrieveDocResponse.Count);
            
            // Step 4. Verify your code can replace a document.
            string jpgReplacingFilename = "TestCase1Replace.jpg";
            writer = new StreamWriter(File.Create(jpgReplacingFilename));
            writer.WriteLine("Prototype a file.");
            writer.Close();
            var chargebackDocumentUploadResponse = _docRequest.ReplaceDocument(caseId, "TestCase1.jpg", jpgReplacingFilename);
            Assert.AreEqual("000", chargebackDocumentUploadResponse.responseCode);

            // Step 5. Try to retrieve the replaced file.
            retrieveDocResponse = _docRequest.RetrieveDocument(caseId, jpgReplacingFilename);
            Assert.Less(0, retrieveDocResponse.Count);
            
            // Step 6. Verify that your code can delete documents.
            chargebackDocumentUploadResponse = _docRequest.DeleteDocument(caseId, tiffFilename);
            Assert.AreEqual("000", chargebackDocumentUploadResponse.responseCode);
            
            // Step 7. Verify the successful deletion by listing documents.
            chargebackDocumentUploadResponse = _docRequest.ListDocuments(caseId);
            Assert.False(chargebackDocumentUploadResponse.documentId.Contains(tiffFilename));
            
            /*Delete all files created and uploaded for tests.*/
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
            chargebackDocumentUploadResponse = _docRequest.DeleteDocument(caseId, pdfFilename);
            Assert.AreEqual("000", chargebackDocumentUploadResponse.responseCode);
            chargebackDocumentUploadResponse = _docRequest.DeleteDocument(caseId, gifFilename);
            Assert.AreEqual("000", chargebackDocumentUploadResponse.responseCode);
            chargebackDocumentUploadResponse = _docRequest.DeleteDocument(caseId, jpgFilename);
            Assert.AreEqual("000", chargebackDocumentUploadResponse.responseCode);
        }
        
        [Test]
        [Ignore("Cannot run multiple times on Prelive.")]
        public void TestCase2()
        {
            // Step 1. Upload one file to the second test location.
            var tiffFilename = "TestCase1.tiff";
            var writer = new StreamWriter(File.Create(tiffFilename));
            writer.WriteLine("Prototype a file.");
            writer.Close();
            
            long caseId = Int32.Parse(_docRequest.Config.Get("merchantId") + "002");
            var tiffResponse
                = _docRequest.UploadDocument(caseId, tiffFilename);
            
            // Step 2. Verify that you receive the response code 010.
            Assert.AreEqual("010", tiffResponse.responseCode);
            Assert.AreEqual("Case not in valid cycle".ToLower(), tiffResponse.responseMessage.ToLower());
            
            File.Delete(tiffFilename);
        }
        
        [Test]
        public void TestCase3()
        {
            // Step 1. Upload one file to the second test location.
            var tiffFilename = "TestCase1.tiff";
            var writer = new StreamWriter(File.Create(tiffFilename));
            writer.WriteLine("Prototype a file.");
            writer.Close();
            
            long caseId = Int32.Parse(_docRequest.Config.Get("merchantId") + "003");
            var tiffResponse
                = _docRequest.UploadDocument(caseId, tiffFilename);
            
            // Step 2. Verify that you receive the response code 004.
            Assert.AreEqual("004", tiffResponse.responseCode);
            Assert.AreEqual("Case not in Merchant Queue".ToLower(), tiffResponse.responseMessage.ToLower());
            
            File.Delete(tiffFilename);
        }
        
        [Test]
        [Ignore("Cannot run multiple times on Prelive.")]
        public void TestCase4()
        {
            
            // Step 1. Upload the file named maxsize.tif to the fourth test location.
            var tifSize = 1024; // 1024 bytes = 1KB.
            var tifFilename = "maxsize.mp3";
            var fileCreator = File.OpenWrite(tifFilename);
            var fileBytes = new byte[tifSize];
            fileCreator.Write(fileBytes, 0, fileBytes.Length);
            fileCreator.Close();
            var caseId = int.Parse(_docRequest.Config.Get("merchantId") + "004");
            var maxSizeResponse
                = _docRequest.UploadDocument(caseId, tifFilename);
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
                = _docRequest.UploadDocument(caseId, hugeFilename);
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
                = _docRequest.UploadDocument(caseId, mediumFilename);
            File.Delete(mediumFilename);
            // Step 6. Verify that you receive the response code 008.
            Assert.AreEqual("008", mediumSizeResponse.responseCode);
            Assert.AreEqual("Max Document Limit Per Case Reached".ToLower(), mediumSizeResponse.responseMessage.ToLower());
            
        }
    }
}