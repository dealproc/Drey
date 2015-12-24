using Autofac;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Drey.Configuration.Infrastructure.IoC
{
    public class AutofacConfigModule : Module
    {
        private string[] REGISTRATION_SUFFIXES = new[] { "Repository", "Service" };
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterAssemblyTypes(ThisAssembly)
                .Where(t => REGISTRATION_SUFFIXES.Any(t.Name.EndsWith))
                .AsImplementedInterfaces()
                .InstancePerLifetimeScope();
        }
    }
}
