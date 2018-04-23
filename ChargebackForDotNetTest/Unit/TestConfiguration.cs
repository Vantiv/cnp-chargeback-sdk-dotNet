using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using NUnit.Framework;
using Moq;
using System.Text.RegularExpressions;
using ChargebackForDotNet;
using ChargebackForDotNet.Properties;
using NUnit.Core;


namespace ChargebackForDotNetTest.Unit
{
    [TestFixture]
    class TestConfiguration
    {

        [Test]
        public void TestConfigurationFromFile()
        {
            Console.WriteLine(Directory.GetCurrentDirectory());
            string filename = "TestConfigurationFromFile";
            StreamWriter writer = new StreamWriter(File.Create(filename));
            writer.WriteLine("username=  username");
            writer.WriteLine("  password  =  password");
            writer.WriteLine("merchantId=merchantId  ");
            writer.WriteLine("host= host  ");
            writer.WriteLine("  downloadDirectory=downloadDirectory  ");
            writer.WriteLine("printXml  =false");
            writer.WriteLine(" proxyHost= proxyHost");
            writer.WriteLine(" proxyPort  =proxyPort");
            writer.Close();
            
            Configuration config = new Configuration(filename);
            
            Assert.AreEqual("username", config.getConfig("username"));
            Assert.AreEqual("password", config.getConfig("password"));
            Assert.AreEqual("merchantId", config.getConfig("merchantId"));
            Assert.AreEqual("host", config.getConfig("host"));
            Assert.AreEqual("downloadDirectory", config.getConfig("downloadDirectory"));
            Assert.AreEqual("false", config.getConfig("printXml"));
            Assert.AreEqual("proxyHost", config.getConfig("proxyHost"));
            Assert.AreEqual("proxyPort", config.getConfig("proxyPort"));
            
            File.Delete(filename);
        }

        [Test]
        public void TestConfigurationFromCustomDictionary()
        {
            Dictionary<string, string> configDict = new Dictionary<string, string>();
            configDict["username"] = "username";
            configDict["password"] = "password";
            configDict["merchantId"] = "merchantId";
            configDict["host"] = "host";
            configDict["downloadDirectory"] = "downloadDirectory";
            configDict["printXml"] = "false";
            configDict["proxyHost"] = "proxyHost";
            configDict["proxyPort"] = "proxyPort";
            
            Configuration config = new Configuration(configDict);
            
            Assert.AreEqual("username", config.getConfig("username"));
            Assert.AreEqual("password", config.getConfig("password"));
            Assert.AreEqual("merchantId", config.getConfig("merchantId"));
            Assert.AreEqual("host", config.getConfig("host"));
            Assert.AreEqual("downloadDirectory", config.getConfig("downloadDirectory"));
            Assert.AreEqual("false", config.getConfig("printXml"));
            Assert.AreEqual("proxyHost", config.getConfig("proxyHost"));
            Assert.AreEqual("proxyPort", config.getConfig("proxyPort"));
        }

        [Test]
        public void TestConfigurationDefault()
        {
            string filename = "TestConfigurationDefault";
            StreamWriter writer = new StreamWriter(File.Create(filename));
            writer.WriteLine("username=username");
            writer.WriteLine("password=password");
            writer.WriteLine("merchantId=merchantId  ");
            writer.WriteLine("host=host");
            writer.WriteLine("downloadDirectory=downloadDirectory");
            writer.WriteLine("printXml=false");
            writer.WriteLine("proxyHost=proxyHost");
            writer.WriteLine("proxyPort=proxyPort");
            writer.WriteLine("Call Vantiv!!!");
            writer.Close();
            string originalConfigPath = Environment.GetEnvironmentVariable("configPath");
            Environment.SetEnvironmentVariable("configPath", Path.Combine(Directory.GetCurrentDirectory(), filename));

            try
            {
                Configuration config = new Configuration();

                Assert.AreEqual("username", config.getConfig("username"));
                Assert.AreEqual("password", config.getConfig("password"));
                Assert.AreEqual("merchantId", config.getConfig("merchantId"));
                Assert.AreEqual("host", config.getConfig("host"));
                Assert.AreEqual("downloadDirectory", config.getConfig("downloadDirectory"));
                Assert.AreEqual("false", config.getConfig("printXml"));
                Assert.AreEqual("proxyHost", config.getConfig("proxyHost"));
                Assert.AreEqual("proxyPort", config.getConfig("proxyPort"));

            }
            catch (ChargebackException ce)
            {
                throw new ChargebackException("" + ce);
            }
            finally
            {
                Console.WriteLine("Restoring env var and deleting file...");
                Environment.SetEnvironmentVariable("configPath", originalConfigPath);
                File.Delete(filename);
            }
        }

        [Test]
        [ExpectedException(typeof(ChargebackException))]
        public void TestMissingSettingInConfig()
        {
            Dictionary<string, string> configDict = new Dictionary<string, string>();
            configDict["username"] = "username";
            configDict["password"] = "password";
            configDict["host"] = "host";
            configDict["downloadDirectory"] = "downloadDirectory";
            configDict["printXml"] = "false";
            configDict["proxyHost"] = "proxyHost";
            configDict["proxyPort"] = "proxyPort";
            Configuration config = new Configuration(configDict);
            
        }

        [Test]
        [ExpectedException(typeof(KeyNotFoundException))]
        public void TestInvalidSettingInConfig()
        {
            Dictionary<string, string> configDict = new Dictionary<string, string>();
            configDict["username"] = "username";
            configDict["password"] = "password";
            configDict["merchantId"] = "merchantId";
            configDict["host"] = "host";
            configDict["downloadDirectory"] = "downloadDirectory";
            configDict["printXml"] = "false";
            configDict["proxyHost"] = "proxyHost";
            configDict["proxyPort"] = "proxyPort";
            configDict["extraKey"] = "extraValue";
            Configuration config = new Configuration(configDict);
            string extraKey = config.getConfig("extraKey");
        }

        [Test]
        public void TestPasswordWithEqualsSignInConfigFile()
        {
            Console.WriteLine(Directory.GetCurrentDirectory());
            string filename = "TestConfigurationFromFile";
            StreamWriter writer = new StreamWriter(File.Create(filename));
            writer.WriteLine("username=username");
            writer.WriteLine("password=pass=word");
            writer.WriteLine("merchantId=merchantId  ");
            writer.WriteLine("host=host");
            writer.WriteLine("downloadDirectory=downloadDirectory");
            writer.WriteLine("printXml=false");
            writer.WriteLine("proxyHost=proxyHost");
            writer.WriteLine("proxyPort=proxyPort");
            writer.Close();
            
            Configuration config = new Configuration(filename);
            
            Assert.AreEqual("pass=word", config.getConfig("password"));
            
            File.Delete(filename);
        }
    }
}
