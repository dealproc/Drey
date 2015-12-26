using Autofac;

using System.Linq;

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
