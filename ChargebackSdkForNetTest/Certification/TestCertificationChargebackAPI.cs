using System;
using System.Collections.Generic;
using ChargebackSdkForNet;
using ChargebackSdkForNet.Properties;
using Xunit;

namespace ChargebackSdkForNetTest.Certification
{
    public class TestCertificationChargebackApi
    {
        private ChargebackRetrievalRequest _retrievalRequest;
        private ChargebackUpdateRequest _updateRequest;

        public void SetUp()
        {
            var config = new Configuration();
            config.Set("host", "https://services.vantivprelive.com");
            _retrievalRequest = new ChargebackRetrievalRequest { Config = config};
            _updateRequest = new ChargebackUpdateRequest { Config = config };
        }


        [Fact(Skip = "Added for reference")]
        public void Test_2_1_1_RetrieveByAParticularDate()
        {
            chargebackRetrievalResponse response = _retrievalRequest.RetrieveByActivityDate(ChargebackUtils.ParseDate("2013-1-1"));
            Assert.NotNull(response);
            Assert.Equal(11, response.chargebackCase.Length);
            chargebackApiCase[] cases = response.chargebackCase;
            Assert.Equal("1111111111", cases[0].acquirerReferenceNumber);
            Assert.Equal("2222222222", cases[1].acquirerReferenceNumber);
            Assert.Equal("3333333333", cases[2].acquirerReferenceNumber);
            Assert.Equal("4444444444", cases[3].acquirerReferenceNumber);
            Assert.Equal("5555555550", cases[4].acquirerReferenceNumber);
            Assert.Equal("5555555551", cases[5].acquirerReferenceNumber);
            Assert.Equal("5555555552", cases[6].acquirerReferenceNumber);
            Assert.Equal("6666666660", cases[7].acquirerReferenceNumber);
            Assert.Equal("7777777770", cases[8].acquirerReferenceNumber);
            Assert.Equal("7777777771", cases[9].acquirerReferenceNumber);
            Assert.Equal("7777777772", cases[10].acquirerReferenceNumber);
/*          Assert.Equal("FIRST_CHARGBACK", cases[0].cycle);
            Assert.Equal("FIRST_CHARGBACK", cases[1].cycle);
            Assert.Equal("FIRST_CHARGBACK", cases[2].cycle);
            Assert.Equal("FIRST_CHARGBACK", cases[3].cycle);
            Assert.Equal("PRE_ARB_CHARGBACK", cases[4].cycle);
            Assert.Equal("PRE_ARB_CHARGBACK", cases[5].cycle);
            Assert.Equal("PRE_ARB_CHARGBACK", cases[6].cycle);
            Assert.Equal("ARBITRATION_CHARGEBACK", cases[7].cycle);
            Assert.Equal("ISSUER_DECLINE_PRESAB", cases[8].cycle);
            Assert.Equal("ISSUER_DECLINE_PRESAB", cases[9].cycle);
            Assert.Equal("ISSUER_DECLINE_PRESAB", cases[10].cycle);*/
        }

        [Fact(Skip = "Added for reference")]
        public void Test_2_1_2_AddNote()
        {
            // Step 1. Perform a new activity for the case retrieved from ARN 1111111111.
            chargebackRetrievalResponse retrievalResponse = _retrievalRequest.RetrieveByArn("1111111111");
            long caseId = retrievalResponse.chargebackCase[0].caseId;
            
            long milSec = DateTime.Now.Millisecond;
            string expectedNote = milSec + " Call Vantiv!";
            chargebackUpdateResponse updateResponse = _updateRequest.AddNote(caseId, expectedNote);
            Assert.NotNull(updateResponse);
            
            // Step 2. Verify that the new activity has been appended to the activity list for the case.
            retrievalResponse = _retrievalRequest.RetrieveByArn("1111111111");
            chargebackApiActivity[] activities = retrievalResponse.chargebackCase[0].activity;
            foreach (var activity in activities)
            {
                if (expectedNote.Equals(activity.notes))
                {
                    Assert.Equal("Add Note", activity.activityType);
                    return;
                }
            }
            Assert.True(false);
        }
        
