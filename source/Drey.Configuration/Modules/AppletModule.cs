using Drey.Configuration.Services;
using Nancy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Drey.Configuration.Modules
{
    public class AppletModule : BaseModule
    {
        IPackageService _packageService;

        public AppletModule(IGlobalSettingsService globalSettingsService, IPackageService packageService) : base(globalSettingsService, "/applet/{id}")
        {
            _packageService = packageService;

            Get["/"] = Dashboard;
        }

        private dynamic Dashboard(dynamic arg)
        {
            return Negotiate.WithView("index").WithModel(_packageService.Dashboard((string)arg.id));
        }
    }
}