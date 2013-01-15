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

using System.IO;
using System.Xml;
using System.Xml.Linq;
using Portable.Licensing.Model;

namespace Portable.Licensing
{
    /// <summary>
    /// Static factory class to create new <see cref="ILicense"/>.
    /// </summary>
    public static class LicenseFactory
    {
        /// <summary>
        /// Create a new <see cref="ILicense"/> using the <see cref="ILicenseBuilder"/>
        /// fluent api.
        /// </summary>
        /// <returns>An instance of the <see cref="ILicenseBuilder"/> class.</returns>
        public static ILicenseBuilder New()
        {
            return new LicenseBuilder();
        }

        /// <summary>
        /// Loads a <see cref="ILicense"/> from a string that contains XML.
        /// </summary>
        /// <param name="xmlString">A <see cref="string"/> that contains XML.</param>
        /// <returns>A <see cref="ILicense"/> populated from the <see cref="string"/> that contains XML.</returns>
        public static ILicense Load(string xmlString)
        {
            return License.Load(XElement.Parse(xmlString, LoadOptions.None));
        }

        /// <summary>
        /// Loads a <see cref="ILicense"/> by using the specified <see cref="Stream"/>
        /// that contains the XML.
        /// </summary>
        /// <param name="stream">A <see cref="Stream"/> that contains the XML.</param>
        /// <returns>A <see cref="ILicense"/> populated from the <see cref="Stream"/> that contains XML.</returns>
        public static ILicense Load(Stream stream)
        {
            return License.Load(XElement.Load(stream, LoadOptions.None));
        }

        /// <summary>
        /// Loads a <see cref="ILicense"/> by using the specified <see cref="TextReader"/>
        /// that contains the XML.
        /// </summary>
        /// <param name="reader">A <see cref="TextReader"/> that contains the XML.</param>
        /// <returns>A <see cref="ILicense"/> populated from the <see cref="TextReader"/> that contains XML.</returns>
        public static ILicense Load(TextReader reader)
        {
            return License.Load(XElement.Load(reader, LoadOptions.None));
        }

        /// <summary>
        /// Loads a <see cref="ILicense"/> by using the specified <see cref="XmlReader"/>
        /// that contains the XML.
        /// </summary>
        /// <param name="reader">A <see cref="XmlReader"/> that contains the XML.</param>
        /// <returns>A <see cref="ILicense"/> populated from the <see cref="TextReader"/> that contains XML.</returns>
        public static ILicense Load(XmlReader reader)
        {
            return License.Load(XElement.Load(reader, LoadOptions.None));
        }
    }
}