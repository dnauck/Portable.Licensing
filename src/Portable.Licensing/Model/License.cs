﻿//
// Copyright © 2012 - 2013 Nauck IT KG     http://www.nauck-it.de
//
// Author:
//  Daniel Nauck        <d.nauck(at)nauck-it.de>
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System;
using System.Globalization;
using System.Text;
using System.Xml.Linq;

namespace Portable.Licensing.Model
{
    /// <summary>
    /// A software license
    /// </summary>
    internal class License : ModelBase, ILicense
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="License"/> class.
        /// </summary>
        public License()
            : base("License")
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="License"/> class
        /// with the specified content.
        /// </summary>
        /// <param name="content">The initial content of this <see cref="License"/>.</param>
        internal License(params object[] content)
            : base("License", content)
        {
        }

        /// <summary>
        /// Gets or sets the unique identifier of this <see cref="ILicense"/>.
        /// </summary>
        public Guid Id
        {
            get { return new Guid(GetTag("Id") ?? Guid.Empty.ToString()); }
            set { SetTag("Id", value.ToString()); }
        }

        /// <summary>
        /// Gets or set the <see cref="LicenseType"/> or this <see cref="ILicense"/>.
        /// </summary>
        public LicenseType Type
        {
            get
            {
                return
                    (LicenseType)
                    Enum.Parse(typeof (LicenseType), GetTag("Type") ?? LicenseType.Trial.ToString(), false);
            }
            set { SetTag("Type", value.ToString()); }
        }

        /// <summary>
        /// Get or sets the quantity of this license.
        /// E.g. the count of per-developer-licenses.
        /// </summary>
        public int Quantity
        {
            get { return int.Parse(GetTag("Quantity") ?? "0"); }
            set { SetTag("Quantity", value.ToString()); }
        }

        /// <summary>
        /// Gets or sets the <see cref="IProductFeatures"/> of this <see cref="ILicense"/>.
        /// </summary>
        public IProductFeatures ProductFeatures
        {
            get
            {
                if (Element("ProductFeatures") == null)
                    ProductFeatures = new ProductFeatures();

                return Element("ProductFeatures") as ProductFeatures;
            }
            private set { Add(value); }
        }

        /// <summary>
        /// Gets or sets the <see cref="ICustomer"/> of this <see cref="ILicense"/>.
        /// </summary>
        public ICustomer Customer
        {
            get
            {
                if(Element("Customer") == null) 
                    Customer = new Customer();

                return Element("Customer") as Customer;
            }
            private set { Add(value); }
        }

        /// <summary>
        /// Gets or sets the expiration date of this <see cref="ILicense"/>.
        /// Use this property to set the expiration date for a trial license
        /// or the expiration of support & subscription updates for a standard license.
        /// </summary>
        public DateTime Expiration
        {
            get { return DateTime.ParseExact(GetTag("Expiration"), "r", CultureInfo.InvariantCulture); }
            set { SetTag("Expiration", value.ToUniversalTime().ToString("r", CultureInfo.InvariantCulture)); }
        }

        /// <summary>
        /// Gets the digital signature of this license.
        /// </summary>
        /// <remarks>Use the <see cref="ILicense.Sign"/> method to compute a signature.</remarks>
        public string Signature
        {
            get { return GetTag("Signature"); }
        }

        /// <summary>
        /// Compute a signature and sign this <see cref="ILicense"/> with the provided key.
        /// </summary>
        /// <param name="privateKey">The private key in xml string format to compute the signature.</param>
        public void Sign(string privateKey)
        {
            var signTag = Element("Signature") ?? new XElement("Signature");

            try
            {
                if (signTag.Parent != null)
                    signTag.Remove();

                using (var e = new Security.Cryptography.ElGamal.ElGamalManaged())
                {
                    e.FromXmlString(privateKey);
                    var signature = e.Sign(Encoding.UTF8.GetBytes(ToString(SaveOptions.DisableFormatting)));
                    signTag.Value = Convert.ToBase64String(signature);
                }
            }
            finally
            {
                Add(signTag);
            }
        }

        /// <summary>
        /// Determines whether the <see cref="ILicense.Signature"/> property verifies for the specified key.
        /// </summary>
        /// <param name="publicKey">The public key in xml string format to verify the <see cref="ILicense.Signature"/>.</param>
        /// <returns>true if the <see cref="ILicense.Signature"/> verifies; otherwise false.</returns>
        public bool VerifySignature(string publicKey)
        {
            var signTag = Element("Signature");
            
            if (signTag == null)
                return false;

            try
            {
                signTag.Remove();

                using (var e = new Security.Cryptography.ElGamal.ElGamalManaged())
                {
                    e.FromXmlString(publicKey);
                    return e.VerifySignature(Encoding.UTF8.GetBytes(ToString(SaveOptions.DisableFormatting)),
                                             Convert.FromBase64String(signTag.Value));
                }
            }
            finally
            {
                Add(signTag);
            }
        }

        /// <summary>
        /// Loads a <see cref="License"/> from a <see cref="XElement"/> that contains the License XML.
        /// </summary>
        /// <param name="element">A <see cref="XElement"/> that contains the License XML.</param>
        /// <returns>A <see cref="License"/> populated from the <see cref="XElement"/> that contains the License XML.</returns>
        internal static License Load(XElement element)
        {
            if (element == null)
                throw new ArgumentNullException("element", "Argument cannot be null.");

            if (element.Name != "License")
                throw new ArgumentException("XML is not a License.", "element");

            return new License(element.Elements(), element.Attributes());
        }
    }
}