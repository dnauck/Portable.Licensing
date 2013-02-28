using System;
using System.Linq;
using System.Text;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Portable.Licensing;
using Portable.Licensing.Validation;

namespace Android.Sample
{
    [Activity(Label = "Android.Sample", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        int count = 1;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

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
                       .Signature(Resources.GetString(Resource.String.LicensePublicKey))
                       .AssertValidLicense().ToList();

            if (validationFailures.Any())
            {
                var dialogBuilder = new AlertDialog.Builder(this);
                dialogBuilder.SetTitle("License validation failure!");
                dialogBuilder.SetIcon(Android.Resource.Drawable.IcDialogAlert);

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

                dialogBuilder.SetMessage(messageBuilder.ToString());
                dialogBuilder.SetNeutralButton("Cancel", (sender, args) => { /* close app */ });

                dialogBuilder.Show();
            }

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            // Get our button from the layout resource,
            // and attach an event to it
            Button button = FindViewById<Button>(Resource.Id.MyButton);

            button.Click += delegate { button.Text = string.Format("{0} clicks!", count++); };
        }
    }
}

