using Autofac;
using Drey.Logging;
using Drey.Nut;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;

namespace Samples.ThirdPartyIntegrations
{
    [Serializable]
    public class Nut : ShellBase
    {
        static ILog _log = LogProvider.For<Nut>();
        string _serviceConfiguration;
        IContainer _container;

        public override bool Startup(INutConfiguration configurationManager)
        {
            _log.Info("Samples - Third Party Integrations Proof of Concept online.");
            if (!base.Startup(configurationManager)) { return false; }

            var bin = Path.GetDirectoryName(GetType().Assembly.CodeBase).Remove(0, 6);
            var configPath = Path.Combine(bin, "Config", "Services.config");
            using (var fReader = new StreamReader(configPath))
                _serviceConfiguration = fReader.ReadToEnd();


            var cb = new ContainerBuilder();

            cb.RegisterType<CoreWriter>()
                .As<IToImplement>();

            var ms = new MemoryStream(Encoding.UTF8.GetBytes(_serviceConfiguration));
            var reader = XmlReader.Create(ms);
            var xmlConfigModule = new XmlConfigurationSettingsReader(reader);
            cb.RegisterModule(xmlConfigModule);

            _container = cb.Build();

            IEnumerable<IToImplement> allWriters = _container.Resolve<IEnumerable<IToImplement>>();

            allWriters.ToList().ForEach(w => w.WriteMessage(_log.Info));
            return true;
        }

        public override string Id
        {
            get { return GetType().Namespace; }
        }

        public override bool RequiresConfigurationStorage
        {
            get { return false; }
        }

        public override IEnumerable<DefaultAppSetting> AppSettingDefaults
        {
            get { return Enumerable.Empty<DefaultAppSetting>(); }
        }

        public override IEnumerable<DefaultConnectionString> ConnectionStringDefaults
        {
            get { return Enumerable.Empty<DefaultConnectionString>(); }
        }

        protected override void Dispose(bool disposing)
        {
            _log.Info("Samples - Third Party Integrations Proof of Concept torn down.");
            base.Dispose(disposing);
        }
    }
}