        [Fact(Skip = "Cannot run multiple times on Prelive.")]
        public void Test_2_1_3_RequestRepresentment()
        {
            // Step 1. Perform a new activity for the case retrieved from ARN 2222222222.
            chargebackRetrievalResponse retrievalResponse = _retrievalRequest.RetrieveByArn("2222222222");
            long caseId = retrievalResponse.chargebackCase[0].caseId;
            _updateRequest.Represent(caseId, "Test2_1_3RequestRepresentment");
            
            // Step 2. Verify that the new activity has been appended to the activity list for the case.
            retrievalResponse = _retrievalRequest.RetrieveByArn("2222222222");
            chargebackApiActivity[] activities = retrievalResponse.chargebackCase[0].activity;
            Assert.Equal(activityType.MERCHANT_REPRESENT.ToString(), activities[activities.Length - 1].activityType);
            
            // Step 3. Perform a new activity for the case retrieved from ARN 3333333333.
            retrievalResponse = _retrievalRequest.RetrieveByArn("3333333333");
            caseId = retrievalResponse.chargebackCase[0].caseId;
            _updateRequest.Represent(caseId, 10027, "Test2_1_3RequestRepresentment");
            
            // Step 4. Verify that the new activity has been appended to the activity list for the case.
            retrievalResponse = _retrievalRequest.RetrieveByArn("3333333333");
            activities = retrievalResponse.chargebackCase[0].activity;
            Assert.Equal(activityType.MERCHANT_REPRESENT.ToString(), activities[activities.Length - 1].activityType);
        }
        
        [Fact(Skip = "Cannot run multiple times on Prelive.")]
        public void Test_2_1_4_AssumingLiability()
        {
            // Step 1. Perform a new activity for the case retrieved from ARN 4444444444.
            chargebackRetrievalResponse retrievalResponse = _retrievalRequest.RetrieveByArn("4444444444");
            long caseId = retrievalResponse.chargebackCase[0].caseId;
            _updateRequest.AcceptLiability(caseId, "Test2_1_4AssumingLiability");
            
            // Step 2. Verify that the new activity has been appended to the activity list for the case.
            retrievalResponse = _retrievalRequest.RetrieveByCaseId(caseId);
            chargebackApiActivity[] activities = retrievalResponse.chargebackCase[0].activity;
            Assert.Equal(activityType.MERCHANT_ACCEPTS_LIABILITY.ToString(), activities[activities.Length - 1].activityType);
        }

        [Fact(Skip = "Added for reference")]
        public void Test_2_1_5_ErrorMessage()
        {
            // Step 1. Perform a new activity for the case retrieved from ARN 4444444444.
            chargebackRetrievalResponse retrievalResponse = _retrievalRequest.RetrieveByArn("4444444444");
            long caseId = retrievalResponse.chargebackCase[0].caseId;
            
            // Step 2-3. Verify that your system correctly handle the error.
            try
            {
                _updateRequest.AcceptLiability(caseId, "Test2_1_5ErrorMessage");
                Assert.True(false);
            }
            catch(ChargebackWebException ce)
            {
                Console.WriteLine("Exception message:" + ce.ErrorMessage);
                Console.WriteLine("End exception message.");
                Assert.Equal("Cannot perform activity <Merchant Accepts Liability> " +
                                "for case <"+caseId+"> in queue <Merchant Assumed>", ce.ErrorMessages[0]);
            }
            
            // Step 4. Attempt to retrieve a case using a random value for caseId.
            caseId = (new Random(DateTime.Now.Millisecond)).Next();
            
            // Step 5-6 Verify that your system correctly handle the error.
            try
            {
                _retrievalRequest.RetrieveByCaseId(caseId);
                Assert.True(false);
            }
            catch(ChargebackWebException ce)
            {
                Assert.Equal("Could not find requested object.", ce.ErrorMessages[0]);
            }
        }

        [Fact(Skip = "Cannot run multiple times on Prelive.")]
        public void Test_2_1_6_DeclineAVisaPreArbitrationCase()
        {
            // Step 1. Perform a new activity for the case retrieved from ARN 5555555550.

            var retrievalResponse = _retrievalRequest.RetrieveByArn("5555555550");
            var caseId = retrievalResponse.chargebackCase[0].caseId;

            _updateRequest.Represent(caseId);
            
            // Step 2. Verify that the new activity has been appended to the activity list for the case.
            retrievalResponse = _retrievalRequest.RetrieveByCaseId(caseId);
            chargebackApiActivity[] activities = retrievalResponse.chargebackCase[0].activity;
            Assert.Equal(activityType.MERCHANT_REPRESENT.ToString(), activities[activities.Length - 1].activityType);
            
            // Step 3. Perform a new activity for the case retrieved from ARN 5555555551.
            retrievalResponse = _retrievalRequest.RetrieveByArn("5555555551");
            caseId = retrievalResponse.chargebackCase[0].caseId;
            _updateRequest.Represent(caseId, 10051);
            
            // Step 4. Verify that the new activity has been appended to the activity list for the case.
            retrievalResponse = _retrievalRequest.RetrieveByCaseId(caseId);
            activities = retrievalResponse.chargebackCase[0].activity;
            Assert.Equal(activityType.MERCHANT_REPRESENT.ToString(), activities[activities.Length - 1].activityType);
            Assert.Equal(10051, activities[activities.Length - 1].settlementAmount);
        }
        
