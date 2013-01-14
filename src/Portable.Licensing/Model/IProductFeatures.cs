using System.Collections.Generic;

namespace Portable.Licensing.Model
{
    /// <summary>
    /// Represents a dictionary of product features.
    /// </summary>
    public interface IProductFeatures
    {
        /// <summary>
        /// Adds a new feature with the specified key and value
        /// to the collection.
        /// </summary>
        /// <param name="key">The key of the feature.</param>
        /// <param name="value">The value of the feature.</param>
        void Add(string key, string value);

        /// <summary>
        /// Adds all new features into the collection.
        /// </summary>
        /// <param name="features">The dictionary of features.</param>
        void AddAll(IDictionary<string, string> features);

        /// <summary>
        /// Removes a feature with the specified key
        /// from the collecton.
        /// </summary>
        /// <param name="key">The key of the feature.</param>
        void Remove(string key);

        /// <summary>
        /// Removes all features from the collection.
        /// </summary>
        new void RemoveAll();

        /// <summary>
        /// Gets the value of a feature with the
        /// specified key.
        /// </summary>
        /// <param name="key">The key of the feature.</param>
        /// <returns>The value of the feature if available; otherwiese null.</returns>
        string Get(string key);

        /// <summary>
        /// Gets all features.
        /// </summary>
        /// <returns>A dictionary of all features in this collection.</returns>
        IDictionary<string, string> GetAll();

        /// <summary>
        /// Determines whether the specified feature is in
        /// this collection.
        /// </summary>
        /// <param name="key">The key of the feature.</param>
        /// <returns>true if the collection contains this feature; otherwise false.</returns>
        bool Contains(string key);

        /// <summary>
        /// Determines whether all specified features are in
        /// this collection.
        /// </summary>
        /// <param name="keys">The list of keys of the features.</param>
        /// <returns>true if the collection contains all specified feature; otherwise false.</returns>
        bool ContainsAll(string[] keys);
    }
}