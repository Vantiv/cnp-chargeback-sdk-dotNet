using System;
using System.Collections.Generic;
using ChargebackSdkForNet;
using ChargebackSdkForNet.Properties;
using Xunit;

namespace ChargebackSdkForNetTest.Functional
{
    public class TestChargebackrRetrieval
    {
        private ChargebackRetrievalRequest _retrievalRequest;

        public TestChargebackrRetrieval()
        {
            _retrievalRequest = new ChargebackRetrievalRequest();
        }

        [Fact]
        public void TestChargebackRetrievalDate()
        {
            var retrievalResponse = 
                _retrievalRequest.RetrieveByActivityDate(new DateTime(2013, 1, 1));
            Assert.NotNull(retrievalResponse);
            Assert.NotNull(retrievalResponse.transactionId);
            Assert.NotNull(retrievalResponse.chargebackCase);
        }

        [Fact]
        public void TestRetrieveByActivityDateWithFinancialImpact()
        {
            var retrievalResponse = 
                _retrievalRequest.RetrieveByActivityDateWithImpact(new DateTime(2013, 1, 1), true);
            Assert.NotNull(retrievalResponse);
            Assert.NotNull(retrievalResponse.transactionId);
            Assert.NotNull(retrievalResponse.chargebackCase);
        }
        
        [Fact]
        public void TestRetrieveActionable()
        {
            var retrievalResponse = _retrievalRequest.RetrieveByActionable(true);
            Assert.NotNull(retrievalResponse);
            Assert.NotNull(retrievalResponse.transactionId);
            Assert.NotNull(retrievalResponse.chargebackCase);
        }
            
        [Fact]
        public void TestRetrieveByCaseId()
        {
            var retrievalResponse = _retrievalRequest.RetrieveByCaseId(1288791001);
            Assert.NotNull(retrievalResponse);
            Assert.NotNull(retrievalResponse.transactionId);
            Assert.NotNull(retrievalResponse.chargebackCase);
        }
            
        [Fact]
        public void TestRetrieveByCardNumber()
        {
            var retrievalResponse = 
                _retrievalRequest.RetrieveByCardNumber(2222222222222222222.ToString(), 2, 1998);
            Assert.NotNull(retrievalResponse);
            Assert.NotNull(retrievalResponse.transactionId);
            Assert.NotNull(retrievalResponse.chargebackCase);
        }
            
        [Fact]
        public void TestRetrieveByArn()
        {
            var retrievalResponse = _retrievalRequest.RetrieveByArn(5555555551.ToString());
            Assert.NotNull(retrievalResponse);
            Assert.NotNull(retrievalResponse.transactionId);
            Assert.NotNull(retrievalResponse.chargebackCase);
        }
    }
}
