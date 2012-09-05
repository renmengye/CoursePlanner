
using System;
namespace Panta.Indexing
{
    /// <summary>
    /// Signs a unique id
    /// </summary>
    [Serializable]
    public class IdSigner<T> where T : IIndexable
    {
        private uint Next { get; set; }

        public IdSigner()
        {
            this.Next = 0;
        }

        /// <summary>
        /// Get the next available id
        /// </summary>
        /// <returns>Next available id</returns>
        public uint SignId(T item)
        {
            lock (this)
            {
                item.ID = this.Next;
                return this.Next++;
            }
        }
    }
}
