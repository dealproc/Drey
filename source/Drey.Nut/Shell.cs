using System;
using System.IO;
using System.Reflection;
using System.Security.Policy;

namespace Drey.Nut
{
    public class Shell : MarshalByRefObject, Drey.Nut.IShell
    {
        AppDomain _hostedApplication;
        IDisposable _Startup;
        public event EventHandler<ShellEventArgs> ShellCallback;

        public Shell(ShellStartOptions options)
        {
            var domainSetup = new AppDomainSetup();
            domainSetup.ApplicationBase = Path.GetDirectoryName(options.DllPath);
            //domainSetup.PrivateBinPath = Path.GetDirectoryName(options.DllPath);
            Evidence adEvidence = AppDomain.CurrentDomain.Evidence;

            _hostedApplication = AppDomain.CreateDomain(options.ApplicationDomainName, adEvidence, domainSetup);
                        
            var startup = (StartupProxy)_hostedApplication.CreateInstanceFromAndUnwrap(typeof(StartupProxy).Assembly.Location, typeof(StartupProxy).FullName);
            startup.Instantiate(options.DllPath, options.StartupClass);
            startup.Invoke("Configuration");

            _Startup = startup as IDisposable;
        }

        public void Dispose()
        {
            try
            {
                if (_Startup != null)
                {
                    _Startup.Dispose();
                }
                AppDomain.Unload(_hostedApplication);
            }
            catch (Exception ex)
            {
                // squelch.. should we log?
            }
        }
    }
}