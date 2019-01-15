using System;
using System.Collections.Generic;
using ChargebackSdkForNet;
using ChargebackSdkForNet.Properties;
using Xunit;

namespace ChargebackSdkForNetTest.Functional
{
    public class TestChargebackUpdate
    {
        private ChargebackUpdateRequest _updateRequest;

        public TestChargebackUpdate()
        {

            _updateRequest = new ChargebackUpdateRequest();

        }

        [Fact]
        public void TestAssignToUser()
        {
            var updateResponse = _updateRequest.AssignToUser(1234, "user@vantiv.com", "Assigning to user");
            Assert.NotNull(updateResponse);
            Assert.IsType<chargebackUpdateResponse>(updateResponse);
            Assert.NotNull(updateResponse.transactionId);
        }
        
        [Fact]
        public void TestAddNote()
        {
            const long caseId = 1000;
            var updateResponse = _updateRequest.AddNote(caseId, "Any note");
            Assert.NotNull(updateResponse);
            Assert.IsType<chargebackUpdateResponse>(updateResponse);
            Assert.NotNull(updateResponse.transactionId);
        }

        [Fact]
        public void TestAcceptLiability()
        {
            const long caseId = 1000;
            var updateResponse = _updateRequest.AcceptLiability(caseId, "Accepting Liability");
            Assert.NotNull(updateResponse);
            Assert.IsType<chargebackUpdateResponse>(updateResponse);
            Assert.NotNull(updateResponse.transactionId);
        }

        [Fact]
        public void TestRepresentWithRepresentedAmount()
        {
            const long caseId = 1000;
            const long amount = 5000;
            var updateResponse = _updateRequest.Represent(caseId, amount, "Represinting with an amount");
            Assert.NotNull(updateResponse);
            Assert.IsType<chargebackUpdateResponse>(updateResponse);
            Assert.NotNull(updateResponse.transactionId);
        }

        [Fact]
        public void TestRepresentWithoutRepresenedAmount()
        {
            const long caseId = 1000;
            var updateResponse = _updateRequest.Represent(caseId, "Representig without an amount");
            Assert.NotNull(updateResponse);
            Assert.IsType<chargebackUpdateResponse>(updateResponse);
            Assert.NotNull(updateResponse.transactionId);
        }

        [Fact]
        public void TestRespond()
        {
            const long caseId = 1000;
            var updateResponse = _updateRequest.Respond(caseId, "Respond to chargeback case");
            Assert.NotNull(updateResponse);
            Assert.IsType<chargebackUpdateResponse>(updateResponse);
            Assert.NotNull(updateResponse.transactionId);
        }

        [Fact]
        public void TestRequestArbitration()
        {
            const long caseId = 1000;
            var updateResponse = _updateRequest.RequestArbitration(caseId, "Requestinng arbitration...");
            Assert.NotNull(updateResponse);
            Assert.IsType<chargebackUpdateResponse>(updateResponse);
            Assert.NotNull(updateResponse.transactionId);
        }

        [Fact]
        public void TestErrorResponse404()
        {
            const long caseId = 1404;
            try
            {
                _updateRequest.AddNote(caseId, "Any note");
                Assert.True(false, "ChargebackException was expected with HTTP 404 Error. None thrown");
            }
            catch (ChargebackWebException ce)
            {
                Assert.True(ce.ErrorMessage.Contains("Update Failed - HTTP 404 Error"));
                Assert.Equal("Could not find requested object.", ce.ErrorMessages[0]);
            }
        }
        
        [Fact]
        public void TestErrorResponse400()
        {
            const long caseId = 1400;
            try
            {
                _updateRequest.AddNote(caseId, "Any note");
                Assert.True(false, "ChargebackException was expected with HTTP 400 Error. None thrown");
            }
            catch (ChargebackWebException ce)
            {
                Console.BackgroundColor = ConsoleColor.DarkGreen;
                Assert.True(ce.ErrorMessage.Contains("Update Failed - HTTP 400 Error"));
                
                Assert.Equal("Bad Request. Error in request. " +
                                "Poorly formatted header/xml or cannot perform activity.", ce.ErrorMessages[0]);
            }
        }
        
        [Fact]
        public void TestErrorResponse401()
        {
            const long caseId = 1401;
            try
            {
                _updateRequest.AddNote(caseId, "Any note");
                Assert.True(false, "ChargebackException was expected with HTTP 401 Error. None thrown");
            }
            catch (ChargebackWebException ce)
            {
                Assert.True(ce.ErrorMessage.Contains("Update Failed - HTTP 401 Error"));
                Assert.Equal("You are not authorized to access this resource. Please check your credentials.", ce.ErrorMessages[0]);
            }
        }
        
        [Fact]
        public void TestErrorResponse403()
        {
            const long caseId = 1403;
            try
            {
                _updateRequest.AddNote(caseId, "Any note");
                Assert.True(false, "ChargebackException was expected with HTTP 403 Error. None thrown");
            }
            catch (ChargebackWebException ce)
            {
                Assert.True(ce.ErrorMessage.Contains("Update Failed - HTTP 403 Error"));
                Assert.Equal("You are not authorized to access this resource. Please check your credentials.", ce.ErrorMessages[0]);
            }
        }
        
        [Fact]
        public void TestErrorResponse500()
        {
            const long caseId = 1500;
            try
            {
                _updateRequest.AddNote(caseId, "Any note");
                Assert.True(false, "ChargebackException was expected with HTTP 500 Error. None thrown");
            }
            catch (ChargebackWebException ce)
            {
                Assert.True(ce.ErrorMessage.Contains("Update Failed - HTTP 500 Error"));
                Assert.Equal("Internal Error. This error has already been escalated to Vantiv for resolution. " +
                                "Please contact support with questions.", ce.ErrorMessages[0]);                  
            }
        }
    }
}
