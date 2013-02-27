Portable.Licensing is a cross platform software licensing framework which allows you to implement licensing into your application or library. It provides you all tools to create and validate licenses for your software.

Portable.Licensing is using the latest military strength, state-of-the-art cryptographic algorithm to ensure that your software and thus your intellectual property is protected.

It is targeting the *Portable Class Library* and thus runs on nearly every .NET/Mono profile including Silverlight, Windows Phone, Windows Store App, Xamarin.iOS, Xamarin.Android, Xamarin.Mac and XBox 360. Use it for your Desktop- (WinForms, WPF, etc.), Console-, Service-, Web- (ASP.NET, MVC, etc.), Mobile (Xamarin.iOS, Xamarin.Android) or even LightSwitch applications.

## Features ##

- runs on .NET >= 4.0.3, Silverlight >= 4, Windows Phone >= 7.5, Windows Store Apps, Mono, Xamarin.iOS, Xamarin.Android, Xamarin.Mac, XBox 360
- easy creation and validation of your licenses
- trial, standard and subscription licenses
- cryptographic signed licenses, no alteration possible
- allows you to enable/disable program modules or product features
- limit various parameters for your software, e.g. max transactions, etc.
- add additional key/value pair attributes to your licenses
- easily extend the fluent validation API with extension methods


----------

### License.Manager light ###

A reference implementation of a management application for products, customers and licenses is available here: [https://github.com/dnauck/License.Manager-Light](https://github.com/dnauck/License.Manager-Light)

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

[http://dev.nauck-it.de/projects/portable-licensing](http://dev.nauck-it.de/projects/portable-licensing)
