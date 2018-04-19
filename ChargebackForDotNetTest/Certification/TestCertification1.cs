using System;
using System.Linq;
using System.Runtime.InteropServices;
using ChargebackForDotNet;
using ChargebackForDotNet.Properties;
using NUnit.Framework;

namespace ChargebackForDotNetTest.Certification
{
    [TestFixture]
    public class TestCertification1
    {
        [Test]
        public void Test2_1_1RetrieveByAParticularDate()
        {
            Configuration conf = new Configuration();
            ChargebackRetrievalRequest request = new ChargebackRetrievalRequest();
            request.config = conf;
            chargebackRetrievalResponse response = request.retrieveByActivityDate(new DateTime(2013, 1, 1));
            Assert.NotNull(response);
            Assert.AreEqual(11, response.chargebackCase.Length);
            chargebackApiCase[] cases = response.chargebackCase;
            Assert.AreEqual("1111111111", cases[0].acquirerReferenceNumber);
            Assert.AreEqual("2222222222", cases[1].acquirerReferenceNumber);
            Assert.AreEqual("3333333333", cases[2].acquirerReferenceNumber);
            Assert.AreEqual("4444444444", cases[3].acquirerReferenceNumber);
            Assert.AreEqual("5555555550", cases[4].acquirerReferenceNumber);
            Assert.AreEqual("5555555551", cases[5].acquirerReferenceNumber);
            Assert.AreEqual("5555555552", cases[6].acquirerReferenceNumber);
            Assert.AreEqual("6666666660", cases[7].acquirerReferenceNumber);
            Assert.AreEqual("7777777770", cases[8].acquirerReferenceNumber);
            Assert.AreEqual("7777777771", cases[9].acquirerReferenceNumber);
            Assert.AreEqual("7777777772", cases[10].acquirerReferenceNumber);
/*            Assert.AreEqual("FIRST_CHARGBACK", cases[0].cycle);
            Assert.AreEqual("FIRST_CHARGBACK", cases[1].cycle);
            Assert.AreEqual("FIRST_CHARGBACK", cases[2].cycle);
            Assert.AreEqual("FIRST_CHARGBACK", cases[3].cycle);
            Assert.AreEqual("PRE_ARB_CHARGBACK", cases[4].cycle);
            Assert.AreEqual("PRE_ARB_CHARGBACK", cases[5].cycle);
            Assert.AreEqual("PRE_ARB_CHARGBACK", cases[6].cycle);
            Assert.AreEqual("ARBITRATION_CHARGEBACK", cases[7].cycle);
            Assert.AreEqual("ISSUER_DECLINE_PRESAB", cases[8].cycle);
            Assert.AreEqual("ISSUER_DECLINE_PRESAB", cases[9].cycle);
            Assert.AreEqual("ISSUER_DECLINE_PRESAB", cases[10].cycle);*/
        }

        [Test]
        public void Test2_1_2AddNote()
        {
            Configuration conf = new Configuration();
            
            // Step 1. Perform a new activity for the case retrieved from ARN 1111111111.
            ChargebackRetrievalRequest retrievalRequest = new ChargebackRetrievalRequest();
            retrievalRequest.config = conf;
            chargebackRetrievalResponse retrievalResponse = retrievalRequest.retrieveByArn("1111111111");
            long caseId = retrievalResponse.chargebackCase[0].caseId;
            
            chargebackUpdateRequest updateRequest = new NoteRequest(caseId, "Call Vantiv!");
            updateRequest.config = conf;
            chargebackUpdateResponse updateResponse = updateRequest.sendUpdateRequest();
            Assert.NotNull(updateResponse);
            
            // Step 2. Verify that the new activity has been appended to the activity list for the case.
            retrievalResponse = retrievalRequest.retrieveByArn("1111111111");
            chargebackApiActivity[] activities = retrievalResponse.chargebackCase[0].activity;
            Assert.AreEqual("Add Note", activities[activities.Length - 1].activityType);
            string notes = activities[activities.Length - 1].notes;
            Assert.AreEqual("Call Vantiv!", notes);
        }
        
