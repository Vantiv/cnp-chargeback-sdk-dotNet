using System;
using System.Collections.Generic;
using System.IO;
using ChargebackSdkForNet;
using ChargebackSdkForNet.Properties;
using Xunit;

namespace ChargebackSdkForNetTest.Unit
{
    public class TestConfiguration
    {

        [Fact]
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
            
            Assert.Equal("username", config.Get("username"));
            Assert.Equal("password", config.Get("password"));
            Assert.Equal("merchantId", config.Get("merchantId"));
            Assert.Equal("host", config.Get("host"));
            Assert.Equal("false", config.Get("printXml"));
            Assert.Equal("false", config.Get("neuterXml"));
            Assert.Equal("proxyHost", config.Get("proxyHost"));
            Assert.Equal("proxyPort", config.Get("proxyPort"));
            
            File.Delete(filename);
        }

        [Fact]
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
            
            Assert.Equal("username", config.Get("username"));
            Assert.Equal("password", config.Get("password"));
            Assert.Equal("merchantId", config.Get("merchantId"));
            Assert.Equal("host", config.Get("host"));
            Assert.Equal("false", config.Get("printXml"));
            Assert.Equal("false", config.Get("neuterXml"));
            Assert.Equal("proxyHost", config.Get("proxyHost"));
            Assert.Equal("proxyPort", config.Get("proxyPort"));
        }

        [Fact]
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
            Exception ex = Assert.Throws<ChargebackException>(() => new Configuration(configDict));
        }

        [Fact]
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
            Exception ex = Assert.Throws<KeyNotFoundException>(() => config.Get("extraKey"));
        }

        [Fact]
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
            
            Assert.Equal("pass=word", config.Get("password"));
            
            File.Delete(filename);
        }
    }
}
