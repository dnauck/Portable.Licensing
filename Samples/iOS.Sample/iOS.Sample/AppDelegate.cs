using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using MonoTouch.Foundation;
using MonoTouch.UIKit;

using Portable.Licensing;
using Portable.Licensing.Validation;

namespace iOS.Sample
{
    [Register("AppDelegate")]
    public partial class AppDelegate : UIApplicationDelegate
    {
		const string licensePublicKey = @"MIIBKjCB4wYHKoZIzj0CATCB1wIBATAsBgcqhkjOPQEBAiEA/////wAAAAEAAAAAAAAAAAAAAAD///////////////8wWwQg/////wAAAAEAAAAAAAAAAAAAAAD///////////////wEIFrGNdiqOpPns+u9VXaYhrxlHQawzFOw9jvOPD4n0mBLAxUAxJ02CIbnBJNqZnjhE50mt4GffpAEIQNrF9Hy4SxCR/i85uVjpEDydwN9gS3rM6D0oTlF2JjClgIhAP////8AAAAA//////////+85vqtpxeehPO5ysL8YyVRAgEBA0IABNVLQ1xKY80BFMgGXec++Vw7n8vvNrq32PaHuBiYMm0PEj2JoB7qSSWhfgcjxNVJsxqJ6gDQVWgl0r7LH4dr0KU=";
        UIWindow window;
        MyViewController viewController;

        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
			var licenseContent = @"<License>
                                    <Id>77d4c193-6088-4c64-9663-ed7398ae8c1a</Id>
                                    <Type>Trial</Type>
                                    <Expiration>Thu, 31 Dec 2009 23:00:00 GMT</Expiration>
                                    <Quantity>1</Quantity>
                                    <Customer>
                                    <Name>John Doe</Name>
                                    <Email>john@doe.tld</Email>
                                    </Customer>
                                    <LicenseAttributes />
                                    <ProductFeatures />
                                    <Signature>MEUCIQDdDpq/Ddt4hZJlird/BcR6FVKLdWF/DENnd6r/0LuB3gIgVm7RSQx5mcjC32JjCoHNdoL8C+etXOtWKiYGLCT4q6w=</Signature>
                                </License>";

			var license = License.Load(licenseContent);
			
			var validationFailures =
				license.Validate()
					.ExpirationDate()
						.When(lic => lic.Type == LicenseType.Trial)
					.And()
					.Signature(licensePublicKey)
					.AssertValidLicense().ToList();
			
			if (validationFailures.Any())
			{
				var messageBuilder = new StringBuilder();
				foreach (var validationFailure in validationFailures)
				{
					messageBuilder.AppendLine("Failure:");
					messageBuilder.AppendLine(validationFailure.Message);
					messageBuilder.AppendLine(" ");
					messageBuilder.AppendLine("Resolve issue by:");
					messageBuilder.AppendLine(validationFailure.HowToResolve);
					messageBuilder.AppendLine(" ");
					messageBuilder.AppendLine(" ");
				}
				
				new UIAlertView ("License validation failure!", messageBuilder.ToString(), null, "Cancel").Show();
			}

            window = new UIWindow(UIScreen.MainScreen.Bounds);

            viewController = new MyViewController();
            window.RootViewController = viewController;

            window.MakeKeyAndVisible();

            return true;
        }
    }
}

