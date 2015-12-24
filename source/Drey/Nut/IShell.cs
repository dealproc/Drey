using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Drey.Nut
{
    public interface IShell : IDisposable
    {
        string Id { get; }
        bool RequiresConfigurationStorage { get; }
        IEnumerable<DefaultAppSetting> AppSettingDefaults { get; }
        IEnumerable<DefaultConnectionString> ConnectionStringDefaults { get; }

        event EventHandler<ShellRequestArgs> OnShellRequest;
        EventHandler<ShellRequestArgs> ShellRequestHandler { get; set; }

        bool Startup(INutConfiguration configurationManager);
        void Shutdown();

        Action<INutConfiguration> ConfigureLogging { get; set; }
    }
}