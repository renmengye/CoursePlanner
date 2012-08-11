using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Panta
{
    public interface IFormatReader<T>
    {
        IEnumerable<T> Read();
    }
}
