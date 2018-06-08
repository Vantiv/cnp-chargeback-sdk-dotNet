using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using ChargebackSdkForNet;
using ChargebackSdkForNet.Properties;
using NUnit.Framework;

namespace ChargebackSdkForNetTest.Functional
{
    [TestFixture]
    internal class TestCommunication
    {
        private Communication _comm;
        private Configuration _config;

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
            _config = new Configuration(configDict);

            _comm = new Communication();
            _comm.SetHost(_config.Get("host"));
        }

        [Test]
        public void TestGet()
        {
            const string date = "?date=2013-01-01";
            var encoded = ChargebackUtils.Encode64(_config.Get("username") + ":" + _config.Get("password"),
                "utf-8");
            _comm.AddToHeader("Authorization", "Basic " + encoded);
            _comm.SetContentType("application/com.vantivcnp.services-v2+xml");
            _comm.SetAccept("application/com.vantivcnp.services-v2+xml");
            _comm.SetProxy(_config.Get("proxyHost"), int.Parse(_config.Get("proxyPort")));
            var responseContent = _comm.Get("/services/chargebacks/" + date);
            var contentType = responseContent.GetContentType();
            var receivedBytes = responseContent.GetByteData();
            Assert.True(receivedBytes.Any());
            Console.WriteLine("Content type returned from the server::" + contentType);
            var xmlResponse = Regex.Replace(ChargebackUtils.BytesToString(receivedBytes), @"\t|\n|\r", "");
            Console.WriteLine(xmlResponse);
            const string pattern = @"<?xml version=.* encoding=.* standalone=.*?>.*" +
                                   "<chargebackRetrievalResponse xmlns=.*<transactionId>.*</transactionId>.*" +
                                   "<chargebackCase>.*</chargebackCase>.*</chargebackRetrievalResponse>";
            var regex = new Regex(pattern, RegexOptions.Multiline);
            Assert.True(regex.IsMatch(xmlResponse));
        }

        [Test]
        public void TestPut()
        {
            const string xmlRequest = "<?xml version='1.0' encoding='UTF-8' standalone='yes'?>" +
                                      "<chargebackUpdateRequest xmlns='http://www.vantivcnp.com/chargebacks'>" +
                                      "<activityType>ADD_NOTE</activityType>" +
                                      "<note>Any note</note>" +
                                      "</chargebackUpdateRequest>";
            var sendingBytes = ChargebackUtils.StringToBytes(xmlRequest);
            const int caseId = 1000;
            var encoded = ChargebackUtils.Encode64(_config.Get("username") + ":" + _config.Get("password"),
                "utf-8");
            _comm.AddToHeader("Authorization", "Basic " + encoded);
            _comm.SetContentType("application/com.vantivcnp.services-v2+xml");
            _comm.SetAccept("application/com.vantivcnp.services-v2+xml");
            _comm.SetProxy(_config.Get("proxyHost"), int.Parse(_config.Get("proxyPort")));
            var responseContent = _comm.Put("/services/chargebacks/" + caseId, sendingBytes);
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

            var encoded = ChargebackUtils.Encode64(_config.Get("username") + ":" + _config.Get("password"), "utf-8");
            _comm.AddToHeader("Authorization", "Basic " + encoded);
            _comm.SetProxy(_config.Get("proxyHost"), int.Parse(_config.Get("proxyPort")));
            _comm.SetContentType("image/tiff");
            try
            {
                var responseContent = _comm.Post("/services/chargebacks/upload/" + caseId + "/" + documentId, sendingBytes);
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
            var encoded = ChargebackUtils.Encode64(_config.Get("username") + ":" + _config.Get("password"), "utf-8");
            _comm.AddToHeader("Authorization", "Basic " + encoded);
            _comm.SetProxy(_config.Get("proxyHost"), int.Parse(_config.Get("proxyPort")));
            var responseContent = _comm.Delete(string.Format("/services/chargebacks/delete/{0}/{1}", caseId, documentId));
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
