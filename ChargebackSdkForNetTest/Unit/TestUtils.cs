using System;
using System.IO;
using System.Linq;
using System.Text;
using ChargebackSdkForNet;
using Xunit;

namespace ChargebackSdkForNetTest.Unit
{
    public class TestUtils
    {
        [Theory]
        [InlineData("@Hello World!")]
        [InlineData("0123456789")]
        [InlineData("empty        spaces")]
        public void TestBytesToString(string testString)
        {
            var bytes = Encoding.ASCII.GetBytes(testString).ToList();
            var result = ChargebackUtils.BytesToString(bytes);
            Assert.Equal(testString, result);
        }
        
        [Fact]
        public void TestStringToBytes()
        {
            const string helloWorld = "@Hello World!";
            var bytes = Encoding.ASCII.GetBytes(helloWorld).ToList();
            var resultBytes = ChargebackUtils.StringToBytes(helloWorld);
            for(var i = 0; i < bytes.Count; i++)
            {
                Assert.Equal(bytes[i], resultBytes[i]);
            }
        }
        
        [Fact]
        public void TestBytesToFile()
        {
            const int size1Kb = 1024;
            var bytes = new byte[size1Kb];
            const string fileName = "TestBytesToFile.testFile";
            var f = File.Create(fileName);
            f.Write(bytes, 0, bytes.Length);
            f.Close();
            Assert.True(File.Exists(fileName));
            Assert.Equal(size1Kb, (new FileInfo(fileName)).Length);
            File.Delete(fileName);
        }
        
        [Theory]
        [InlineData("abcdef123456!@#$%^ABCDEF")]
        [InlineData("this password should be 64 encoded")]
        public void TestEncode64(string password)
        {
            var expectedEncodedPassword = Convert.ToBase64String(Encoding.GetEncoding("utf-8").GetBytes(password));
            Assert.Equal(expectedEncodedPassword, ChargebackUtils.Encode64(password, "utf-8"));
        }
        
        [Fact]
        public void TestDeserializeResponse()
        {
            
        }
    }
}