        [Fact(Skip = "Cannot run multiple times on Prelive.")]
        public void Test_2_1_7_AssumeLiabilityOfAVisaPreArbitration()
        {
            // Step 1. Perform a new activity for the case retrieved from ARN 5555555552.

            var retrievalResponse = _retrievalRequest.RetrieveByArn("5555555552");
            var caseId = retrievalResponse.chargebackCase[0].caseId;

            _updateRequest.AcceptLiability(caseId);
            
            // Step 2. Verify that the new activity has been appended to the activity list for the case.
            retrievalResponse = _retrievalRequest.RetrieveByCaseId(caseId);
            chargebackApiActivity[] activities = retrievalResponse.chargebackCase[0].activity;
            Assert.Equal(activityType.MERCHANT_ACCEPTS_LIABILITY.ToString(), activities[activities.Length - 1].activityType);
        }
        
        [Fact(Skip = "Cannot run multiple times on Prelive.")]
        public void Test_2_1_8_AssumeLiabilityOfAVisaArbitration()
        {
            // Step 1. Perform a new activity for the case retrieved from ARN 6666666660.

            var retrievalResponse = _retrievalRequest.RetrieveByArn("6666666660");
            var caseId = retrievalResponse.chargebackCase[0].caseId;

            _updateRequest.AcceptLiability(caseId);
            
            // Step 2. Verify that the new activity has been appended to the activity list for the case.
            retrievalResponse = _retrievalRequest.RetrieveByCaseId(caseId);
            chargebackApiActivity[] activities = retrievalResponse.chargebackCase[0].activity;
            Assert.Equal(activityType.MERCHANT_ACCEPTS_LIABILITY.ToString(), activities[activities.Length - 1].activityType);
        }
        
        [Fact(Skip = "Cannot run multiple times on Prelive.")]
        public void Test_2_1_9_DeclineAVisaIssuerDeclinedPreAbitrationCase()
        {
            // Step 1. Perform a new activity for the case retrieved from ARN 7777777770.

            var retrievalResponse = _retrievalRequest.RetrieveByArn("7777777770");
            var caseId = retrievalResponse.chargebackCase[0].caseId;

            _updateRequest.Represent(caseId);
            
            // Step 2. Verify that the new activity has been appended to the activity list for the case.
            retrievalResponse = _retrievalRequest.RetrieveByCaseId(caseId);
            chargebackApiActivity[] activities = retrievalResponse.chargebackCase[0].activity;
            Assert.Equal(activityType.MERCHANT_REPRESENT.ToString(), activities[activities.Length - 1].activityType);
            
            // Step 3. Perform a new activity for the case retrieved from ARN 7777777771.
            retrievalResponse = _retrievalRequest.RetrieveByArn("7777777771");
            caseId = retrievalResponse.chargebackCase[0].caseId;
            _updateRequest.Represent(caseId, 10071);
            
            // Step 4. Verify that the new activity has been appended to the activity list for the case.
            retrievalResponse = _retrievalRequest.RetrieveByCaseId(caseId);
            activities = retrievalResponse.chargebackCase[0].activity;
            Assert.Equal(activityType.MERCHANT_REPRESENT.ToString(), activities[activities.Length - 1].activityType);
            Assert.Equal(10071, activities[activities.Length - 1].settlementAmount);
        }

        [Fact(Skip = "Cannot run multiple times on Prelive.")]
        public void Test_2_1_9_AssumeLiabilityOfAVisaIssuerDeclinedPreAbitrationCase()
        {
            // Step 1. Perform a new activity for the case retrieved from ARN 7777777772.
            var retrievalResponse = _retrievalRequest.RetrieveByArn("7777777772");
            var caseId = retrievalResponse.chargebackCase[0].caseId;
            _updateRequest.Represent(caseId);
            
            // Step 2. Verify that the new activity has been appended to the activity list for the case.
            retrievalResponse = _retrievalRequest.RetrieveByCaseId(caseId);
            chargebackApiActivity[] activities = retrievalResponse.chargebackCase[0].activity;
            Assert.Equal(activityType.MERCHANT_ACCEPTS_LIABILITY.ToString(), activities[activities.Length - 1].activityType);
        }
    }
}