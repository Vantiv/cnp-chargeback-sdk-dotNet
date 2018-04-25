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

1.) To install it, just copy ChargebackForDotNet.dll into your Visual Studio referernces. 

2.) You can configure it statically by setting the path to the configuration file, which contains [key=value], to the environment variable named 'chargebackConfigPath'. An example configuration file can be found at (https://github.com/Vantiv/cnp-chargeback-sdk-dotNet/blob/2.x/sampleConfig.txt). If you are just trying it out, the username, password and merchant id don't matter, and you should choose the sandbox url at https://www.testvantivcnp.com/sandbox/new/services.

3.) Create a c# class similar to:  

```c#
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

    class Example
    {
        [STAThread]
        public static void Main(String[] args)
        {
            // Sample code.
        }
    }

```

4) Compile and run this file.  You should see the following result:

    Result

More examples can be found in [Functional and Unit Tests] (https://github.com/Vantiv/cnp-chargeback-sdk-dotNet/tree/2.x/ChargebackForDotNetTest)

Please contact Vantiv eCommerce with any further questions. You can reach us at sdksupport@Vantiv.com.
