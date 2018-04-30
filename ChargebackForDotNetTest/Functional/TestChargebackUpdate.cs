using System;
using System.Collections.Generic;
using ChargebackForDotNet;
using ChargebackForDotNet.Properties;
using NUnit.Framework;

namespace ChargebackForDotNetTest.Functional
{
    [TestFixture]
    public class TestChargebackUpdate
    {
        private ChargebackUpdateRequest _updateRequest;

        [TestFixtureSetUp]
        public void SetUp()
        {
            var configDict = new Dictionary<string, string>();
            configDict["username"] = "dotnet";
            configDict["password"] = "dotnet";
            configDict["merchantId"] = "101";
            configDict["host"] = "https://www.testvantivcnp.com/sandbox/new/services";
            configDict["printXml"] = "true";
            configDict["neuterXml"] = "false";
            configDict["proxyHost"] = "websenseproxy";
            configDict["proxyPort"] = "8080";
            var config = new Configuration(configDict);

            _updateRequest = new ChargebackUpdateRequest {Config = config};
        }

        [Test]
        public void TestAssignToUser()
        {
            var updateResponse = _updateRequest.AssignToUser(1234, "user@vantiv.com", "Assigning to user");
            Assert.NotNull(updateResponse);
            Assert.IsInstanceOf<chargebackUpdateResponse>(updateResponse);
            Assert.NotNull(updateResponse.transactionId);
        }
        
        [Test]
        public void TestAddNote()
        {
            const long caseId = 1000;
            var updateResponse = _updateRequest.AddNote(caseId, "Any note");
            Assert.NotNull(updateResponse);
            Assert.IsInstanceOf<chargebackUpdateResponse>(updateResponse);
            Assert.NotNull(updateResponse.transactionId);
        }

        [Test]
        public void TestAcceptLiability()
        {
            const long caseId = 1000;
            var updateResponse = _updateRequest.AcceptLiability(caseId, "Accepting Liability");
            Assert.NotNull(updateResponse);
            Assert.IsInstanceOf<chargebackUpdateResponse>(updateResponse);
            Assert.NotNull(updateResponse.transactionId);
        }

        [Test]
        public void TestRepresentWithRepresentedAmount()
        {
            const long caseId = 1000;
            const long amount = 5000;
            var updateResponse = _updateRequest.Represent(caseId, amount, "Represinting with an amount");
            Assert.NotNull(updateResponse);
            Assert.IsInstanceOf<chargebackUpdateResponse>(updateResponse);
            Assert.NotNull(updateResponse.transactionId);
        }

        [Test]
        public void TestRepresentWithoutRepresenedAmount()
        {
            const long caseId = 1000;
            var updateResponse = _updateRequest.Represent(caseId, "Representig without an amount");
            Assert.NotNull(updateResponse);
            Assert.IsInstanceOf<chargebackUpdateResponse>(updateResponse);
            Assert.NotNull(updateResponse.transactionId);
        }

        [Test]
        public void TestRespond()
        {
            const long caseId = 1000;
            var updateResponse = _updateRequest.Respond(caseId, "Respond to chargeback case");
            Assert.NotNull(updateResponse);
            Assert.IsInstanceOf<chargebackUpdateResponse>(updateResponse);
            Assert.NotNull(updateResponse.transactionId);
        }

        [Test]
        public void TestRequestArbitration()
        {
            const long caseId = 1000;
            var updateResponse = _updateRequest.RequestArbitration(caseId, "Requestinng arbitration...");
            Assert.NotNull(updateResponse);
            Assert.IsInstanceOf<chargebackUpdateResponse>(updateResponse);
            Assert.NotNull(updateResponse.transactionId);
        }

        [Test]
        public void TestErrorResponse404()
        {
            const long caseId = 1404;
            try
            {
                _updateRequest.AddNote(caseId, "Any note");
                Assert.Fail("ChargebackException was expected with HTTP 404 Error. None thrown");
            }
            catch (ChargebackWebException ce)
            {
                Assert.True(ce.ErrorMessage.Contains("Update Failed - HTTP 404 Error"));
                Assert.AreEqual("Could not find requested object.", ce.ErrorMessages[0]);
            }
        }
        
        [Test]
        public void TestErrorResponse400()
        {
            const long caseId = 1400;
            try
            {
                _updateRequest.AddNote(caseId, "Any note");
                Assert.Fail("ChargebackException was expected with HTTP 400 Error. None thrown");
            }
            catch (ChargebackWebException ce)
            {
                Console.BackgroundColor = ConsoleColor.DarkGreen;
                Assert.True(ce.ErrorMessage.Contains("Update Failed - HTTP 400 Error"));
                
                Assert.AreEqual("Bad Request. Error in request. " +
                                "Poorly formatted xml or cannot perform activity.", ce.ErrorMessages[0]);
            }
        }
        
        [Test]
        public void TestErrorResponse401()
        {
            const long caseId = 1401;
            try
            {
                _updateRequest.AddNote(caseId, "Any note");
                Assert.Fail("ChargebackException was expected with HTTP 401 Error. None thrown");
            }
            catch (ChargebackWebException ce)
            {
                Assert.True(ce.ErrorMessage.Contains("Update Failed - HTTP 401 Error"));
                Assert.AreEqual("You are not authorized to access this resource. Please check your credentials.", ce.ErrorMessages[0]);
            }
        }
        
        [Test]
        public void TestErrorResponse403()
        {
            const long caseId = 1403;
            try
            {
                _updateRequest.AddNote(caseId, "Any note");
                Assert.Fail("ChargebackException was expected with HTTP 403 Error. None thrown");
            }
            catch (ChargebackWebException ce)
            {
                Assert.True(ce.ErrorMessage.Contains("Update Failed - HTTP 403 Error"));
                Assert.AreEqual("You are not authorized to access this resource. Please check your credentials.", ce.ErrorMessages[0]);
            }
        }
        
        [Test]
        public void TestErrorResponse500()
        {
            const long caseId = 1500;
            try
            {
                _updateRequest.AddNote(caseId, "Any note");
                Assert.Fail("ChargebackException was expected with HTTP 500 Error. None thrown");
            }
            catch (ChargebackWebException ce)
            {
                Assert.True(ce.ErrorMessage.Contains("Update Failed - HTTP 500 Error"));
                Assert.AreEqual("Internal Error. This error has already been escalated to Vantiv for resolution. " +
                                "Please contact support with questions.", ce.ErrorMessages[0]);                  
            }
        }
    }
}