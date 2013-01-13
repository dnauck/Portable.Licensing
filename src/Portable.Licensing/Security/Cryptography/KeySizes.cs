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

namespace Portable.Licensing.Security.Cryptography
{
    public sealed class KeySizes
    {
        // Fields
        private int m_maxSize;
        private int m_minSize;
        private int m_skipSize;

        // Methods
        public KeySizes(int minSize, int maxSize, int skipSize)
        {
            this.m_minSize = minSize;
            this.m_maxSize = maxSize;
            this.m_skipSize = skipSize;
        }

        // Properties
        public int MaxSize
        {
            get { return this.m_maxSize; }
        }

        public int MinSize
        {
            get { return this.m_minSize; }
        }

        public int SkipSize
        {
            get { return this.m_skipSize; }
        }
    }
}