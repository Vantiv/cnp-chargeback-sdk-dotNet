﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using ChargebackForDotNet;
using ChargebackForDotNet.Properties;
using NUnit.Framework;

namespace ChargebackForDotNetTest.Functional
{
    [TestFixture]
    public class TestChargebackrRetrieval
    {
        private ChargebackRetrievalRequest retrievalRequest;

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
            
            retrievalRequest = new ChargebackRetrievalRequest();
            retrievalRequest.Config = config;
        }

        [Test]
        public void TestChargebackRetrievalDate()
        {
            chargebackRetrievalResponse retrievalResponse = 
                retrievalRequest.RetrieveByActivityDate(new DateTime(2013, 1, 1));
            Assert.NotNull(retrievalResponse);
            Assert.NotNull(retrievalResponse.transactionId);
            Assert.NotNull(retrievalResponse.chargebackCase);
        }

        [Test]
        public void TestRetrieveByActivityDateWithFinancialImpact()
        {
            chargebackRetrievalResponse retrievalResponse = 
                retrievalRequest.RetrieveByActivityDateWithImpact(new DateTime(2013, 1, 1), true);
            Assert.NotNull(retrievalResponse);
            Assert.NotNull(retrievalResponse.transactionId);
            Assert.NotNull(retrievalResponse.chargebackCase);
        }
        
        [Test]
        public void TestRetrieveActionable()
        {
            chargebackRetrievalResponse retrievalResponse = retrievalRequest.RetrieveByActionable(true);
            Assert.NotNull(retrievalResponse);
            Assert.NotNull(retrievalResponse.transactionId);
            Assert.NotNull(retrievalResponse.chargebackCase);
        }
            
        [Test]
        public void TestRetrieveByCaseId()
        {
            chargebackRetrievalResponse retrievalResponse = retrievalRequest.RetrieveByCaseId(1288791001);
            Assert.NotNull(retrievalResponse);
            Assert.NotNull(retrievalResponse.transactionId);
            Assert.NotNull(retrievalResponse.chargebackCase);
        }
            
        [Test]
        public void TestRetrieveByCardNumber()
        {
            chargebackRetrievalResponse retrievalResponse = 
                retrievalRequest.RetrieveByCardNumber(0001.ToString(), 2, 1998);
            Assert.NotNull(retrievalResponse);
            Assert.NotNull(retrievalResponse.transactionId);
            Assert.NotNull(retrievalResponse.chargebackCase);
        }
            
        [Test]
        public void TestRetrieveByArn()
        {
            chargebackRetrievalResponse retrievalResponse = retrievalRequest.RetrieveByArn(5555555551.ToString());
            Assert.NotNull(retrievalResponse);
            Assert.NotNull(retrievalResponse.transactionId);
            Assert.NotNull(retrievalResponse.chargebackCase);
        }
    }
}