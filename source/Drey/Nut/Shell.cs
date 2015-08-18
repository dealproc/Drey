using System;
using System.IO;
using System.Security.Policy;
using System.Threading.Tasks;

namespace Drey.Nut
{
    public class Shell : MarshalByRefObject, Drey.Nut.IShell
    {
        AppDomain _hostedApplication;
        StartupProxy _Startup;
        string _packageId = string.Empty;
        string _instanceId = string.Empty;

        public string PackageId
        {
            get { return _packageId; }
        }
        
        public string InstanceId
        {
            get { return _instanceId; }
        }

        public Shell(ShellStartOptions options, Drey.Nut.INutConfiguration config)
        {
            _instanceId = Guid.NewGuid().ToString();

            var domainSetup = new AppDomainSetup();
            domainSetup.ApplicationBase = Path.GetDirectoryName(options.DllPath);
            Evidence adEvidence = AppDomain.CurrentDomain.Evidence;

            _hostedApplication = AppDomain.CreateDomain(options.ApplicationDomainName, adEvidence, domainSetup);

            _Startup = (StartupProxy)_hostedApplication.CreateInstanceFromAndUnwrap(typeof(StartupProxy).Assembly.Location, typeof(StartupProxy).FullName);
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