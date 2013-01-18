# Documentation #

----------

## About Portable.Licensing ##

Portable.Licensing is a portable solution to implement a licensing component into your application or library.

## Features ##

- runs on .NET >= 4.0.3, Silverlight >= 4, Windows Phone >= 7.5, Windows Store Apps, Mono, XBox 360
- easy creation and validation of your licenses
- trial licenses
- cryptographic signed licenses, no alteration possible


----------


## Getting started ##
### Create a private and public key for your product ###

Portable.Licensing uses asymmetric (public-key) encryption to ensure the license cannot be altered after creation.

First you need to create a new public/private key pair for your product:

    var keyGenerator = Portable.Licensing.Security.Cryptography.KeyGenerator.Create();  
    privateKey = keyGenerator.ToXmlString(true);  
    publicKey = keyGenerator.ToXmlString(false);

Store the private key securely and distribute the public key with your product.


### Create the license generator ###


Now we need something to generate licenses. This could be easily done with the *LicenseFactory*:

    var license = LicenseFactory.New()  
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
        .CreateAndSignWithPrivateKey(privateKey);

You can now take the license and save it to a file for example:

    File.WriteAllText("License.lic", license.ToString(), Encoding.UTF8);


### Validate the license in your application ###

The easiest way to assert the license in the entry point of your application.

First load the license from a file or resource:

    var license = LicenseFactory.Load(...);

Then you can assert the license:

    var validationFailures = license.Validate()  
                                    .ExpirationDate()  
                                        .When(lic => lic.Type == LicenseType.Trial)  
                                    .And()  
                                    .Signature(publicKey)  
                                    .AssertValidLicense();

----------


## License ##

Portable.Licensing is distributed using the MIT/X11 License.

## Further Information ##

The latest release and documentation can be found on
the Portable.Licensing project website:

[https://github.com/dnauck/Portable.Licensing](https://github.com/dnauck/Portable.Licensing)
