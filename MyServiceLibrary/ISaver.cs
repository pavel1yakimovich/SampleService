using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyServiceLibrary
{
    public interface ISaver<T>
    {
        void Save(IEnumerable<T> list);
    }
}
