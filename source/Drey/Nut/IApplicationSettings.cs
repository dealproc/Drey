using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Drey.Nut
{
    public interface IApplicationSettings
    {
        string this[string key] { get; }
    }
}
