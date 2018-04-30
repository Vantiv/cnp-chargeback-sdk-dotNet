using System.Collections;
using System.Text;
using ChargebackForDotNet;
using Moq;
using NUnit.Framework;

namespace ChargebackForDotNetTest.Unit
{
    [TestFixture]
    public class TestChargebackUpdate
    {
        [SetUp]
        public void SetUp()
        {
            
        }

        private string generateXmlRequest(activityType activityType, string assignedTo, string note,
            bool representedAmountFieldSpecified = false, long representedAmount = 0)
        {
            var header = "<?xml version=\"1.0\" encoding=\"UTF-8\" standalone=\"yes\"?>" +
                            "\n<chargebackUpdateRequest xmlns=\"http://www.vantivcnp.com/chargebacks\">";
            var footer = "\n</chargebackUpdateRequest>";
            var body = "";
            
            body += string.Format("\n<activityType>{0}</activityType>", activityType);
            if (assignedTo != null)
            {
                body += string.Format("\n<assignedTo>{0}</assignedTo>", assignedTo);
            }

            if (note != null)
            {
                body += string.Format("\n<note>{0}</note>", note);
            }

            if (representedAmountFieldSpecified)
            {
                body += string.Format("\n<representedAmount>{0}</representedAmount>",representedAmount);
            }
            return header+body+footer;
        }

        private string generateXmlResponse(int transactionId)
        {
            var head = string.Format(
                "<?xml version=\"1.0\" encoding=\"UTF-8\" standalone=\"yes\"?>" +
                "<chargebackUpdateResponse xmlns=\"http://www.vantivcnp.com/chargebacks\">" +
                "    <transactionId>{0}</transactionId>", transactionId);
            var foot =
                "</chargebackUpdateResponse>";

            return head + foot;
        }

        private string generateErrorXmlResponse(string[] errorMessages)
        {
            var head =
                "<?xml version=\"1.0\" encoding=\"UTF-8\" standalone=\"yes\"?>" +
                "<errorResponse>" +
                "<errors>";
            var foot =
                "</errors>" +
                "</errorResponse>";
            var xmlResponse = new StringBuilder(head);
            foreach (var error in errorMessages)
            {
                xmlResponse.Append("<error>" + error + "</error>");
            }

            xmlResponse.Append(foot);
            return xmlResponse.ToString();
        }

