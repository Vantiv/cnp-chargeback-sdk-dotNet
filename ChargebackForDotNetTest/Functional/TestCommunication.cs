
using System;
using System.Collections.Generic;
using ChargebackForDotNet;
using ChargebackForDotNet.Properties;
using NUnit.Framework;

namespace ChargebackForDotNetTest.Functional
{
        [TestFixture]
        class TestCommunication
        {

            [Test]
            public void TestGet()
            {
                Configuration conf = new Configuration();
                string date = "?date=2013-01-01";
                List<byte> bytes = new List<byte>();
                string contentType =Communication.get(conf, "/services/chargebacks/"+date,bytes);
                Console.WriteLine("Content type returned from the server::"+contentType);
                String xmlResponse = Utils.bytesToString(bytes);
                Console.WriteLine(xmlResponse);
                Assert.True(true);
            }
            
            
            
            

            [Test]
            public void TestRetrieveByActivityDateWithFinancialImpact()
            {
                Configuration conf = new Configuration();
                ChargebackRetrievalRequest request = new ChargebackRetrievalRequest();
                request.config = conf;
                chargebackRetrievalResponse response = 
                    request.retrieveByActivityDateWithImpact(new DateTime(2013, 1, 1), true);
                Assert.NotNull(response);
            }
            
            [Test]
            public void TestRetrieveActionable()
            {
                Configuration conf = new Configuration();
                ChargebackRetrievalRequest request = new ChargebackRetrievalRequest();
                request.config = conf;
                chargebackRetrievalResponse response = request.retrieveActionable(true);
                Assert.NotNull(response);
            }
            
            [Test]
            public void TestRetrieveByCaseId()
            {
                Configuration conf = new Configuration();
                ChargebackRetrievalRequest request = new ChargebackRetrievalRequest();
                request.config = conf;
                chargebackRetrievalResponse response = request.retrieveByCaseId(1288791001);
                Assert.NotNull(response);
            }
            
            [Test]
            public void TestRetrieveByCardNumber()
            {
                Configuration conf = new Configuration();
                ChargebackRetrievalRequest request = new ChargebackRetrievalRequest();
                request.config = conf;
                chargebackRetrievalResponse response = 
                    request.retrieveByCardNumber(0001.ToString(), new DateTime(2013, 1, 15));
                Assert.NotNull(response);
            }
            
            [Test]
            public void TestRetrieveByArn()
            {
                Configuration conf = new Configuration();
                ChargebackRetrievalRequest request = new ChargebackRetrievalRequest();
                request.config = conf;
                chargebackRetrievalResponse response = request.retrieveByArn(5555555551.ToString());
                Assert.NotNull(response);
            }
            
            [Test]
            public void TestRetrieveDocument()
            {
                Configuration conf = new Configuration();
                ChargebackDocumentationRequest request = new ChargebackDocumentationRequest();
                conf.setConfigValue("host", "https://www.testvantivcnp.com/sandbox/new");
                request.config = conf;
                chargebackDocumentReceivedResponse response = 
                    (chargebackDocumentReceivedResponse)request.retrieveDocument(1000, "doc.tiff");
                Assert.NotNull(response);
                Console.WriteLine(response.retrievedFilePath);
            }
            
            [Test]
            public void TestRetrieveDocument_DocumentNotFound_009()
            {
                Configuration conf = new Configuration();
                ChargebackDocumentationRequest request = new ChargebackDocumentationRequest();
                conf.setConfigValue("host", "https://www.testvantivcnp.com/sandbox/new");
                request.config = conf;
                chargebackDocumentUploadResponse response = (chargebackDocumentUploadResponse)request.retrieveDocument(10009, "testDoc.tiff");
                Assert.NotNull(response);
                Assert.AreEqual("009", response.responseCode);
                Assert.AreEqual("Document Not Found".ToLower(), response.responseMessage.ToLower());
            }
            
            [Test]
            public void TestListDocument()
            {
                Configuration conf = new Configuration();
                ChargebackDocumentationRequest request = new ChargebackDocumentationRequest();
                conf.setConfigValue("host", "https://www.testvantivcnp.com/sandbox/new");
                request.config = conf;
                chargebackDocumentUploadResponse response = request.listDocuments(1000);
                Assert.NotNull(response);
                Assert.AreEqual("000", response.responseCode);
                Assert.AreEqual("Success".ToLower(), response.responseMessage.ToLower());
                
            }
            
            [Test]
            public void TestDeleteDocument()
            {
                
                Configuration conf = new Configuration();
                ChargebackDocumentationRequest request = new ChargebackDocumentationRequest();
                conf.setConfigValue("host", "https://www.testvantivcnp.com/sandbox/new");
                request.config = conf;
                chargebackDocumentUploadResponse response = request.deleteDocument(1000, "logo.tiff");
                Assert.NotNull(response);
                Assert.AreEqual("000", response.responseCode);
                Assert.AreEqual("Success".ToLower(), response.responseMessage.ToLower());
                
            }
        }
}