using Nancy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Samples.Server.Modules {
    public class HomeModule : NancyModule {
        public HomeModule() : base("/") {
            Get["/"] = _ => "Hello World!";
        }
    }
}
