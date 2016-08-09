using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GaLib.Interop
{
    public interface IObjectMapper
    {
        object GetObject(string type, Guid id);
        Guid GetId(object obj);
    }
}
