using Autofac;

using Drey.Nut;

namespace Drey.Configuration.Infrastructure.IoC
{
    public class AutofacConfigModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterAssemblyTypes(ThisAssembly)
                .Where(t => t.Name.EndsWith("Service"))
                .AsImplementedInterfaces()
                .InstancePerLifetimeScope();

            builder.RegisterAssemblyTypes(ThisAssembly)
                .Where(t => t.Name.EndsWith("Repository"))
                .AsImplementedInterfaces()
                .FindConstructorsWith(t => new[] { 
                    t.GetConstructor(new[] { typeof(INutConfiguration) })
                })
                .InstancePerLifetimeScope();
        }
    }
}
