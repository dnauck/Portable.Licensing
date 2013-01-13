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
    internal class ElGamalDecryptor : ElGamalAbstractCipher
    {

        public ElGamalDecryptor(ElGamalKeyStruct p_struct)
            : base(p_struct)
        {
            // set the default block size to be ciphertext
            o_block_size = o_ciphertext_blocksize;
        }

        protected override byte[] ProcessDataBlock(byte[] p_block)
        {
            // extract the byte arrays that represent A and B
            byte[] x_a_bytes = new byte[o_ciphertext_blocksize / 2];
            Array.Copy(p_block, 0, x_a_bytes, 0, x_a_bytes.Length);
            byte[] x_b_bytes = new Byte[o_ciphertext_blocksize / 2];
            Array.Copy(p_block, x_a_bytes.Length, x_b_bytes, 0, x_b_bytes.Length);
            // create big integers from the byte arrays
            BigInteger A = new BigInteger(x_a_bytes);
            BigInteger B = new BigInteger(x_b_bytes);
            // calculate the value M
            BigInteger M = (B * A.modPow(o_key_struct.X, o_key_struct.P).modInverse(o_key_struct.P)) % o_key_struct.P;
            // return the result - take care to ensure that we create
            // a result which is the correct length
            byte[] x_m_bytes = M.getBytes();
            // we may end up with results which are short some leading
            // bytes - add these are required
            if (x_m_bytes.Length < o_plaintext_blocksize)
            {
                byte[] x_full_result = new byte[o_plaintext_blocksize];
                Array.Copy(x_m_bytes, 0, x_full_result, o_plaintext_blocksize - x_m_bytes.Length, x_m_bytes.Length);
                x_m_bytes = x_full_result;
            }
            return x_m_bytes;
        }

        protected override byte[] ProcessFinalDataBlock(byte[] p_final_block)
        {
            if (p_final_block.Length > 0)
            {
                return ProcessDataBlock(p_final_block);
            }
            return new byte[0];
        }
    }
}