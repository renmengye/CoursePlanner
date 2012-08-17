using System.Collections.Generic;

namespace Panta.Formatters
{
    /// <summary>
    /// A formatter that reads a string and converts to a list of certain type objects
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IWebpageFormatter<T>
    {
        /// <summary>
        /// A url to fetch content from
        /// </summary>
        string Url { get; }

        /// <summary>
        /// Read the webpage from Url and format into a enumerable list of items of T
        /// </summary>
        /// <returns>a enumerable list of items of T</returns>
        IEnumerable<T> Read();
    }
}
