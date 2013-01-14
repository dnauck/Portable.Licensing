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
    public class License : XElement
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="License"/> class.
        /// </summary>
        public License()
            : base("License")
        {
        }

        /// <summary>
        /// Gets or sets the unique identifier of this <see cref="License"/>.
        /// </summary>
        public Guid Id
        {
            get { return new Guid(Element("Id").Value); }
            set { SetElementValue("Id", value); }
        }

        /// <summary>
        /// Gets or set the <see cref="LicenseType"/> or this <see cref="License"/>.
        /// </summary>
        public LicenseType Type
        {
            get { return (LicenseType) Enum.Parse(typeof (LicenseType), Element("Type").Value, false); }
            set { Add(new XElement("Type", value)); }
        }

        /// <summary>
        /// Get or sets the quantity of this license.
        /// E.g. the count of per-developer-licenses.
        /// </summary>
        public int Quantity
        {
            get { return int.Parse(Element("Quantity").Value); }
            set { Add(new XElement("Quantity", value)); }
        }

        /// <summary>
        /// Gets or sets the <see cref="Customer"/> of this <see cref="License"/>.
        /// </summary>
        public Customer Customer
        {
            get { return Element("Customer") as Customer; }
            set { Add(value); }
        }

        /// <summary>
        /// Gets or sets the expiration date of this <see cref="License"/>.
        /// Use this property to set the expiration date for a trial license
        /// or the expiration of support&subscription updates for a standard license.
        /// </summary>
        public DateTime Expiration
        {
            get { return DateTime.ParseExact(Element("Expiration").Value, "r", CultureInfo.InvariantCulture); }
            set { Add(new XElement("Expiration", value.ToUniversalTime().ToString("r", CultureInfo.InvariantCulture))); }
        }

        /// <summary>
        /// Gets the digital signature of this license.
        /// </summary>
        /// <remarks>Use the <see cref="License.Sign"/> method to compute a signature.</remarks>
        public string Signature
        {
            get { return Element("Signature").Value; }
        }

        /// <summary>
        /// Compute a signature and sign this <see cref="License"/> with the provided key.
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
        /// Determines whether the <see cref="License.Signature"/> property verifies for the specified key.
        /// </summary>
        /// <param name="publicKey">The public key in xml string format to verify the <see cref="License.Signature"/>.</param>
        /// <returns>true if the <see cref="License.Signature"/> verifies; otherwise false.</returns>
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
    }
}