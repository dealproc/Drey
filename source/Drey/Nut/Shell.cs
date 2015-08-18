using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Drey.Nut
{
    public class Shell : MarshalByRefObject, Drey.Nut.IShell
    {
        ShellStartOptions _options;
        AppDomain _hostedApplication;
        StartupProxy _Startup;
        string _packageId = string.Empty;

        public string PackageId
        {
            get { return _packageId; }
        }

        public Shell(ShellStartOptions options, Drey.Nut.INutConfiguration config)
        {
            _options = options;
            _packageId = options.PackageId;

            _hostedApplication = Utilities.AppDomainUtils.CreateDomain(options.ApplicationDomainName);

            _Startup = (StartupProxy)_hostedApplication.CreateInstanceFromAndUnwrap(typeof(StartupProxy).Assembly.Location, typeof(StartupProxy).FullName);

            _Startup.SetStartOptions(_options);
            _hostedApplication.AssemblyResolve += _Startup.ResolveAssemblyInDomain;
            _Startup.Instantiate(options.DllPath, options.StartupClass);

            if (options.ProvideConfigurationOptions)
            {
                _Startup.Invoke("Configuration", config);
            }
            else
            {
                _Startup.Invoke("Configuration");
            }
        }



        public Task Shutdown()
        {
            return Task.Factory.StartNew(() => _Startup.Shutdown());
        }

        public void Dispose()
        {
            try
            {
                if (_Startup != null)
                {
                    _Startup.Dispose();
                    _Startup = null;
                }
                AppDomain.Unload(_hostedApplication);
            }
            catch (Exception)
            {
                // squelch.. should we log?
            }
        }
    }
}