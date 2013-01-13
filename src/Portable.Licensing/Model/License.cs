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
using System.Xml.Linq;

namespace Portable.Licensing.Model
{
    /// <summary>
    /// A software license
    /// </summary>
    public class License : XElement
    {
        private static readonly XNamespace ns = "http://schema.nauck-it.de/portable.licensing#license";

        public License()
            : base(ns + "License")
        {
        }

        public Guid Id
        {
            get { return new Guid(Element(ns + "Id").Value); }
            set { SetElementValue(ns + "Id", value); }
        }

        public LicenseType Type
        {
            get { return (LicenseType) Enum.Parse(typeof (LicenseType), Element(ns + "Type").Value, false); }
            set { Add(new XElement(ns + "Type", value)); }
        }

        public int Quantity
        {
            get { return int.Parse(Element(ns + "Quantity").Value); }
            set { Add(new XElement(ns + "Quantity", value)); }
        }

        public Customer Customer
        {
            get { return Element("Customer") as Customer; }
            set { Add(value); }
        }

        public DateTime Expiration
        {
            get { return DateTime.ParseExact(Element(ns + "Expiration").Value, "r", CultureInfo.InvariantCulture); }
            set { Add(new XElement(ns + "Expiration", value.ToUniversalTime().ToString("r", CultureInfo.InvariantCulture))); }
        }
    }
}