        [Test]
        public void Test2_1_3RequestRepresentment()
        {
            Configuration conf = new Configuration();
            // Step 1. Perform a new activity for the case retrieved from ARN 2222222222.
            ChargebackRetrievalRequest retrievalRequest = new ChargebackRetrievalRequest();
            retrievalRequest.config = conf;
            chargebackRetrievalResponse retrievalResponse = retrievalRequest.retrieveByArn("2222222222");
            long caseId = retrievalResponse.chargebackCase[0].caseId;
            chargebackUpdateRequest updateRequest = new MerchantRepresent(caseId, "Test2_1_3RequestRepresentment");
            updateRequest.config = conf;
            chargebackUpdateResponse updateResponse = updateRequest.sendUpdateRequest();
            
            // Step 2. Verify that the new activity has been appended to the activity list for the case.
            retrievalResponse = retrievalRequest.retrieveByArn("2222222222");
            chargebackApiActivity[] activities = retrievalResponse.chargebackCase[0].activity;
            Assert.AreEqual(activityType.MERCHANT_REPRESENT, activities[activities.Length - 1].activityType);
            
            // Step 3. Perform a new activity for the case retrieved from ARN 3333333333.
            retrievalResponse = retrievalRequest.retrieveByArn("3333333333");
            caseId = retrievalResponse.chargebackCase[0].caseId;
            updateRequest = new MerchantRepresent(caseId, 10027, "Test2_1_3RequestRepresentment");
            updateResponse = updateRequest.sendUpdateRequest();
            
            // Step 4. Verify that the new activity has been appended to the activity list for the case.
            retrievalResponse = retrievalRequest.retrieveByArn("3333333333");
            activities = retrievalResponse.chargebackCase[0].activity;
            Assert.AreEqual(activityType.MERCHANT_REPRESENT, activities[activities.Length - 1].activityType);
        }
        
        [Test]
        public void Test2_1_4AssumingLiability()
        {
            Configuration conf = new Configuration();
            // Step 1. Perform a new activity for the case retrieved from ARN 4444444444.
            ChargebackRetrievalRequest retrievalRequest = new ChargebackRetrievalRequest();
            retrievalRequest.config = conf;
            chargebackRetrievalResponse retrievalResponse = retrievalRequest.retrieveByArn("4444444444");
            long caseId = retrievalResponse.chargebackCase[0].caseId;

            chargebackUpdateRequest updateRequest = new MerchantAcceptsLiability(caseId, "Test2_1_4AssumingLiability");
            updateRequest.config = conf;
            
            // Step 2. Verify that the new activity has been appended to the activity list for the case.
            retrievalResponse = retrievalRequest.retrieveByArn("4444444444");
            chargebackApiActivity[] activities = retrievalResponse.chargebackCase[0].activity;
            Assert.AreEqual(activityType.MERCHANT_ACCEPTS_LIABILITY, activities[activities.Length - 1].activityType);
        }

        [Test]
        public void Test2_1_5ErrorMessage()
        {
            Configuration conf = new Configuration();
            ChargebackRetrievalRequest retrievalRequest = new ChargebackRetrievalRequest();
            retrievalRequest.config = conf;
            chargebackRetrievalResponse retrievalResponse = retrievalRequest.retrieveByArn("4444444444");
            long caseId = retrievalResponse.chargebackCase[0].caseId;

            chargebackUpdateRequest updateRequest = new MerchantAcceptsLiability(caseId, "Test2_1_5ErrorMessage");
            updateRequest.config = conf;
            try
            {
                updateRequest.sendUpdateRequest();
            }
            catch(ChargebackException ce)
            {
                Assert.AreEqual("Cannot perform activity <Merchant Accepts Liability> " +
                                "for case <"+caseId+"> in queue <Merchant Assumed>", ce.errors[0]);
                
            }
        }
    }
}