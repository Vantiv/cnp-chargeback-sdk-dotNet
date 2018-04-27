using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using ChargebackForDotNet;
using ChargebackForDotNet.Properties;
using NUnit.Framework;

namespace ChargebackForDotNetTest.Functional
{
    [TestFixture]
    class TestCommunication
    {
        private Communication comm;
        private Configuration config;

        [SetUp]
        public void SetUp()
        {
            Dictionary<string, string> configDict = new Dictionary<string, string>();
            configDict["username"] = "dotnet";
            configDict["password"] = "dotnet";
            configDict["merchantId"] = "101";
            configDict["host"] = "https://www.testvantivcnp.com/sandbox/new";
            configDict["printXml"] = "true";
            configDict["proxyHost"] = "websenseproxy";
            configDict["proxyPort"] = "8080";
            config = new Configuration(configDict);

            comm = new Communication();
            comm.SetHost(config.Get("host"));
        }

        [Test]
        public void TestGet()
        {
            string date = "?date=2013-01-01";
            string encoded = ChargebackUtils.Encode64(config.Get("username") + ":" + config.Get("password"),
                "utf-8");
            comm.AddToHeader("Authorization", "Basic " + encoded);
            comm.SetContentType("application/com.vantivcnp.services-v2+xml");
            comm.SetAccept("application/com.vantivcnp.services-v2+xml");
            comm.SetProxy(config.Get("proxyHost"), int.Parse(config.Get("proxyPort")));
            var responseContent = comm.Get("/services/chargebacks/" + date);
            var contentType = responseContent.GetContentType();
            var receivedBytes = responseContent.GetByteData();
            Assert.True(receivedBytes.Any());
            Console.WriteLine("Content type returned from the server::" + contentType);
            string xmlResponse = Regex.Replace(ChargebackUtils.BytesToString(receivedBytes), @"\t|\n|\r", "");
            Console.WriteLine(xmlResponse);
            string pattern = @"<?xml version=.* encoding=.* standalone=.*?>.*" +
                             "<chargebackRetrievalResponse xmlns=.*<transactionId>.*</transactionId>.*" +
                             "<chargebackCase>.*</chargebackCase>.*</chargebackRetrievalResponse>";
            Regex regex = new Regex(pattern, RegexOptions.Multiline);
            Assert.True(regex.IsMatch(xmlResponse));
        }

        [Test]
        public void TestPut()
        {
            var sendingBytes = new List<byte>();
            var xmlRequest = "<?xml version='1.0' encoding='UTF-8' standalone='yes'?>" +
                                "<chargebackUpdateRequest xmlns='http://www.vantivcnp.com/chargebacks'>" +
                                "<activityType>ADD_NOTE</activityType>" +
                                "<note>Any note</note>" +
                                "</chargebackUpdateRequest>";
            sendingBytes = ChargebackUtils.StringToBytes(xmlRequest);
            var caseId = 1000;
            var encoded = ChargebackUtils.Encode64(config.Get("username") + ":" + config.Get("password"),
                "utf-8");
            comm.AddToHeader("Authorization", "Basic " + encoded);
            comm.SetContentType("application/com.vantivcnp.services-v2+xml");
            comm.SetAccept("application/com.vantivcnp.services-v2+xml");
            comm.SetProxy(config.Get("proxyHost"), int.Parse(config.Get("proxyPort")));
            var responseContent = comm.Put("/services/chargebacks/" + caseId, sendingBytes);
            var contentType = responseContent.GetContentType();
            var receivedBytes = responseContent.GetByteData();
            Assert.True(receivedBytes.Any());
            var xmlResponse = Regex.Replace(ChargebackUtils.BytesToString(receivedBytes), @"\t|\n|\r", "");
            var pattern = @"<?xml version=.* encoding=.* standalone=.*?>.*" +
                             "<chargebackUpdateResponse xmlns=.*<transactionId>.*</transactionId>.*" +
                             "</chargebackUpdateResponse>";
            var regex = new Regex(pattern, RegexOptions.Multiline);
            Assert.True(regex.IsMatch(xmlResponse));
        }

        [Test]
        public void TestPost()
        {
            var caseId = 1000;
            var documentId = "callVantiv.pdf";
            var sendingBytes = ChargebackUtils.StringToBytes("Hello! Call Vantiv!");

            var encoded = ChargebackUtils.Encode64(config.Get("username") + ":" + config.Get("password"), "utf-8");
            comm.AddToHeader("Authorization", "Basic " + encoded);
            comm.SetProxy(config.Get("proxyHost"), int.Parse(config.Get("proxyPort")));
            comm.SetContentType("image/tiff");
            try
            {
                var responseContent = comm.Post("/services/chargebacks/upload/" + caseId + "/" + documentId, sendingBytes);
                var contentType = responseContent.GetContentType();
                var receivedBytes = responseContent.GetByteData();
                Assert.True(receivedBytes.Any());
                var xmlResponse = Regex.Replace(ChargebackUtils.BytesToString(receivedBytes), @"\t|\n|\r", "");
                var pattern = @"<?xml version=.* encoding=.* standalone=.*?>.*" +
                                 "<chargebackDocumentUploadResponse xmlns=.*<merchantId>.*</merchantId>.*" +
                                 "<caseId>.*</caseId>.*" +
                                 "<responseCode>000</responseCode>.*" +
                                 "<responseMessage>Success</responseMessage>.*" +
                                 "</chargebackDocumentUploadResponse>";
                Regex regex = new Regex(pattern, RegexOptions.Multiline);
                Assert.True(regex.IsMatch(xmlResponse));
            }
            catch (Exception e)
            {
                Assert.Fail("Post failed" + e);
            }
        }

        [Test]
        public void TestDelete()
        {
            var caseId = 1000;
            const string documentId = "uploadTest.tiff";
            var encoded = ChargebackUtils.Encode64(config.Get("username") + ":" + config.Get("password"), "utf-8");
            comm.AddToHeader("Authorization", "Basic " + encoded);
            comm.SetProxy(config.Get("proxyHost"), int.Parse(config.Get("proxyPort")));
            var responseContent = comm.Delete(string.Format("/services/chargebacks/remove/{0}/{1}", caseId, documentId));
            var contentType = responseContent.GetContentType();
            var receivedBytes = responseContent.GetByteData();
            Assert.True(receivedBytes.Any());
            var xmlResponse = Regex.Replace(ChargebackUtils.BytesToString(receivedBytes), @"\t|\n|\r", "");
            var pattern = @"<?xml version=.* encoding=.* standalone=.*?>.*" +
                             "<chargebackDocumentUploadResponse xmlns=.*<merchantId>.*</merchantId>.*" +
                             "<caseId>.*</caseId>.*" +
                             "<responseCode>000</responseCode>.*" +
                             "<responseMessage>Success</responseMessage>.*" +
                             "</chargebackDocumentUploadResponse>";
            var regex = new Regex(pattern, RegexOptions.Multiline);
            Assert.True(regex.IsMatch(xmlResponse));
        }
    }
}