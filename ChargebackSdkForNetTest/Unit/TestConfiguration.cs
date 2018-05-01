using System;
using System.Collections.Generic;
using System.IO;
using ChargebackSdkForNet;
using ChargebackSdkForNet.Properties;
using NUnit.Framework;

namespace ChargebackSdkForNetTest.Unit
{
    [TestFixture]
    internal class TestConfiguration
    {

        [Test]
        public void TestConfigurationFromFile()
        {
            Console.WriteLine(Directory.GetCurrentDirectory());
            const string filename = "TestConfigurationFromFile";
            var writer = new StreamWriter(File.Create(filename));
            writer.WriteLine("username=  username");
            writer.WriteLine("  password  =  password");
            writer.WriteLine("merchantId=merchantId  ");
            writer.WriteLine("host= host  ");
            writer.WriteLine("printXml  =false");
            writer.WriteLine("neuterXml = false");
            writer.WriteLine(" proxyHost= proxyHost");
            writer.WriteLine(" proxyPort  =proxyPort");
            writer.Close();
            
            var config = new Configuration(filename);
            
            Assert.AreEqual("username", config.Get("username"));
            Assert.AreEqual("password", config.Get("password"));
            Assert.AreEqual("merchantId", config.Get("merchantId"));
            Assert.AreEqual("host", config.Get("host"));
            Assert.AreEqual("false", config.Get("printXml"));
            Assert.AreEqual("false", config.Get("neuterXml"));
            Assert.AreEqual("proxyHost", config.Get("proxyHost"));
            Assert.AreEqual("proxyPort", config.Get("proxyPort"));
            
            File.Delete(filename);
        }

        [Test]
        public void TestConfigurationFromCustomDictionary()
        {
            var configDict = new Dictionary<string, string>();
            configDict["username"] = "username";
            configDict["password"] = "password";
            configDict["merchantId"] = "merchantId";
            configDict["host"] = "host";
            configDict["printXml"] = "false";
            configDict["neuterXml"] = "false";
            configDict["proxyHost"] = "proxyHost";
            configDict["proxyPort"] = "proxyPort";
            
            var config = new Configuration(configDict);
            
            Assert.AreEqual("username", config.Get("username"));
            Assert.AreEqual("password", config.Get("password"));
            Assert.AreEqual("merchantId", config.Get("merchantId"));
            Assert.AreEqual("host", config.Get("host"));
            Assert.AreEqual("false", config.Get("printXml"));
            Assert.AreEqual("false", config.Get("neuterXml"));
            Assert.AreEqual("proxyHost", config.Get("proxyHost"));
            Assert.AreEqual("proxyPort", config.Get("proxyPort"));
        }

        [Test]
        [ExpectedException(typeof(ChargebackException))]
        public void TestMissingSettingInConfig()
        {
            var configDict = new Dictionary<string, string>();
            configDict["username"] = "username";
            configDict["password"] = "password";
            configDict["host"] = "host";
            configDict["printXml"] = "false";
            configDict["neuterXml"] = "false";
            configDict["proxyHost"] = "proxyHost";
            configDict["proxyPort"] = "proxyPort";
            var configuration = new Configuration(configDict);
        }

        [Test]
        [ExpectedException(typeof(KeyNotFoundException))]
        public void TestInvalidSettingInConfig()
        {
            var configDict = new Dictionary<string, string>();
            configDict["username"] = "username";
            configDict["password"] = "password";
            configDict["merchantId"] = "merchantId";
            configDict["host"] = "host";
            configDict["printXml"] = "false";
            configDict["neuterXml"] = "false";
            configDict["proxyHost"] = "proxyHost";
            configDict["proxyPort"] = "proxyPort";
            configDict["extraKey"] = "extraValue";
            var config = new Configuration(configDict);
            var extraKey = config.Get("extraKey");
        }

        [Test]
        public void TestPasswordWithEqualsSignInConfigFile()
        {
            Console.WriteLine(Directory.GetCurrentDirectory());
            const string filename = "TestConfigurationFromFile";
            var writer = new StreamWriter(File.Create(filename));
            writer.WriteLine("username=username");
            writer.WriteLine("password=pass=word");
            writer.WriteLine("merchantId=merchantId  ");
            writer.WriteLine("host=host");
            writer.WriteLine("printXml=false");
            writer.WriteLine("neuterXml=false");
            writer.WriteLine("proxyHost=proxyHost");
            writer.WriteLine("proxyPort=proxyPort");
            writer.Close();
            
            var config = new Configuration(filename);
            
            Assert.AreEqual("pass=word", config.Get("password"));
            
            File.Delete(filename);
        }
    }
}