        [TestCase(1000, "test1@assign.com.com", "Assign to test1", 1234567)]
        [TestCase(1000, "test2@assign.com.com", null, 4568778)]
        public void TestAssignToUser(long caseId, string assignedTo, string note, int expectedTxnId)
        {
            var expectedXmlResponse = generateXmlResponse(expectedTxnId);
            var expectedXmlRequest = generateXmlRequest(activityType.ASSIGN_TO_USER, assignedTo, note);
            var expectedSendingBytes = ChargebackUtils.StringToBytes(expectedXmlRequest);
            var expectedResponseContent = new ResponseContent(
                "text/xml", ChargebackUtils.StringToBytes(expectedXmlResponse));
            Mock<Communication> commMock = new Mock<Communication>();
            commMock.Setup(c => c.Put("/chargebacks/" + caseId, expectedSendingBytes))
                .Returns(expectedResponseContent);
            var request
                = new chargebackUpdateRequest(commMock.Object);
            var response
                = request.AssignToUser(caseId, assignedTo, note);
            Assert.AreEqual(expectedTxnId, response.transactionId);
        }
        
        
        [TestCase(1000, "Note test1", 1234567)]
        public void TestAddNote(long caseId, string note, int expectedTxnId)
        {
            var expectedXmlResponse = generateXmlResponse(expectedTxnId);
            var expectedXmlRequest = generateXmlRequest(activityType.ADD_NOTE, null, note);
            var expectedSendingBytes = ChargebackUtils.StringToBytes(expectedXmlRequest);
            var expectedResponseContent = new ResponseContent(
                "text/xml", ChargebackUtils.StringToBytes(expectedXmlResponse));
            Mock<Communication> commMock = new Mock<Communication>();
            commMock.Setup(c => c.Put("/chargebacks/" + caseId, expectedSendingBytes))
                .Returns(expectedResponseContent);
            var request
                = new chargebackUpdateRequest(commMock.Object);
            var response
                = request.AddNote(caseId, note);
            Assert.AreEqual(expectedTxnId, response.transactionId);
        }
        
        
        [TestCase(1000, "Accept liability test1", 1234567)]
        [TestCase(1000, null, 4568778)]
        public void TestAcceptLiability(long caseId, string note, int expectedTxnId)
        {
            var expectedXmlResponse = generateXmlResponse(expectedTxnId);
            var expectedXmlRequest = generateXmlRequest(activityType.MERCHANT_ACCEPTS_LIABILITY, null, note);
            var expectedSendingBytes = ChargebackUtils.StringToBytes(expectedXmlRequest);
            var expectedResponseContent = new ResponseContent(
                "text/xml", ChargebackUtils.StringToBytes(expectedXmlResponse));
            Mock<Communication> commMock = new Mock<Communication>();
            commMock.Setup(c => c.Put("/chargebacks/" + caseId, expectedSendingBytes))
                .Returns(expectedResponseContent);
            var request
                = new chargebackUpdateRequest(commMock.Object);
            var response
                = request.AcceptLiability(caseId, note);
            Assert.AreEqual(expectedTxnId, response.transactionId);
        }
        
        
        [TestCase(1000, 5000, "Merchant represent with amount test1", 1234567)]
        [TestCase(1000, 2050, null, 4568778)]
        public void TestRepresentWithRepresentedAmount(long caseId, long representedAmount, string note, int expectedTxnId)
        {
            var expectedXmlResponse = generateXmlResponse(expectedTxnId);
            var expectedXmlRequest 
                = generateXmlRequest(activityType.MERCHANT_REPRESENT, null, note, true, representedAmount);
            var expectedSendingBytes = ChargebackUtils.StringToBytes(expectedXmlRequest);
            var expectedResponseContent = new ResponseContent(
                "text/xml", ChargebackUtils.StringToBytes(expectedXmlResponse));
            Mock<Communication> commMock = new Mock<Communication>();
            commMock.Setup(c => c.Put("/chargebacks/" + caseId, expectedSendingBytes))
                .Returns(expectedResponseContent);
            var request
                = new chargebackUpdateRequest(commMock.Object);
            var response
                = request.Represent(caseId, representedAmount, note);
            Assert.AreEqual(expectedTxnId, response.transactionId);
        }
        
        
        [TestCase(1000, "Merchant represent without amount test1", 1234567)]
        [TestCase(1000, null, 4568778)]
        public void TestRepresentWithoutRepresenedAmount(long caseId, string note, int expectedTxnId)
        {
            var expectedXmlResponse = generateXmlResponse(expectedTxnId);
            var expectedXmlRequest 
                = generateXmlRequest(activityType.MERCHANT_REPRESENT, null, note);
            var expectedSendingBytes = ChargebackUtils.StringToBytes(expectedXmlRequest);
            var expectedResponseContent = new ResponseContent(
                "text/xml", ChargebackUtils.StringToBytes(expectedXmlResponse));
            Mock<Communication> commMock = new Mock<Communication>();
            commMock.Setup(c => c.Put("/chargebacks/" + caseId, expectedSendingBytes))
                .Returns(expectedResponseContent);
            var request
                = new chargebackUpdateRequest(commMock.Object);
            var response
                = request.Represent(caseId, note);
            Assert.AreEqual(expectedTxnId, response.transactionId);
        }


        [TestCase(1000, "Merchant respond test1", 1234567)]
        [TestCase(1000, null, 4568778)]
        public void TestRespond(long caseId, string note, int expectedTxnId)
        {
            var expectedXmlResponse = generateXmlResponse(expectedTxnId);
            var expectedXmlRequest 
                = generateXmlRequest(activityType.MERCHANT_RESPOND, null, note);
            var expectedSendingBytes = ChargebackUtils.StringToBytes(expectedXmlRequest);
            var expectedResponseContent = new ResponseContent(
                "text/xml",ChargebackUtils.StringToBytes(expectedXmlResponse));
            Mock<Communication> commMock = new Mock<Communication>();
            commMock.Setup(c => c.Put("/chargebacks/" + caseId, expectedSendingBytes))
                .Returns(expectedResponseContent);
            var request
                = new chargebackUpdateRequest(commMock.Object);
            var response
                = request.Respond(caseId, note);
            Assert.AreEqual(expectedTxnId, response.transactionId);
        }
        
        
        [TestCase(1000, "Merchant requests arbitration test1", 1234567)]
        [TestCase(1000, null, 4568778)]
        public void TestRequestArbitration(long caseId, string note, int expectedTxnId)
        {
            var expectedXmlResponse = generateXmlResponse(expectedTxnId);
            var expectedXmlRequest 
                = generateXmlRequest(activityType.MERCHANT_REQUESTS_ARBITRATION, null, note);
            var expectedSendingBytes = ChargebackUtils.StringToBytes(expectedXmlRequest);
            var expectedResponseContent = new ResponseContent(
                "text/xml", ChargebackUtils.StringToBytes(expectedXmlResponse));
            Mock<Communication> commMock = new Mock<Communication>();
            commMock.Setup(c => c.Put("/chargebacks/" + caseId, expectedSendingBytes))
                .Returns(expectedResponseContent);
            var request
                = new chargebackUpdateRequest(commMock.Object);
            var response
                = request.RequestArbitration(caseId, note);
            Assert.AreEqual(expectedTxnId, response.transactionId);
        }
    }
}