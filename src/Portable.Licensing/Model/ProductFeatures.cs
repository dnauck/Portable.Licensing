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

using System.Collections.Generic;
using System.Linq;

namespace Portable.Licensing.Model
{
    /// <summary>
    /// Represents a dictionary of product features.
    /// </summary>
    internal class ProductFeatures : ModelBase, IProductFeatures
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProductFeatures"/> class.
        /// </summary>
        public ProductFeatures()
            : base("ProductFeatures")
        {
        }

        /// <summary>
        /// Adds a new feature with the specified key and value
        /// to the collection.
        /// </summary>
        /// <param name="key">The key of the feature.</param>
        /// <param name="value">The value of the feature.</param>
        public void Add(string key, string value)
        {
            SetTag(key, value);
        }

        /// <summary>
        /// Adds all new features into the collection.
        /// </summary>
        /// <param name="features">The dictionary of features.</param>
        public void AddAll(IDictionary<string, string> features)
        {
            foreach (var feature in features)
                Add(feature.Key, feature.Value);
        }

        /// <summary>
        /// Removes a feature with the specified key
        /// from the collection.
        /// </summary>
        /// <param name="key">The key of the feature.</param>
        public void Remove(string key)
        {
            var element = Element(key);
            if(element != null)
                element.Remove();
        }

        /// <summary>
        /// Removes all features from the collection.
        /// </summary>
        public new void RemoveAll()
        {
            base.RemoveAll();
        }

        /// <summary>
        /// Gets the value of a feature with the
        /// specified key.
        /// </summary>
        /// <param name="key">The key of the feature.</param>
        /// <returns>The value of the feature if available; otherwise null.</returns>
        public string Get(string key)
        {
            return GetTag(key);
        }

        /// <summary>
        /// Gets all features.
        /// </summary>
        /// <returns>A dictionary of all features in this collection.</returns>
        public IDictionary<string, string> GetAll()
        {
            return Elements().ToDictionary(e => e.Name.ToString(), e => e.Value);
        }

        /// <summary>
        /// Determines whether the specified feature is in
        /// this collection.
        /// </summary>
        /// <param name="key">The key of the feature.</param>
        /// <returns>true if the collection contains this feature; otherwise false.</returns>
        public bool Contains(string key)
        {
            return Elements().Any(e => e.Name == key);
        }

        /// <summary>
        /// Determines whether all specified features are in
        /// this collection.
        /// </summary>
        /// <param name="keys">The list of keys of the features.</param>
        /// <returns>true if the collection contains all specified feature; otherwise false.</returns>
        public bool ContainsAll(string[] keys)
        {
            return Elements().All(e => keys.Contains(e.Name.ToString()));
        }
    }
}