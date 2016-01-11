using Autofac;

using Drey.Nut;

namespace Drey.Configuration.Infrastructure.IoC
{
    /// <summary>
    /// Basic autoconfiguration routine(s).
    /// </summary>
    public class AutofacConfigModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterAssemblyTypes(ThisAssembly)
                .Where(t => t.Name.EndsWith("Service"))
                .AsImplementedInterfaces()
                .InstancePerLifetimeScope();

            // For Repositories, we provide two constructors.
            // the constructor we want Autofac to use takes an INutConfiguration instance.
            // the alternative constructor is used specifically for integration testing.
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
