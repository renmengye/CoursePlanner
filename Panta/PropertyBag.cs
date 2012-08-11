using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Panta
{
    [Serializable]
    public class PropertyBag
    {
        private Dictionary<string, string> Fields { get; set; }

        public PropertyBag()
        {
            Fields = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        }

        public string this[string fieldName]
        {
            get
            {
                string value = null;
                Fields.TryGetValue(fieldName, out value);
                return value ?? String.Empty;
            }
            set
            {
                Add(fieldName, value);
            }
        }

        public void Add(string fieldName, string value)
        {
            Fields[fieldName] = value;
        }

        public void Remove(string fieldName)
        {
            Fields.Remove(fieldName);
        }
    }
}
