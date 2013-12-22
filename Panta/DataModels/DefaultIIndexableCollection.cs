using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using Panta.Fetchers;
using Panta.Indexing;

namespace Panta.DataModels
{
    /// <summary>
    /// A school that offers courses to students
    /// </summary>
    [Serializable]
    public class DefaultIIndexableCollection<T> : IName, IIndexableCollection<T> where T : IIndexable
    {
        public string Name { get; set; }
        public string Abbr { get; set; }

        /// <summary>
        /// Final storage of the couses once got from the department
        /// </summary>
        public Dictionary<uint, T> IIndexableItemsCatalog { get; set; }

        public IEnumerable<T> Items { get { return IIndexableItemsCatalog.Values; } }

        public DefaultIIndexableCollection(string name, string abbr, IdSigner<T> signer, IEnumerable<T> items)
        {
            this.Name = name;
            this.Abbr = abbr;
            this.IIndexableItemsCatalog = new Dictionary<uint, T>();
            foreach (T item in items)
            {
                // Generate course universal id as the key (not the course name any more)
                this.IIndexableItemsCatalog.Add(signer.SignId(item), item);
            }
        }

        public DefaultIIndexableCollection()
        {
        }

        public bool TryGetItem(uint id, out T item)
        {
            return this.IIndexableItemsCatalog.TryGetValue(id, out item);
        }

        #region Read/Save
        // Read a school from a serialized binary file
        public static DefaultIIndexableCollection<T> ReadBin(string path)
        {
            IFormatter formatter = new BinaryFormatter();
            Stream stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);
            DefaultIIndexableCollection<T> obj = (DefaultIIndexableCollection<T>)formatter.Deserialize(stream);
            stream.Close();
            return obj;
        }

        // Save the instance to a serialized binary file
        public void SaveBin()
        {
            IFormatter formatter = new BinaryFormatter();
            Stream stream = new FileStream(this.Abbr + ".bin", FileMode.Create, FileAccess.Write, FileShare.None);
            formatter.Serialize(stream, this);
            stream.Close();
        }

        #endregion
    }
}
