using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Samples.Server.Infrastructure
{
    class SampleClientRegistry : ConcurrentDictionary<string, string>, Drey.Server.Infrastructure.IClientRegistry { }
}
