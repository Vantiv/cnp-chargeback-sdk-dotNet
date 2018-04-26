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
            
            updateRequest = new chargebackUpdateRequest();
            updateRequest.config = config;
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
            long caseId = 1000;
            var updateResponse = updateRequest.AddNote(caseId, "Any note");
            Assert.NotNull(updateResponse);
            Assert.IsInstanceOf<chargebackUpdateResponse>(updateResponse);
            Assert.NotNull(updateResponse.transactionId);
        }

        [Test]
        public void TestAcceptLiability()
        {
            long caseId = 1000;
            var updateResponse = updateRequest.AcceptLiability(caseId, "Accepting Liability");
            Assert.NotNull(updateResponse);
            Assert.IsInstanceOf<chargebackUpdateResponse>(updateResponse);
            Assert.NotNull(updateResponse.transactionId);
        }

        [Test]
        public void TestRepresentWithRepresentedAmount()
        {
            long caseId = 1000;
            long amount = 5000;
            var updateResponse = updateRequest.Represent(caseId, amount, "Represinting with an amount");
            Assert.NotNull(updateResponse);
            Assert.IsInstanceOf<chargebackUpdateResponse>(updateResponse);
            Assert.NotNull(updateResponse.transactionId);
        }

        [Test]
        public void TestRepresentWithoutRepresenedAmount()
        {
            long caseId = 1000;
            var updateResponse = updateRequest.Represent(caseId, "Representig without an amount");
            Assert.NotNull(updateResponse);
            Assert.IsInstanceOf<chargebackUpdateResponse>(updateResponse);
            Assert.NotNull(updateResponse.transactionId);
        }

        [Test]
        public void TestRespond()
        {
            long caseId = 1000;
            var updateResponse = updateRequest.Respond(caseId, "Respond to chargeback case");
            Assert.NotNull(updateResponse);
            Assert.IsInstanceOf<chargebackUpdateResponse>(updateResponse);
            Assert.NotNull(updateResponse.transactionId);
        }

        [Test]
        public void TestRequestArbitration()
        {
            long caseId = 1000;
            var updateResponse = updateRequest.RequestArbitration(caseId, "Requestinng arbitration...");
            Assert.NotNull(updateResponse);
            Assert.IsInstanceOf<chargebackUpdateResponse>(updateResponse);
            Assert.NotNull(updateResponse.transactionId);
        }

        [Test]
        public void TestErrorResponse404()
        {
            long caseId = 1404;
            try
            {
                var response = updateRequest.AddNote(caseId, "Any note");
                Assert.Fail("ChargebackException was expected with HTTP 404 Error. None thrown");
            }
            catch (ChargebackException ce)
            {
                Assert.True(ce.Message.Contains("Update Failed - HTTP 404 Error"));
                Assert.IsTrue(ce.Message.Contains("Could not find requested object."));
            }
        }
        
        [Test]
        public void TestErrorResponse400()
        {
            long caseId = 1400;
            try
            {
                var response = updateRequest.AddNote(caseId, "Any note");
                Assert.Fail("ChargebackException was expected with HTTP 400 Error. None thrown");
            }
            catch (ChargebackException ce)
            {
                Console.BackgroundColor = ConsoleColor.DarkGreen;
                Assert.True(ce.Message.Contains("Update Failed - HTTP 400 Error"));
                string pattern = @".*Poorly formatted xml or cannot perform activity.*";
                Regex regex = new Regex(pattern, RegexOptions.IgnoreCase);
                Assert.True(ce.Message.Contains("Poorly formatted xml or cannot perform activity."));
            }
        }
        
        [Test]
        public void TestErrorResponse401()
        {
            long caseId = 1401;
            try
            {
                var response = updateRequest.AddNote(caseId, "Any note");
                Assert.Fail("ChargebackException was expected with HTTP 401 Error. None thrown");
            }
            catch (ChargebackException ce)
            {
                Assert.True(ce.Message.Contains("Update Failed - HTTP 401 Error"));
                Assert.True(ce.Message.Contains("You are not authorized to access this resource. Please check your credentials."));
            }
        }
        
        [Test]
        public void TestErrorResponse403()
        {
            long caseId = 1403;
            try
            {
                var response = updateRequest.AddNote(caseId, "Any note");
                Assert.Fail("ChargebackException was expected with HTTP 403 Error. None thrown");
            }
            catch (ChargebackException ce)
            {
                Assert.True(ce.Message.Contains("Update Failed - HTTP 403 Error"));
                Assert.True(ce.Message.Contains("You are not authorized to access this resource. Please check your credentials."));
            }
        }
        
        [Test]
        public void TestErrorResponse500()
        {
            long caseId = 1500;
            try
            {
                var response = updateRequest.AddNote(caseId, "Any note");
                Assert.Fail("ChargebackException was expected with HTTP 500 Error. None thrown");
            }
            catch (ChargebackException ce)
            {
                Assert.True(ce.Message.Contains("Update Failed - HTTP 500 Error"));
                Assert.IsTrue(ce.Message.Contains("Internal Error. This error has already been escalated to Vantiv for resolution. " +
                                "Please contact support with questions."));                  
            }
        }
    }
}