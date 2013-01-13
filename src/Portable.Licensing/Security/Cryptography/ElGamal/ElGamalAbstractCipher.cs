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
using System.IO;

namespace Portable.Licensing.Security.Cryptography.ElGamal
{
    internal abstract class ElGamalAbstractCipher
    {
        protected int o_block_size;
        protected int o_plaintext_blocksize;
        protected int o_ciphertext_blocksize;
        protected ElGamalKeyStruct o_key_struct;

        public ElGamalAbstractCipher(ElGamalKeyStruct p_key_struct)
        {
            o_key_struct = p_key_struct;
            // calculate the blocksizes
            o_plaintext_blocksize = (p_key_struct.P.bitCount() - 1) / 8;
            o_ciphertext_blocksize = ((p_key_struct.P.bitCount() + 7) / 8) * 2;
            // set the default block for plaintext, which is suitable for encryption
            o_block_size = o_plaintext_blocksize;
        }

        public byte[] ProcessData(byte[] p_data)
        {

            // create a stream backed by a memory array
            MemoryStream x_stream = new MemoryStream();
            // determine how many complete blocks there are
            int x_complete_blocks = p_data.Length / o_block_size;

            // create an array which will hold a block
            byte[] x_block = new Byte[o_block_size];

            // run through and process the complete blocks
            int i = 0;
            for (; i < x_complete_blocks; i++)
            {
                // copy the block and create the big integer
                Array.Copy(p_data, i * o_block_size, x_block, 0, o_block_size);
                // process the block
                byte[] x_result = ProcessDataBlock(x_block);
                // write the processed data into the stream
                x_stream.Write(x_result, 0, x_result.Length);
            }

            // process the final block
            byte[] x_final_block = new Byte[p_data.Length - (x_complete_blocks * o_block_size)];
            Array.Copy(p_data, i * o_block_size, x_final_block, 0, x_final_block.Length);
            // process the final block
            byte[] x_final_result = ProcessFinalDataBlock(x_final_block);
            // write the final data bytes into the stream
            x_stream.Write(x_final_result, 0, x_final_result.Length);
            // return the contents of the stream as a byte array
            return x_stream.ToArray();
        }

        protected abstract byte[] ProcessDataBlock(byte[] p_block);

        protected abstract byte[] ProcessFinalDataBlock(byte[] p_final_block);
    }
}