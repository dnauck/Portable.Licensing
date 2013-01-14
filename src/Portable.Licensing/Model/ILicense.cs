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

namespace Portable.Licensing.Model
{
    /// <summary>
    /// A software license
    /// </summary>
    public interface ILicense
    {
        /// <summary>
        /// Gets or sets the unique identifier of this <see cref="ILicense"/>.
        /// </summary>
        Guid Id { get; set; }

        /// <summary>
        /// Gets or set the <see cref="LicenseType"/> or this <see cref="ILicense"/>.
        /// </summary>
        LicenseType Type { get; set; }

        /// <summary>
        /// Get or sets the quantity of this license.
        /// E.g. the count of per-developer-licenses.
        /// </summary>
        int Quantity { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IProductFeatures"/> of this <see cref="ILicense"/>.
        /// </summary>
        IProductFeatures ProductFeatures { get; }

        /// <summary>
        /// Gets or sets the <see cref="ICustomer"/> of this <see cref="ILicense"/>.
        /// </summary>
        ICustomer Customer { get; }

        /// <summary>
        /// Gets or sets the expiration date of this <see cref="ILicense"/>.
        /// Use this property to set the expiration date for a trial license
        /// or the expiration of support & subscription updates for a standard license.
        /// </summary>
        DateTime Expiration { get; set; }

        /// <summary>
        /// Gets the digital signature of this license.
        /// </summary>
        /// <remarks>Use the <see cref="ILicense.Sign"/> method to compute a signature.</remarks>
        string Signature { get; }

        /// <summary>
        /// Compute a signature and sign this <see cref="ILicense"/> with the provided key.
        /// </summary>
        /// <param name="privateKey">The private key in xml string format to compute the signature.</param>
        void Sign(string privateKey);

        /// <summary>
        /// Determines whether the <see cref="ILicense.Signature"/> property verifies for the specified key.
        /// </summary>
        /// <param name="publicKey">The public key in xml string format to verify the <see cref="ILicense.Signature"/>.</param>
        /// <returns>true if the <see cref="ILicense.Signature"/> verifies; otherwise false.</returns>
        bool VerifySignature(string publicKey);
    }
}