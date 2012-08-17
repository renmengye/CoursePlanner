
using System;
namespace Panta.Indexing
{
    /// <summary>
    /// Signs a unique id
    /// </summary>
    [Serializable]
    public class IdSigner
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
        public uint SignId(){
            lock (this)
            {
                return this.Next++;
            }
        }
    }
}
