using System.Text;
using ChargebackSdkForNet;
using Moq;
using NUnit.Framework;

namespace ChargebackSdkForNetTest.Unit
{
    [TestFixture]
    public class TestChargebackRetrieval
    {
            
        [SetUp]
        public void SetUp()
        {
        }

        private string GenerateXmlResponse(int transactionId, int nCases)
        {
            var head = string.Format(
                "<?xml version=\"1.0\" encoding=\"UTF-8\" standalone=\"yes\"?>" +
                "<chargebackRetrievalResponse xmlns=\"http://www.vantivcnp.com/chargebacks\">" +
                "    <transactionId>{0}</transactionId>", transactionId);
            const string body = "    <chargebackCase>" +
                                "        <caseId>12887910010</caseId>" +
                                "        <merchantId>1288791</merchantId>" +
                                "        <dayIssuedByBank>2002-01-01</dayIssuedByBank>" +
                                "        <dateReceivedByVantivCnp>2018-04-12</dateReceivedByVantivCnp>" +
                                "        <vantivCnpTxnId>2110834816113</vantivCnpTxnId>" +
                                "        <cycle>Issuer Declined Pre-Arbitration</cycle>" +
                                "        <orderId>12345</orderId>" +
                                "        <cardNumberLast4>0001</cardNumberLast4>" +
                                "        <cardType>VISA</cardType>" +
                                "        <chargebackAmount>71000</chargebackAmount>" +
                                "        <chargebackCurrencyType>USD</chargebackCurrencyType>" +
                                "        <originalTxnDay>2002-01-01</originalTxnDay>" +
                                "        <chargebackType>D</chargebackType>" +
                                "        <reasonCode>11.3</reasonCode>" +
                                "        <reasonCodeDescription>Allocation Flow - Authorization - No Authorization</reasonCodeDescription>" +
                                "        <currentQueue>Merchant</currentQueue>" +
                                "        <acquirerReferenceNumber>7777777771</acquirerReferenceNumber>" +
                                "        <chargebackReferenceNumber>jjjjjjjjjj</chargebackReferenceNumber>" +
                                "        <bin>410000</bin>" +
                                "        <paymentAmount>4510</paymentAmount>" +
                                "        <activity>" +
                                "            <activityDate>2018-04-12</activityDate>" +
                                "            <activityType>Assign To Merchant</activityType>" +
                                "            <fromQueue>Vantiv</fromQueue>" +
                                "            <toQueue>Merchant Automated</toQueue>" +
                                "            <notes>Please work this case</notes>" +
                                "        </activity>" +
                                "    </chargebackCase>";
            const string foot = "</chargebackRetrievalResponse>";
            var xmlResponse = new StringBuilder(head);
            for (var i = 0; i < nCases; i++)
            {
                xmlResponse.Append(body);
            }

            xmlResponse.Append(foot);
            return xmlResponse.ToString();
        }

        [TestCase("1234-12-09", 1234567890, 1, false)]
        [TestCase("1253-12-09", 2117084919, 2, false)]
        [TestCase("1253-04-09", 1559459335, 1, false)]
        [TestCase("2018-04-09", 2111123987, 0, true)]
        public void TestRetrieveByActivityDate(string date, int expectedId, int expectedNCases, bool expectedNull)
        {
            var expectedXmlResponse = GenerateXmlResponse(expectedId, expectedNCases);
            var expectedResponseContent = new ResponseContent(
                "text/xml", ChargebackUtils.StringToBytes(expectedXmlResponse));
            var commMock = new Mock<Communication>();
            commMock.Setup(c => c.Get("/chargebacks/?date=" + date))
                .Returns(expectedResponseContent);
            var request
                = new ChargebackRetrievalRequest(commMock.Object);
            var response
                = request.RetrieveByActivityDate(ChargebackUtils.ParseDate(date));
            Assert.AreEqual(expectedId, response.transactionId);
            var nullCase = response.chargebackCase == null;
            Assert.AreEqual(expectedNull, nullCase);
            if(!nullCase)
                Assert.AreEqual(expectedNCases, response.chargebackCase.Length);
        }

        [TestCase("2018-04-22", true, 1234567, 3, false)]
        [TestCase("2018-04-22", false, 1234567, 1, false)]
        [TestCase("2018-04-28", true, 1234567, 0, true)]
        public void TestRetrieveByActivityDateWithImpact(string date, bool impact,
            int expectedId, int expectedNCases, bool expectedNull)
        {
            var expectedXmlResponse = GenerateXmlResponse(expectedId, expectedNCases);
            var expectedResponseContent = new ResponseContent(
                "text/xml", ChargebackUtils.StringToBytes(expectedXmlResponse));
            var commMock = new Mock<Communication>();
            commMock.Setup(c => c.Get(string.Format("/chargebacks/?date={0}&financialOnly={1}", date, impact)))
                .Returns(expectedResponseContent);
            var request
                = new ChargebackRetrievalRequest(commMock.Object);
            var response
                = request.RetrieveByActivityDateWithImpact(ChargebackUtils.ParseDate(date), impact);
            Assert.AreEqual(expectedId, response.transactionId);
            var nullCase = response.chargebackCase == null;
            Assert.AreEqual(expectedNull, nullCase);
            if(!nullCase)
                Assert.AreEqual(expectedNCases, response.chargebackCase.Length);
        }

