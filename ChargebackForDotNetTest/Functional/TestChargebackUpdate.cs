using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using ChargebackForDotNet;
using ChargebackForDotNet.Properties;
using NUnit.Framework;

namespace ChargebackForDotNetTest.Functional
{
    [TestFixture]
    public class TestChargebackUpdate
    {
        private chargebackUpdateRequest updateRequest;

        [TestFixtureSetUp]
        public void SetUp()
        {
            Dictionary<string, string> configDict = new Dictionary<string, string>();
            configDict["username"] = "dotnet";
            configDict["password"] = "dotnet";
            configDict["merchantId"] = "101";
            configDict["host"] = "https://www.testvantivcnp.com/sandbox/new/services";
            configDict["downloadDirectory"] = "downloadDirectory";
            configDict["printXml"] = "true";
            configDict["proxyHost"] = "websenseproxy";
            configDict["proxyPort"] = "8080";
            Configuration config = new Configuration(configDict);
            
            updateRequest = new chargebackUpdateRequest(config);
        }

        [Test]
        public void TestAssignToUser()
        {
            var updateResponse = updateRequest.AssignToUser(1234, "user@vantiv.com", "Assigning to user");
            Assert.NotNull(updateResponse);
            Assert.IsInstanceOf<chargebackUpdateResponse>(updateResponse);
            Assert.NotNull(updateResponse.transactionId);
        }
        
        [Test]
        public void TestAddNote()
        {
            var updateResponse = updateRequest.AddNote(1234, "Any note");
            Assert.NotNull(updateResponse);
            Assert.IsInstanceOf<chargebackUpdateResponse>(updateResponse);
            Assert.NotNull(updateResponse.transactionId);
        }

        [Test]
        public void TestAcceptLiability()
        {
            var updateResponse = updateRequest.AcceptLiability(1234, "Accepting Liability");
            Assert.NotNull(updateResponse);
            Assert.IsInstanceOf<chargebackUpdateResponse>(updateResponse);
            Assert.NotNull(updateResponse.transactionId);
        }

        [Test]
        public void TestRepresentWithRepresentedAmount()
        {
            var updateResponse = updateRequest.Represent(1234, 1000, "Represinting with an amount");
            Assert.NotNull(updateResponse);
            Assert.IsInstanceOf<chargebackUpdateResponse>(updateResponse);
            Assert.NotNull(updateResponse.transactionId);
        }

        [Test]
        public void TestRepresentWithoutRepresenedAmount()
        {
            var updateResponse = updateRequest.Represent(1234, 1000, "Representig without an amount");
            Assert.NotNull(updateResponse);
            Assert.IsInstanceOf<chargebackUpdateResponse>(updateResponse);
            Assert.NotNull(updateResponse.transactionId);
        }

        [Test]
        public void TestRespond()
        {
            var updateResponse = updateRequest.Respond(1234, "Respond to chargeback case");
            Assert.NotNull(updateResponse);
            Assert.IsInstanceOf<chargebackUpdateResponse>(updateResponse);
            Assert.NotNull(updateResponse.transactionId);
        }

        [Test]
        public void TestRequestArbitration()
        {
            var updateResponse = updateRequest.RequestArbitration(1234, "Requestinng arbitration...");
            Assert.NotNull(updateResponse);
            Assert.IsInstanceOf<chargebackUpdateResponse>(updateResponse);
            Assert.NotNull(updateResponse.transactionId);
        }

        [Test]
        public void TestErrorResponse404()
        {
            try
            {
                var response = updateRequest.AddNote(1404, "Any note");
                Assert.Fail("ChargebackException was expected with HTTP 404 Error. None thrown");
            }
            catch (ChargebackException ce)
            {
                Assert.True(ce.Message.Contains("Update Failed - HTTP 404 Error"));
                Assert.AreEqual("Could not find requested object.", ce.errors[0]);
            }
        }
        
        [Test]
        public void TestErrorResponse400()
        {
            try
            {
                var response = updateRequest.AddNote(1400, "Any note");
                Assert.Fail("ChargebackException was expected with HTTP 400 Error. None thrown");
            }
            catch (ChargebackException ce)
            {
                Assert.True(ce.Message.Contains("Update Failed - HTTP 400 Error"));
                string pattern = @"Cannot perform activity.*Merchant Accepts Liability.*Merchant Assumed.*";
                Regex regex = new Regex(pattern, RegexOptions.IgnoreCase);
                Assert.True(regex.IsMatch(ce.errors[0]));
            }
        }
        
        [Test]
        public void TestErrorResponse401()
        {
            try
            {
                var response = updateRequest.AddNote(1401, "Any note");
                Assert.Fail("ChargebackException was expected with HTTP 401 Error. None thrown");
            }
            catch (ChargebackException ce)
            {
                Assert.True(ce.Message.Contains("Update Failed - HTTP 401 Error"));
                Assert.AreEqual("You are not authorized to access this resource. Please check your credentials.",
                    ce.errors[0]);
            }
        }
        
        [Test]
        public void TestErrorResponse403()
        {
            try
            {
                var response = updateRequest.AddNote(1403, "Any note");
                Assert.Fail("ChargebackException was expected with HTTP 403 Error. None thrown");
            }
            catch (ChargebackException ce)
            {
                Assert.True(ce.Message.Contains("Update Failed - HTTP 403 Error"));
                Assert.AreEqual("You are not authorized to access this resource. Please check your credentials.",
                    ce.errors[0]);
            }
        }
        
        [Test]
        public void TestErrorResponse500()
        {
            try
            {
                var response = updateRequest.AddNote(1500, "Any note");
                Assert.Fail("ChargebackException was expected with HTTP 500 Error. None thrown");
            }
            catch (ChargebackException ce)
            {
                Assert.True(ce.Message.Contains("Update Failed - HTTP 500 Error"));
                Assert.AreEqual("Internal Error. This error has already been escalated to Vantiv for resolution. " +
                                "Please contact support with questions.", ce.errors[0]);                  
            }
        }
    }
}