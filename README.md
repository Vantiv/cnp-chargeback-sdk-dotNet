Vantiv Now Worldpay eCommerce .NET Chargeback SDK
=====================

#### WARNING:
##### All major version changes require recertification to the new version. Once certified for the use of a new version, Vantiv Now Worldpay modifies your Merchant Profile, allowing you to submit transaction to the Production Environment using the new version. Updating your code without recertification and modification of your Merchant Profile will result in transaction declines. Please consult you Implementation Analyst for additional information about this process.

About Vantiv Now Worldpay eCommerce
------------
[Vantiv Now Worldpay eCommerce](https://developer.vantiv.com/community/ecommerce) powers the payment processing engines for leading companies that sell directly to consumers through  internet retail, direct response marketing (TV, radio and telephone), and online services. Vantiv eCommerce is the leading authority in card-not-present (CNP) commerce, transaction processing and merchant services.


About this Chargeback SDK
--------------
The Vantiv eCommerce .NET Chargeback SDK is a C# implementation of the [Vantiv eCommerce](https://developer.vantiv.com/docs/DOC-1196) Chargeback API. This Chargeback SDK was created to make it as easy as possible to process your chargebacks and upload documents supporting your cases with Vantiv eCommerce. This Chargeback SDK utilizes the HTTPS protocol to securely connect to Vantiv eCommerce. Using the Chargeback SDK requires coordination with the Vantiv eCommerce team in order to be provided with credentials for accessing our systems.

Each .NET Chargeback SDK release supports all of the functionality present in the associated Vantiv eCommerce Chargeback API version (e.g., Chargeback SDK v2.1 supports Vantiv eCommerce Chargeback API v2.1). Please see the online copy of our Chargeback XSD for Vantiv eCommerce XML to get more details on what the Vantiv eCommerce Chargeback API supports.

This Chargeback SDK is implemented to support the .NET plaform, including C#, VB.NET and Managed C++ and was created by Vantiv eCommerce. Its intended use is for managing your chargeback cases and uploading/downloading documents supporting your cases.

See LICENSE file for details on using this software.

Source Code available from : https://github.com/Vantiv/cnp-chargeback-sdk-dotNet

Please contact [Vantiv eCommerce](http://developer.vantiv.com/community/ecommerce) to receive valid merchant credentials in order to run tests successfully or if you require assistance in any way.  We are reachable at sdksupport@Vantiv.com

Setup
-----

1.) To install it, copy ChargebackForDotNet.dll into your Visual Studio references.

2.) You can configure it statically by adding the following section to your project's App.config
```
    <configSections>
        <sectionGroup name="vantivWorldpay">
            <section name="chargebackSettings"
                     type="System.Configuration.NameValueSectionHandler" />
        </sectionGroup>
    </configSections>
    <vantivWorldpay>
        <chargebackSettings>
            <add key="username" value="myUsername" />
            <add key="password" value="myPassword" />
            <add key="merchantId" value="777777" />
            <add key="host" value="https://www.testvantivcnp.com/sandbox/new/services" />
            <add key="printXml" value="true" />
            <add key="proxyHost" value="myProxyHost" />
            <add key="proxyPort" value="7777" />
            <add key="neuterXml" value="false" />
        </chargebackSettings>
    </vantivWorldpay>
```
Also, you can use a different Configuration constructor to pass a file path to a simple configuration file that contains [key=value] settings; an example of this configuration file can be found at (https://github.com/Vantiv/cnp-chargeback-sdk-dotNet/blob/2.x/sampleConfig.txt). 
```
    username = myUsername
    password = myPassword
    merchantId = 777777
    host = https://www.testvantivcnp.com/sandbox/new/services
    printXml = true
    neuterXml = false
    proxyHost = myProxyHost
    proxyPort = 7777
```
In addition, you can configure it dynamically by create a dictionary with required settings and call the Configuration constructor taking a dictionary.

3.) Let's try our SDK with the Sandbox, which requires no valid username and password:  

```c#
using System;
using ChargebackForDotNet;

namespace Merchant
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            var request = new ChargebackRetrievalRequest();
            request.Config.Set("host", "https://www.testvantivcnp.com/sandbox/new/services");
            var dateTime = new DateTime(2013,1,1);
            var response = request.RetrieveByActivityDate(dateTime);
            var cases = response.chargebackCase;
            foreach (var c in cases)
            {
                Console.WriteLine("Case Id:" + c.caseId);
            }
        }
    }
}
```

4) Compile and run this file.  You should see the following result:
~~~
    Case Id:1288791001
~~~

5) In Step 3, you tried our .NET SDK with the Sandbox. Once you are registered to test with our Prelive environment, run the example with the following host (https://services.vantivprelive.com).

```c#
using System;
using ChargebackForDotNet;

namespace Merchant
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            var request = new ChargebackRetrievalRequest();
            request.Config.Set("host", "https://services.vantivprelive.com");
            var dateTime = new DateTime(2013,1,1);
            var response = request.RetrieveByActivityDate(dateTime);
            var cases = response.chargebackCase;
            foreach (var c in cases)
            {
                Console.WriteLine("Case Id:" + c.caseId);
            }
        }
    }
}
```

6) The expected output should be:
~~~
    Case Id:1288791001
    Case Id:1288791002
    Case Id:1288791003
    Case Id:1288791004
    Case Id:1288791005
    Case Id:1288791006
    Case Id:1288791007
    Case Id:1288791008
    Case Id:1288791009
    Case Id:12887910010
    Case Id:12887910011
~~~

More examples can be found in [Functional and Unit Tests](https://github.com/Vantiv/cnp-chargeback-sdk-dotNet/tree/2.x/ChargebackForDotNetTest)

Please contact Vantiv eCommerce with any further questions. You can reach us at sdksupport@Vantiv.com.
