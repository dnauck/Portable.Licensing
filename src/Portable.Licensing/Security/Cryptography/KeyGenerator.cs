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
using Portable.Licensing.Model;

namespace Portable.Licensing.Security.Cryptography
{
    /// <summary>
    /// Represents a generator for signature keys of <see cref="License"/>.
    /// </summary>
    public class KeyGenerator : IDisposable
    {
        private readonly AsymmetricAlgorithm hashAlgorithm;

        /// <summary>
        /// Initializes a new instance of the <see cref="KeyGenerator"/> class.
        /// </summary>
        public KeyGenerator()
            : this(1088)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="KeyGenerator"/> class
        /// with the specified key size.
        /// </summary>
        /// <param name="keySize">The key size.</param>
        internal KeyGenerator(int keySize)
        {
            hashAlgorithm = AsymmetricAlgorithm.Create();
            hashAlgorithm.KeySize = keySize;
        }

        /// <summary>
        /// Creates a new instance of the <see cref="KeyGenerator"/> class.
        /// </summary>
        public static KeyGenerator Create()
        {
            return new KeyGenerator();
        }

        public void Dispose()
        {
        }

        /// <summary>
        /// Creates and returns an XML string containing the key.
        /// </summary>
        /// <param name="includePrivateParameters">true to include a public and private key; false to include only the public key.</param>
        /// <returns>An XML string containing the key.</returns>
        public string ToXmlString(bool includePrivateParameters)
        {
            return hashAlgorithm.ToXmlString(includePrivateParameters);
        }
    }
}