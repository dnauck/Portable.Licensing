﻿//
// Copyright © 2012 - 2013 AG-Software     http://www.ag-software.de
//
// Author:
//  Alexander Gnauck        <gnauck(at)ag-software.de>
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

namespace Portable.Licensing.Security.Cryptography
{
    internal abstract class AsymmetricAlgorithm : IDisposable
    {
        protected int KeySizeValue;
        protected KeySizes[] LegalKeySizesValue;
        
        public abstract string KeyExchangeAlgorithm
        {
            get;
        }

        public virtual int KeySize
        {
            get { return KeySizeValue; }
            set
            {
                if (!IsLegalKeySize(LegalKeySizesValue, value))
                    throw new CryptographicException("Key size not supported by algorithm.");

                KeySizeValue = value;
            }
        }

        internal bool IsLegalKeySize(KeySizes[] legalKeys, int size)
        {
            foreach (KeySizes legalKeySize in legalKeys)
            {
                if (IsLegalKeySize(size, legalKeySize))
                    return true;
            }
            return false;
        }

        internal bool IsLegalKeySize(int keySize, KeySizes keySizes)
        {
            int ks = keySize - keySizes.MinSize;
            bool result = ((ks >= 0) && (keySize <= keySizes.MaxSize));
            return ((keySizes.SkipSize == 0) ? result : (result && (ks % keySizes.SkipSize == 0)));
        }

        public virtual KeySizes[] LegalKeySizes
        {
            get { return LegalKeySizesValue; }
        }

        public abstract string SignatureAlgorithm
        {
            get;
        }

        void IDisposable.Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);  // Finalization is now unnecessary
        }

        public void Clear()
        {
            Dispose(false);
        }

        protected abstract void Dispose(bool disposing);

        public abstract void FromXmlString(string xmlString);

        public abstract string ToXmlString(bool includePrivateParameters);

        public static AsymmetricAlgorithm Create()
        {
            return Create("Portable.Licensing.Security.Cryptography.ElGamal.ElGamalManaged");
        }

        public static AsymmetricAlgorithm Create(string algName)
        {
            return (AsymmetricAlgorithm) Activator.CreateInstance(Type.GetType(algName));
        }
    }
}