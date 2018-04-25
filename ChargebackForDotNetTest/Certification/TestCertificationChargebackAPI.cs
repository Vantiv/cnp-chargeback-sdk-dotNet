using System;
using System.Linq;
using System.Runtime.InteropServices;
using ChargebackForDotNet;
using ChargebackForDotNet.Properties;
using NUnit.Framework;

namespace ChargebackForDotNetTest.Certification
{
    [TestFixture]
    public class TestCertificationChargebackAPI
    {
        [Test]
        public void Test_2_1_1_RetrieveByAParticularDate()
        {
            ChargebackRetrievalRequest request = new ChargebackRetrievalRequest();
            chargebackRetrievalResponse response = request.RetrieveByActivityDate(ChargebackUtils.ParseDate("2013-1-1"));
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
        public void Test_2_1_2_AddNote()
        {
            // Step 1. Perform a new activity for the case retrieved from ARN 1111111111.
            ChargebackRetrievalRequest retrievalRequest = new ChargebackRetrievalRequest();
            chargebackRetrievalResponse retrievalResponse = retrievalRequest.RetrieveByArn("1111111111");
            long caseId = retrievalResponse.chargebackCase[0].caseId;
            
            chargebackUpdateRequest updateRequest = new chargebackUpdateRequest();
            long milSec = DateTime.Now.Millisecond;
            string expectedNote = milSec + " Call Vantiv!";
            chargebackUpdateResponse updateResponse = updateRequest.AddNote(caseId, expectedNote);
            Assert.NotNull(updateResponse);
            
            // Step 2. Verify that the new activity has been appended to the activity list for the case.
            retrievalResponse = retrievalRequest.RetrieveByArn("1111111111");
            chargebackApiActivity[] activities = retrievalResponse.chargebackCase[0].activity;
            foreach (var activity in activities)
            {
                if (expectedNote.Equals(activity.notes))
                {
                    Assert.AreEqual("Add Note", activity.activityType);
                    Assert.Pass();
                }
            }
            Assert.Fail();
        }
        
        [Test]
        [Ignore("Cannot run multiple times on Prelive.")]
        public void Test_2_1_3_RequestRepresentment()
        {
            // Step 1. Perform a new activity for the case retrieved from ARN 2222222222.
            ChargebackRetrievalRequest retrievalRequest = new ChargebackRetrievalRequest();
            chargebackRetrievalResponse retrievalResponse = retrievalRequest.RetrieveByArn("2222222222");
            long caseId = retrievalResponse.chargebackCase[0].caseId;
            chargebackUpdateRequest updateRequest = new chargebackUpdateRequest();
            updateRequest.Represent(caseId, "Test2_1_3RequestRepresentment");
            
            // Step 2. Verify that the new activity has been appended to the activity list for the case.
            retrievalResponse = retrievalRequest.RetrieveByArn("2222222222");
            chargebackApiActivity[] activities = retrievalResponse.chargebackCase[0].activity;
            Assert.AreEqual(activityType.MERCHANT_REPRESENT, activities[activities.Length - 1].activityType);
            
            // Step 3. Perform a new activity for the case retrieved from ARN 3333333333.
            retrievalResponse = retrievalRequest.RetrieveByArn("3333333333");
            caseId = retrievalResponse.chargebackCase[0].caseId;
            updateRequest.Represent(caseId, 10027, "Test2_1_3RequestRepresentment");
            
            // Step 4. Verify that the new activity has been appended to the activity list for the case.
            retrievalResponse = retrievalRequest.RetrieveByArn("3333333333");
            activities = retrievalResponse.chargebackCase[0].activity;
            Assert.AreEqual(activityType.MERCHANT_REPRESENT, activities[activities.Length - 1].activityType);
        }
        
        [Test]
        [Ignore("Cannot run multiple times on Prelive.")]
        public void Test_2_1_4_AssumingLiability()
        {
            // Step 1. Perform a new activity for the case retrieved from ARN 4444444444.
            ChargebackRetrievalRequest retrievalRequest = new ChargebackRetrievalRequest();
            chargebackRetrievalResponse retrievalResponse = retrievalRequest.RetrieveByArn("4444444444");
            long caseId = retrievalResponse.chargebackCase[0].caseId;
            chargebackUpdateRequest updateRequest = new chargebackUpdateRequest();
            updateRequest.AcceptLiability(caseId, "Test2_1_4AssumingLiability");
            
            // Step 2. Verify that the new activity has been appended to the activity list for the case.
            retrievalResponse = retrievalRequest.RetrieveByCaseId(caseId);
            chargebackApiActivity[] activities = retrievalResponse.chargebackCase[0].activity;
            Assert.AreEqual(activityType.MERCHANT_ACCEPTS_LIABILITY, activities[activities.Length - 1].activityType);
        }

        [Test]
        public void Test_2_1_5_ErrorMessage()
        {
            // Step 1. Perform a new activity for the case retrieved from ARN 4444444444.
            ChargebackRetrievalRequest retrievalRequest = new ChargebackRetrievalRequest();
            chargebackRetrievalResponse retrievalResponse = retrievalRequest.RetrieveByArn("4444444444");
            long caseId = retrievalResponse.chargebackCase[0].caseId;
            
            // Step 2-3. Verify that your system correctly handle the error.
            chargebackUpdateRequest updateRequest = new chargebackUpdateRequest();
            try
            {
                updateRequest.AcceptLiability(caseId, "Test2_1_5ErrorMessage");
            }
            catch(ChargebackException ce)
            {
                Console.WriteLine("Exception message:" + ce.Message);
                Console.WriteLine("End exception message.");
                Assert.True(ce.Message.Contains("Cannot perform activity <Merchant Accepts Liability> " +
                                "for case <"+caseId+"> in queue <Merchant Assumed>"));
            }
            
            // Step 4. Attempt to retrieve a case using a random value for caseId.
            caseId = (new Random(DateTime.Now.Millisecond)).Next();
            
            // Step 5-6 Verify that your system correctly handle the error.
            try
            {
                retrievalRequest.RetrieveByCaseId(caseId);
            }
            catch(ChargebackException ce)
            {
                Assert.True(ce.Message.Contains("Could not find requested object."));
            }
        }

        [Test]
        [Ignore("Cannot run multiple times on Prelive.")]
        public void Test_2_1_6_DeclineAVisaPreArbitrationCase()
        {
            // Step 1. Perform a new activity for the case retrieved from ARN 5555555550.
            var retrievalRequest = new ChargebackRetrievalRequest();
            var retrievalResponse = retrievalRequest.RetrieveByArn("5555555550");
            var caseId = retrievalResponse.chargebackCase[0].caseId;
            var updateRequest = new chargebackUpdateRequest();
            updateRequest.Represent(caseId);
            
            // Step 2. Verify that the new activity has been appended to the activity list for the case.
            retrievalResponse = retrievalRequest.RetrieveByCaseId(caseId);
            chargebackApiActivity[] activities = retrievalResponse.chargebackCase[0].activity;
            Assert.AreEqual(activityType.MERCHANT_REPRESENT, activities[activities.Length - 1].activityType);
            
            // Step 3. Perform a new activity for the case retrieved from ARN 5555555551.
            retrievalResponse = retrievalRequest.RetrieveByArn("5555555551");
            caseId = retrievalResponse.chargebackCase[0].caseId;
            updateRequest.Represent(caseId, 10051);
            
            // Step 4. Verify that the new activity has been appended to the activity list for the case.
            retrievalResponse = retrievalRequest.RetrieveByCaseId(caseId);
            activities = retrievalResponse.chargebackCase[0].activity;
            Assert.AreEqual(activityType.MERCHANT_REPRESENT, activities[activities.Length - 1].activityType);
            Assert.AreEqual(10051, activities[activities.Length - 1].settlementAmount);
        }
        
        [Test]
        [Ignore("Cannot run multiple times on Prelive.")]
        public void Test_2_1_7_AssumeLiabilityOfAVisaPreArbitration()
        {
            // Step 1. Perform a new activity for the case retrieved from ARN 5555555552.
            var retrievalRequest = new ChargebackRetrievalRequest();
            var retrievalResponse = retrievalRequest.RetrieveByArn("5555555552");
            var caseId = retrievalResponse.chargebackCase[0].caseId;
            var updateRequest = new chargebackUpdateRequest();
            updateRequest.AcceptLiability(caseId);
            
            // Step 2. Verify that the new activity has been appended to the activity list for the case.
            retrievalResponse = retrievalRequest.RetrieveByCaseId(caseId);
            chargebackApiActivity[] activities = retrievalResponse.chargebackCase[0].activity;
            Assert.AreEqual(activityType.MERCHANT_ACCEPTS_LIABILITY, activities[activities.Length - 1].activityType);
        }
        
        [Test]
        [Ignore("Cannot run multiple times on Prelive.")]
        public void Test_2_1_8_AssumeLiabilityOfAVisaArbitration()
        {
            // Step 1. Perform a new activity for the case retrieved from ARN 6666666660.
            var retrievalRequest = new ChargebackRetrievalRequest();
            var retrievalResponse = retrievalRequest.RetrieveByArn("6666666660");
            var caseId = retrievalResponse.chargebackCase[0].caseId;
            var updateRequest = new chargebackUpdateRequest();
            updateRequest.AcceptLiability(caseId);
            
            // Step 2. Verify that the new activity has been appended to the activity list for the case.
            retrievalResponse = retrievalRequest.RetrieveByCaseId(caseId);
            chargebackApiActivity[] activities = retrievalResponse.chargebackCase[0].activity;
            Assert.AreEqual(activityType.MERCHANT_ACCEPTS_LIABILITY, activities[activities.Length - 1].activityType);
        }
        
        [Test]
        [Ignore("Cannot run multiple times on Prelive.")]
        public void Test_2_1_9_DeclineAVisaIssuerDeclinedPreAbitrationCase()
        {
            // Step 1. Perform a new activity for the case retrieved from ARN 7777777770.
            var retrievalRequest = new ChargebackRetrievalRequest();
            var retrievalResponse = retrievalRequest.RetrieveByArn("7777777770");
            var caseId = retrievalResponse.chargebackCase[0].caseId;
            var updateRequest = new chargebackUpdateRequest();
            updateRequest.Represent(caseId);
            
            // Step 2. Verify that the new activity has been appended to the activity list for the case.
            retrievalResponse = retrievalRequest.RetrieveByCaseId(caseId);
            chargebackApiActivity[] activities = retrievalResponse.chargebackCase[0].activity;
            Assert.AreEqual(activityType.MERCHANT_REPRESENT, activities[activities.Length - 1].activityType);
            
            // Step 3. Perform a new activity for the case retrieved from ARN 7777777771.
            retrievalResponse = retrievalRequest.RetrieveByArn("7777777771");
            caseId = retrievalResponse.chargebackCase[0].caseId;
            updateRequest.Represent(caseId, 10071);
            
            // Step 4. Verify that the new activity has been appended to the activity list for the case.
            retrievalResponse = retrievalRequest.RetrieveByCaseId(caseId);
            activities = retrievalResponse.chargebackCase[0].activity;
            Assert.AreEqual(activityType.MERCHANT_REPRESENT, activities[activities.Length - 1].activityType);
            Assert.AreEqual(10071, activities[activities.Length - 1].settlementAmount);
        }

        [Test]
        [Ignore("Cannot run multiple times on Prelive.")]
        public void Test_2_1_9_AssumeLiabilityOfAVisaIssuerDeclinedPreAbitrationCase()
        {
            // Step 1. Perform a new activity for the case retrieved from ARN 7777777772.
            var retrievalRequest = new ChargebackRetrievalRequest();
            var retrievalResponse = retrievalRequest.RetrieveByArn("7777777772");
            var caseId = retrievalResponse.chargebackCase[0].caseId;
            var updateRequest = new chargebackUpdateRequest();
            updateRequest.Represent(caseId);
            
            // Step 2. Verify that the new activity has been appended to the activity list for the case.
            retrievalResponse = retrievalRequest.RetrieveByCaseId(caseId);
            chargebackApiActivity[] activities = retrievalResponse.chargebackCase[0].activity;
            Assert.AreEqual(activityType.MERCHANT_ACCEPTS_LIABILITY, activities[activities.Length - 1].activityType);
        }
    }
}