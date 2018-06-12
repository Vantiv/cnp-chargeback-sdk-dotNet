using System;
using System.Collections.Generic;
using ChargebackSdkForNet;
using ChargebackSdkForNet.Properties;
using NUnit.Framework;

namespace ChargebackSdkForNetTest.Functional
{
    [TestFixture]
    public class TestChargebackrRetrieval
    {
        private ChargebackRetrievalRequest _retrievalRequest;

        [TestFixtureSetUp]
        public void SetUp()
        {

            _retrievalRequest = new ChargebackRetrievalRequest();
        }

        [Test]
        public void TestChargebackRetrievalDate()
        {
            var retrievalResponse = 
                _retrievalRequest.RetrieveByActivityDate(new DateTime(2013, 1, 1));
            Assert.NotNull(retrievalResponse);
            Assert.NotNull(retrievalResponse.transactionId);
            Assert.NotNull(retrievalResponse.chargebackCase);
        }

        [Test]
        public void TestRetrieveByActivityDateWithFinancialImpact()
        {
            var retrievalResponse = 
                _retrievalRequest.RetrieveByActivityDateWithImpact(new DateTime(2013, 1, 1), true);
            Assert.NotNull(retrievalResponse);
            Assert.NotNull(retrievalResponse.transactionId);
            Assert.NotNull(retrievalResponse.chargebackCase);
        }
        
        [Test]
        public void TestRetrieveActionable()
        {
            var retrievalResponse = _retrievalRequest.RetrieveByActionable(true);
            Assert.NotNull(retrievalResponse);
            Assert.NotNull(retrievalResponse.transactionId);
            Assert.NotNull(retrievalResponse.chargebackCase);
        }
            
        [Test]
        public void TestRetrieveByCaseId()
        {
            var retrievalResponse = _retrievalRequest.RetrieveByCaseId(1288791001);
            Assert.NotNull(retrievalResponse);
            Assert.NotNull(retrievalResponse.transactionId);
            Assert.NotNull(retrievalResponse.chargebackCase);
        }
            
        [Test]
        public void TestRetrieveByCardNumber()
        {
            var retrievalResponse = 
                _retrievalRequest.RetrieveByCardNumber(2222222222222222222.ToString(), 2, 1998);
            Assert.NotNull(retrievalResponse);
            Assert.NotNull(retrievalResponse.transactionId);
            Assert.NotNull(retrievalResponse.chargebackCase);
        }
            
        [Test]
        public void TestRetrieveByArn()
        {
            var retrievalResponse = _retrievalRequest.RetrieveByArn(5555555551.ToString());
            Assert.NotNull(retrievalResponse);
            Assert.NotNull(retrievalResponse.transactionId);
            Assert.NotNull(retrievalResponse.chargebackCase);
        }
    }
}
