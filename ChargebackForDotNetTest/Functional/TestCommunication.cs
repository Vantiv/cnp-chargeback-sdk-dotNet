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
            configDict["downloadDirectory"] = "C:\\Vantiv\\chargebacks";
            configDict["printXml"] = "true";
            configDict["proxyHost"] = "websenseproxy";
            configDict["proxyPort"] = "8080";
            config = new Configuration(configDict);
            
            if (!Directory.Exists(configDict["downloadDirectory"]))
            {
                Directory.CreateDirectory(configDict["downloadDirectory"]);
            }

            comm = new Communication();
            comm.setHost(config.getConfig("host"));
        }

        [Test]
        public void TestGet()
        {
            string date = "?date=2013-01-01";
            string encoded = Utils.encode64(config.getConfig("username") + ":" + config.getConfig("password"),
                "utf-8");
            comm.addToHeader("Authorization", "Basic " + encoded);
            comm.setContentType("application/com.vantivcnp.services-v2+xml");
            comm.setAccept("application/com.vantivcnp.services-v2+xml");
            comm.setProxy(config.getConfig("proxyHost"), int.Parse(config.getConfig("proxyPort")));
            var responseTuple = comm.get("/services/chargebacks/" + date);
            var contentType = (string) responseTuple[0];
            var receivedBytes = (List<byte>) responseTuple[1];
            Assert.True(receivedBytes.Any());
            Console.WriteLine("Content type returned from the server::" + contentType);
            string xmlResponse = Regex.Replace(Utils.bytesToString(receivedBytes), @"\t|\n|\r", "");
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
            List<byte> sendingBytes = new List<byte>();
            string xmlRequest = "<?xml version='1.0' encoding='UTF-8' standalone='yes'?>" +
                                "<chargebackUpdateRequest xmlns='http://www.vantivcnp.com/chargebacks'>" +
                                "<activityType>ADD_NOTE</activityType>" +
                                "<note>Any note</note>" +
                                "</chargebackUpdateRequest>";
            sendingBytes = Utils.stringToBytes(xmlRequest);
            long caseId = 1000;
            string encoded = Utils.encode64(config.getConfig("username") + ":" + config.getConfig("password"),
                "utf-8");
            comm.addToHeader("Authorization", "Basic " + encoded);
            comm.setContentType("application/com.vantivcnp.services-v2+xml");
            comm.setAccept("application/com.vantivcnp.services-v2+xml");
            comm.setProxy(config.getConfig("proxyHost"), int.Parse(config.getConfig("proxyPort")));
            var responseTuple = comm.put("/services/chargebacks/" + caseId, sendingBytes);
            var contentType = (string) responseTuple[0];
            var receivedBytes = (List<byte>) responseTuple[1];
            Assert.True(receivedBytes.Any());
            string xmlResponse = Regex.Replace(Utils.bytesToString(receivedBytes), @"\t|\n|\r", "");
            string pattern = @"<?xml version=.* encoding=.* standalone=.*?>.*" +
                             "<chargebackUpdateResponse xmlns=.*<transactionId>.*</transactionId>.*" +
                             "</chargebackUpdateResponse>";
            Regex regex = new Regex(pattern, RegexOptions.Multiline);
            Assert.True(regex.IsMatch(xmlResponse));
        }

        [Test]
        public void TestPost()
        {
            long caseId = 1000;
            var sendingBytes = new List<byte>();
            const string tiffFilename = "uploadTest.tiff";
            var tiffFilePath = Path.Combine(config.getConfig("downloadDirectory"), tiffFilename);
            var writer = new StreamWriter(File.Create(tiffFilePath));
            writer.WriteLine("Prototype a file.");
            writer.Close();
            sendingBytes = File.ReadAllBytes(tiffFilePath).ToList();

            string encoded = Utils.encode64(config.getConfig("username") + ":" + config.getConfig("password"), "utf-8");
            comm.addToHeader("Authorization", "Basic " + encoded);
            comm.setProxy(config.getConfig("proxyHost"), int.Parse(config.getConfig("proxyPort")));
            comm.setContentType("image/tiff");
            try
            {
                var responseTuple = comm.post("/services/chargebacks/upload/" + caseId + "/" + tiffFilename, sendingBytes);
                var contentType = (string) responseTuple[0];
                var receivedBytes = (List<byte>) responseTuple[1];
                Assert.True(receivedBytes.Any());
                string xmlResponse = Regex.Replace(Utils.bytesToString(receivedBytes), @"\t|\n|\r", "");
                string pattern = @"<?xml version=.* encoding=.* standalone=.*?>.*" +
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
            finally
            {
                File.Delete(tiffFilePath);
            }
        }

        [Test]
        public void TestDelete()
        {
            long caseId = 1000;
            const string tiffFilename = "uploadTest.tiff";
            string encoded = Utils.encode64(config.getConfig("username") + ":" + config.getConfig("password"), "utf-8");
            comm.addToHeader("Authorization", "Basic " + encoded);
            comm.setProxy(config.getConfig("proxyHost"), int.Parse(config.getConfig("proxyPort")));
            var responseTuple = comm.delete(string.Format("/services/chargebacks/remove/{0}/{1}", caseId, tiffFilename));
            var contentType = (string) responseTuple[0];
            var receivedBytes = (List<byte>) responseTuple[1];
            Assert.True(receivedBytes.Any());
            string xmlResponse = Regex.Replace(Utils.bytesToString(receivedBytes), @"\t|\n|\r", "");
            string pattern = @"<?xml version=.* encoding=.* standalone=.*?>.*" +
                             "<chargebackDocumentUploadResponse xmlns=.*<merchantId>.*</merchantId>.*" +
                             "<caseId>.*</caseId>.*" +
                             "<responseCode>000</responseCode>.*" +
                             "<responseMessage>Success</responseMessage>.*" +
                             "</chargebackDocumentUploadResponse>";
            Regex regex = new Regex(pattern, RegexOptions.Multiline);
            Assert.True(regex.IsMatch(xmlResponse));
        }
        
        [TearDown]
        public void TearDown()
        {
            Directory.Delete(config.getConfig("downloadDirectory"));
        }
    }
}