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
using Portable.Licensing.Security.Cryptography.Math;

namespace Portable.Licensing.Security.Cryptography.ElGamal
{
    internal class ElGamalManaged : ElGamal
    {
        private ElGamalKeyStruct o_key_struct;

        public ElGamalManaged()
        {
            // create the key struct
            o_key_struct = new ElGamalKeyStruct();
            // set all of the big integers to zero
            o_key_struct.P = new BigInteger(0);
            o_key_struct.G = new BigInteger(0);
            o_key_struct.Y = new BigInteger(0);
            o_key_struct.X = new BigInteger(0);
            // set the default key size value
            KeySizeValue = 1024;
            // set the range of legal keys
            LegalKeySizesValue = new KeySizes[] { new KeySizes(384, 1088, 8) };
        }

        public override string SignatureAlgorithm
        {
            get
            {
                return "ElGamal";
            }
        }

        public override string KeyExchangeAlgorithm
        {
            get
            {
                return "ElGamal";
            }
        }

        public override byte[] EncryptData(byte[] p_data)
        {
            if (NeedToGenerateKey())
            {
                // we need to create a new key before we can export 
                CreateKeyPair(KeySizeValue);
            }
            // encrypt the data
            ElGamalEncryptor x_enc = new ElGamalEncryptor(o_key_struct);
            return x_enc.ProcessData(p_data);
        }

        public override byte[] DecryptData(byte[] p_data)
        {
            if (NeedToGenerateKey())
            {
                // we need to create a new key before we can export 
                CreateKeyPair(KeySizeValue);
            }
            // encrypt the data
            ElGamalDecryptor x_enc = new ElGamalDecryptor(o_key_struct);
            return x_enc.ProcessData(p_data);
        }

        public override byte[] Sign(byte[] p_hashcode)
        {
            if (NeedToGenerateKey())
            {
                // we need to create a new key before we can export 
                CreateKeyPair(KeySizeValue);
            }
            return ElGamalSignature.CreateSignature(p_hashcode, o_key_struct);
        }

        public override bool VerifySignature(byte[] p_hashcode, byte[] p_signature)
        {
            if (NeedToGenerateKey())
            {
                // we need to create a new key before we can export 
                CreateKeyPair(KeySizeValue);
            }
            return ElGamalSignature.VerifySignature(p_hashcode, p_signature, o_key_struct);
        }


        public override ElGamalParameters ExportParameters(bool p_include_private_params)
        {
            if (NeedToGenerateKey())
            {
                // we need to create a new key before we can export 
                CreateKeyPair(KeySizeValue);
            }
            // create the parameter set
            ElGamalParameters x_params = new ElGamalParameters();
            // set the public values of the parameters
            x_params.P = o_key_struct.P.getBytes();
            x_params.G = o_key_struct.G.getBytes();
            x_params.Y = o_key_struct.Y.getBytes();
            // if required, include the private value, X
            if (p_include_private_params)
            {
                x_params.X = o_key_struct.X.getBytes();
            }
            else
            {
                // ensure that we zero the value
                x_params.X = new byte[1];
            }
            // retuen the parameter set
            return x_params;
        }

        /// <summary>
        /// Do we need to generate a key?
        /// </summary>
        /// <returns></returns>
        private bool NeedToGenerateKey()
        {
            return o_key_struct.P == 0 && o_key_struct.G == 0 && o_key_struct.Y == 0;
        }

        public ElGamalKeyStruct KeyStruct
        {
            get
            {
                if (NeedToGenerateKey())
                {
                    CreateKeyPair(KeySizeValue);
                }
                return o_key_struct;
            }
            set
            {
                o_key_struct = value;
            }
        }

        /// <summary>
        /// Configure the key settings for this algorithm instance from
        /// a set of key parameters
        /// </summary>
        /// <param name="p_parameters"></param>
        public override void ImportParameters(ElGamalParameters p_parameters)
        {
            // obtain the  big integer values from the byte parameter values
            o_key_struct.P = new BigInteger(p_parameters.P);
            o_key_struct.G = new BigInteger(p_parameters.G);
            o_key_struct.Y = new BigInteger(p_parameters.Y);
            if (p_parameters.X != null && p_parameters.X.Length > 0)
            {
                o_key_struct.X = new BigInteger(p_parameters.X);
            }
            // set the length of the key based on the import
            KeySizeValue = o_key_struct.P.bitCount();
        }

        protected override void Dispose(bool p_bool)
        {
            // do nothing - no managed resources to release
        }

        private void CreateKeyPair(int p_key_strength)
        {
            // create the random number generator
            Random x_random_generator = new Random();
            // create the large prime number, P
            o_key_struct.P = BigInteger.genPseudoPrime(p_key_strength, 16, x_random_generator);
            // create the two random numbers, which are smaller than P
            o_key_struct.X = new BigInteger();
            o_key_struct.X.genRandomBits(p_key_strength - 1, x_random_generator);
            o_key_struct.G = new BigInteger();
            o_key_struct.G.genRandomBits(p_key_strength - 1, x_random_generator);
            // compute Y
            o_key_struct.Y = o_key_struct.G.modPow(o_key_struct.X, o_key_struct.P);
        }
    }
}