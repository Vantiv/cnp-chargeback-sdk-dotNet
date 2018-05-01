using System;
using System.IO;
using System.Linq;
using System.Text;
using ChargebackSdkForNet;
using NUnit.Framework;

namespace ChargebackSdkForNetTest.Unit
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
            var bytes = Encoding.ASCII.GetBytes(testString).ToList();
            var result = ChargebackUtils.BytesToString(bytes);
            Assert.AreEqual(testString, result);
        }
        
        [Test]
        public void TestStringToBytes()
        {
            const string helloWorld = "@Hello World!";
            var bytes = Encoding.ASCII.GetBytes(helloWorld).ToList();
            var resultBytes = ChargebackUtils.StringToBytes(helloWorld);
            for(var i = 0; i < bytes.Count; i++)
            {
                Assert.AreEqual(bytes[i], resultBytes[i]);
            }
        }
        
        [Test]
        public void TestBytesToFile()
        {
            const int size1Kb = 1024;
            var bytes = new byte[size1Kb];
            const string fileName = "TestBytesToFile.testFile";
            var f = File.Create(fileName);
            f.Write(bytes, 0, bytes.Length);
            f.Close();
            Assert.IsTrue(File.Exists(fileName));
            Assert.AreEqual(size1Kb, (new FileInfo(fileName)).Length);
            File.Delete(fileName);
        }
        
        [TestCase("abcdef123456!@#$%^ABCDEF")]
        [TestCase("this password should be 64 encoded")]
        public void TestEncode64(string password)
        {
            var expectedEncodedPassword = Convert.ToBase64String(Encoding.GetEncoding("utf-8").GetBytes(password));
            Assert.AreEqual(expectedEncodedPassword, ChargebackUtils.Encode64(password, "utf-8"));
        }
        
        [Test]
        public void TestDeserializeResponse()
        {
            
        }
    }
}