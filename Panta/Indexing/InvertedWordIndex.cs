using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using Panta.Indexing.Expressions;
using System.Linq;

namespace Panta.Indexing
{
    [Serializable]
    public class InvertedWordIndex : IIdProvider
    {
        private SortedDictionary<string, HashSet<uint>> IndexEntries { get; set; }
        public string Name { get; set; }

        public string[] SortedKeys
        {
            get
            {
                return IndexEntries.Keys.ToArray();
            }
        }

        public HashSet<uint> GetMatchedIDs(string key)
        {
            HashSet<uint> results;
            if (IndexEntries.TryGetValue(key, out results))
            {
                return results;
            }
            else
            {
                return (results = new HashSet<uint>());
            }

        }

        public InvertedWordIndex() : this("Current") { }

        public InvertedWordIndex(string name)
        {
            this.Name = name;
            this.IndexEntries = new SortedDictionary<string, HashSet<uint>>();
        }

        public void Add(IIndexable item)
        {
            foreach (string s in item.GetSplittedIndexStrings())
            {
                HashSet<uint> collection = new HashSet<uint>();

                // Index only root
                if (!IndexEntries.TryGetValue(s, out collection))
                {
                    collection = new HashSet<uint>();
                    IndexEntries.Add(s, collection);
                }
                collection.Add(item.ID);
            }
        }

        #region IIdProvider implementation
        public bool TryGetValue(string key, out ICollection<uint> value)
        {
            HashSet<uint> tryValue;
            bool result = this.IndexEntries.TryGetValue(key, out tryValue);
            value = tryValue;
            return result;
        }
        #endregion

        #region Read/Save
        // Read a school from a serialized binary file
        public static InvertedWordIndex Read(string path)
        {
            IFormatter formatter = new BinaryFormatter();
            Stream stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);
            InvertedWordIndex obj = formatter.Deserialize(stream) as InvertedWordIndex;
            stream.Close();
            return obj;
        }

        // Save the instance to a serialized binary file
        public void Save()
        {
            IFormatter formatter = new BinaryFormatter();
            Stream stream = new FileStream(this.Name + ".idx", FileMode.Create, FileAccess.Write, FileShare.None);
            formatter.Serialize(stream, this);
            stream.Close();
        }
        #endregion
    }
}
