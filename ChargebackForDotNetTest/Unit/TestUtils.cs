using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using ChargebackForDotNet;
using NUnit.Framework;

namespace ChargebackForDotNetTest.Unit
{
    [TestFixture]
    public class TestUtils
    {
        [TestFixtureSetUp]
        public void SetUp()
        {
            
        }
        
        [TestCase("@Hello World!")]
        [TestCase("0123456789")]
        [TestCase("empty        spaces")]
        public void TestBytesToString(string testString)
        {
            List<byte> bytes = Encoding.ASCII.GetBytes(testString).ToList();
            string result = ChargebackUtils.BytesToString(bytes);
            Assert.AreEqual(testString, result);
        }
        
        [Test]
        public void TestStringToBytes()
        {
            string helloWorld = "@Hello World!";
            List<byte> bytes = Encoding.ASCII.GetBytes(helloWorld).ToList();
            List<byte> resultBytes = ChargebackUtils.StringToBytes(helloWorld);
            for(var i = 0; i < bytes.Count; i++)
            {
                Assert.AreEqual(bytes[i], resultBytes[i]);
            }
        }
        
        [Test]
        public void TestBytesToFile()
        {
            int _1Kb = 1024;
            byte[] bytes = new byte[_1Kb];
            string fileName = "TestBytesToFile.testFile";
            FileStream f = File.Create(fileName);
            f.Write(bytes, 0, bytes.Length);
            f.Close();
            Assert.IsTrue(File.Exists(fileName));
            Assert.AreEqual(_1Kb, (new FileInfo(fileName)).Length);
            File.Delete(fileName);
        }
        
        [TestCase("abcdef123456!@#$%^ABCDEF")]
        [TestCase("this password should be 64 encoded")]
        public void TestEncode64(string password)
        {
            string expectedEncodedPassword = Convert.ToBase64String(Encoding.GetEncoding("utf-8").GetBytes(password));
            Assert.AreEqual(expectedEncodedPassword, ChargebackUtils.Encode64(password, "utf-8"));
        }
        
        [Test]
        public void TestDeserializeResponse()
        {
            
        }
    }
}