        [TestCase(true, 1234567, 3, false)]
        [TestCase(true, 1234567, 1, false)]
        [TestCase(false, 1234567, 0, true)]
        public void TestRetrieveByActionable(bool actionable, int expectedId, int expectedNCases,
            bool expectedNull)
        {
            var expectedXmlResponse = GenerateXmlResponse(expectedId, expectedNCases);
            var expectedResponseContent = new ResponseContent(
                "text/xml", ChargebackUtils.StringToBytes(expectedXmlResponse));
            var commMock = new Mock<Communication>();
            commMock.Setup(c => c.Get(string.Format("/chargebacks/?actionable={0}", actionable)))
                .Returns(expectedResponseContent);
            var request
                = new ChargebackRetrievalRequest(commMock.Object);
            var response
                = request.RetrieveByActionable(actionable);
            Assert.AreEqual(expectedId, response.transactionId);
            var nullCase = response.chargebackCase == null;
            Assert.AreEqual(expectedNull, nullCase);
            if(!nullCase)
                Assert.AreEqual(expectedNCases, response.chargebackCase.Length);
        }

        
        [TestCase(11111, 1234567, 3, false)]
        [TestCase(22222, 1234567, 1, false)]
        [TestCase(33333, 1234567, 0, true)]
        public void TestRetrieveByCaseId(long caseId, int expectedId, int expectedNCases,
            bool expectedNull)
        {
            var expectedXmlResponse = GenerateXmlResponse(expectedId, expectedNCases);
            var expectedResponseContent = new ResponseContent(
                "text/xml", ChargebackUtils.StringToBytes(expectedXmlResponse));
            var commMock = new Mock<Communication>();
            commMock.Setup(c => c.Get(string.Format("/chargebacks/{0}", caseId)))
                .Returns(expectedResponseContent);
            var request
                = new ChargebackRetrievalRequest(commMock.Object);
            var response
                = request.RetrieveByCaseId(caseId);
            Assert.AreEqual(expectedId, response.transactionId);
            var nullCase = response.chargebackCase == null;
            Assert.AreEqual(expectedNull, nullCase);
            if(!nullCase)
                Assert.AreEqual(expectedNCases, response.chargebackCase.Length);
        }

        [TestCase("12asde589", 1234567, 3, false)]
        [TestCase("12autde5801", 1234567, 1, false)]
        [TestCase("dfhkfhiwrjfjg65jg6j1f6ftj165", 1234567, 0, true)]
        public void TestRetrieveByToken(string token, int expectedId, int expectedNCases,
            bool expectedNull)
        {
            var expectedXmlResponse = GenerateXmlResponse(expectedId, expectedNCases);
            var expectedResponseContent = new ResponseContent(
                "text/xml", ChargebackUtils.StringToBytes(expectedXmlResponse));
            var commMock = new Mock<Communication>();
            commMock.Setup(c => c.Get(string.Format("/chargebacks/?token={0}", token)))
                .Returns(expectedResponseContent);
            var request
                = new ChargebackRetrievalRequest(commMock.Object);
            var response
                = request.RetrieveByToken(token);
            Assert.AreEqual(expectedId, response.transactionId);
            var nullCase = response.chargebackCase == null;
            Assert.AreEqual(expectedNull, nullCase);
            if(!nullCase)
                Assert.AreEqual(expectedNCases, response.chargebackCase.Length);
        }

        [TestCase("1234566375237", "2018-04-1", 1234567, 3, false)]
        [TestCase("1234784575237", "2018-04-1", 1234567, 1, false)]
        [TestCase("11111111111111","2018-02-1", 1234567, 0, true)]
        public void TestRetrieveByCardNumber(string cardNumber, string expirationDate,
            int expectedId, int expectedNCases, bool expectedNull)
        {
            var expectedXmlResponse = GenerateXmlResponse(expectedId, expectedNCases);
            var expectedResponseContent = new ResponseContent(
                "text/xml", ChargebackUtils.StringToBytes(expectedXmlResponse));
            var commMock = new Mock<Communication>();
            var cardExpirationdate = ChargebackUtils.ParseDate(expirationDate);
            var stringCardExpirationdate = cardExpirationdate.ToString("MMyy");
            string expectedQuery =
                string.Format("/chargebacks/?cardNumber={0}&expirationDate={1}", cardNumber, stringCardExpirationdate);
            commMock.Setup(c => c.Get(expectedQuery))
                .Returns(expectedResponseContent);
            var request
                = new ChargebackRetrievalRequest(commMock.Object);
            var response
                = request.RetrieveByCardNumber(cardNumber, cardExpirationdate.Month, cardExpirationdate.Year);
            Assert.AreEqual(expectedId, response.transactionId);
            var nullCase = response.chargebackCase == null;
            Assert.AreEqual(expectedNull, nullCase);
            if(!nullCase)
                Assert.AreEqual(expectedNCases, response.chargebackCase.Length);
        }

        [TestCase("111111111", 1234567, 3, false)]
        [TestCase("222222222", 1234567, 1, false)]
        [TestCase("333333333", 1234567, 0, true)]
        public void TestRetrieveByArn(string arn, int expectedId, int expectedNCases, bool expectedNull)
        {
            var expectedXmlResponse = GenerateXmlResponse(expectedId, expectedNCases);
            var expectedResponseContent = new ResponseContent(
                "text/xml", ChargebackUtils.StringToBytes(expectedXmlResponse));
            var commMock = new Mock<Communication>();
            commMock.Setup(c => c.Get(string.Format("/chargebacks/?arn={0}", arn)))
                .Returns(expectedResponseContent);
            var request
                = new ChargebackRetrievalRequest(commMock.Object);
            var response
                = request.RetrieveByArn(arn);
            Assert.AreEqual(expectedId, response.transactionId);
            var nullCase = response.chargebackCase == null;
            Assert.AreEqual(expectedNull, nullCase);
            if(!nullCase)
                Assert.AreEqual(expectedNCases, response.chargebackCase.Length);
        }
    }
}