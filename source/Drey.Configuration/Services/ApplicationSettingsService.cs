using Drey.Nut;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Drey.Configuration.Services
{
    public class ApplicationSettingsService : MarshalByRefObject, IApplicationSettings
    {
        readonly string _packageId;
        readonly Repositories.IPackageSettingRepository _packageSettingsRepository;

        public ApplicationSettingsService(string packageId, Repositories.IPackageSettingRepository packageSettingsRepository)
        {
            _packageId = packageId;
            _packageSettingsRepository = packageSettingsRepository;
        }

        public string this[string key]
        {
            get { return _packageSettingsRepository.ByKey(_packageId, key); }
        }

        public void Register(IEnumerable<string> keys)
        {
            throw new NotImplementedException();
        }
    }
}
