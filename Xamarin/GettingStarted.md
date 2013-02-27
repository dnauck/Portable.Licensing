## Getting started ##

### License.Manager light ###

A reference implementation of a management application for products, customers and licenses is available here: [https://github.com/dnauck/License.Manager-Light](https://github.com/dnauck/License.Manager-Light)

### Create a private and public key for your product ###

Portable.Licensing uses the Elliptic Curve Digital Signature Algorithmus (ECDSA) to ensure the license cannot be altered after creation.

First you need to create a new public/private key pair for your product:

    var keyGenerator = Portable.Licensing.Security.Cryptography.KeyGenerator.Create(); 
    var keyPair = keyGenerator.GenerateKeyPair(); 
    var privateKey = keyPair.ToEncryptedPrivateKeyString(passPhrase);  
    var publicKey = keyPair.ToPublicKeyString();

Store the private key securely and distribute the public key with your product.


### Create the license generator ###


Now we need something to generate licenses. This could be easily done with the *LicenseFactory*:

    var license = License.New()  
        .WithUniqueIdentifier(Guid.NewGuid())  
        .As(LicenseType.Trial)  
        .ExpiresAt(DateTime.Now.AddDays(30))  
        .WithMaximumUtilization(5)  
        .WithProductFeatures(new Dictionary<string, string>  
                                      {  
                                          {"Sales Module", "yes"},  
                                          {"Purchase Module", "yes"},  
                                          {"Maximum Transactions", "10000"}  
                                      })  
        .LicensedTo("John Doe", "john.doe@yourmail.here")  
        .CreateAndSignWithPrivateKey(privateKey, passPhrase);

Now you can take the license and save it to a file:

    File.WriteAllText("License.lic", license.ToString(), Encoding.UTF8);

or

    license.Save(xmlWriter);  


### Validate the license in your application ###

The easiest way to assert the license is in the entry point of your application.

First load the license from a file or resource:

    var license = License.Load(...);

Then you can assert the license:

    var validationFailures = license.Validate()  
                                    .ExpirationDate()  
                                        .When(lic => lic.Type == LicenseType.Trial)  
                                    .And()  
                                    .Signature(publicKey)  
                                    .AssertValidLicense();