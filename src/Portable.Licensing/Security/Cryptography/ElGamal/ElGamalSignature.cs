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
    internal class ElGamalSignature
    {

        public static BigInteger mod(BigInteger p_base, BigInteger p_val)
        {
            BigInteger x_result = p_base % p_val;
            if (x_result < 0)
            {
                x_result += p_val;
            }
            return x_result;
        }

        public static byte[] CreateSignature(byte[] p_data, ElGamalKeyStruct p_key_struct)
        {
            // define P -1
            BigInteger x_pminusone = p_key_struct.P - 1;
            // create K, which is the random number        
            BigInteger K;
            do
            {
                K = new BigInteger();
                K.genRandomBits(p_key_struct.P.bitCount() - 1, new Random());
            } while (K.gcd(x_pminusone) != 1);

            // compute the values A and B
            BigInteger A = p_key_struct.G.modPow(K, p_key_struct.P);
            BigInteger B = mod(mod(K.modInverse(x_pminusone) * (new BigInteger(p_data) - (p_key_struct.X * (A))), (x_pminusone)), (x_pminusone));
            // copy the bytes from A and B into the result array
            byte[] x_a_bytes = A.getBytes();
            byte[] x_b_bytes = B.getBytes();
            // define the result size
            int x_result_size = (((p_key_struct.P.bitCount() + 7) / 8) * 2);
            // create an array to contain the ciphertext
            byte[] x_result = new byte[x_result_size];
            // populate the arrays
            Array.Copy(x_a_bytes, 0, x_result, x_result_size / 2 - x_a_bytes.Length, x_a_bytes.Length);
            Array.Copy(x_b_bytes, 0, x_result, x_result_size - x_b_bytes.Length, x_b_bytes.Length);
            // return the result array
            return x_result;
        }

        public static bool VerifySignature(byte[] p_data, byte[] p_sig, ElGamalKeyStruct p_key_struct)
        {
            // define the result size
            int x_result_size = p_sig.Length / 2;
            // extract the byte arrays that represent A and B
            byte[] x_a_bytes = new byte[x_result_size];
            Array.Copy(p_sig, 0, x_a_bytes, 0, x_a_bytes.Length);
            byte[] x_b_bytes = new Byte[x_result_size];
            Array.Copy(p_sig, x_result_size, x_b_bytes, 0, x_b_bytes.Length);
            // create big integers from the byte arrays
            BigInteger A = new BigInteger(x_a_bytes);
            BigInteger B = new BigInteger(x_b_bytes);
            // create the two results
            BigInteger x_result1 = mod(p_key_struct.Y.modPow(A, p_key_struct.P)
                * A.modPow(B, p_key_struct.P), p_key_struct.P);
            BigInteger x_result2 = p_key_struct.G.modPow(new BigInteger(p_data), p_key_struct.P);
            // return true if the two results are the same
            return x_result1 == x_result2;
        }
    }
}