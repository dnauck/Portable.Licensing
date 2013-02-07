# Documentation #

----------

## About Portable.Licensing ##

Portable.Licensing is a portable solution which allows you to implement a licensing component into your application or library.

## Features ##

- runs on .NET >= 4.0.3, Silverlight >= 4, Windows Phone >= 7.5, Windows Store Apps, Mono, XBox 360
- easy creation and validation of your licenses
- trial licenses
- cryptographic signed licenses, no alteration possible


----------


## Getting started ##

### License.Manager light ###

A reference implementation of a management application for products, customers and licenses is available here: [https://github.com/dnauck/License.Manager-Light](https://github.com/dnauck/License.Manager-Light)

### Download ###

[Portable.Licensing](https://nuget.org/packages/Portable.Licensing) is distributed with the popular NuGet Packaging Manager. This will make it easier for developers to get the Portable.Licensing distribution into their project.

Go to the NuGet website for more details: [http://nuget.org/](http://nuget.org/)

NuGet provides several ways to get Portable.Licensing into your Project. The easiest way is to right click your project references in Visual Studio and choose the menu item "Manage NuGet Packages".

Search in the "Online" tab for "Portable.Licensing" and click "Install" on your selected packages.

It is also possible to install the Portable.Licensing packages via the "NuGet Package Manager Console".

Type the following to install the Footloose core library:

    PM> Install-Package Portable.Licensing

Or use the following command to get a specific version:

    PM> Install-Package Portable.Licensing -Version 1.0.0

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

----------

## License ##

Portable.Licensing is distributed using the MIT/X11 License.

## Further Information ##

The latest release and documentation can be found on the Portable.Licensing project website:

[https://github.com/dnauck/Portable.Licensing](https://github.com/dnauck/Portable.Licensing)
