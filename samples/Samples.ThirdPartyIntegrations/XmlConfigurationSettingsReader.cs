using Autofac.Configuration.Core;

using System.Xml;

namespace Samples.ThirdPartyIntegrations
{
    class XmlConfigurationSettingsReader : ConfigurationModule
    {
        public XmlConfigurationSettingsReader(XmlReader reader)
        {
            this.SectionHandler = Autofac.Configuration.SectionHandler.Deserialize(reader);
        }
    }
}
