using Drey.Nut;

using System;
using System.Linq;
using System.Security.Permissions;

namespace Drey.Configuration.Services
{
    public class ApplicationSettingsService : MarshalByRefObject, IApplicationSettings
    {
        readonly string _packageId;
        readonly Repositories.IPackageSettingRepository _packageSettingsRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="ApplicationSettingsService"/> class.
        /// </summary>
        /// <param name="packageId">The package identifier.</param>
        /// <param name="packageSettingsRepository">The package settings repository.</param>
        public ApplicationSettingsService(string packageId, Repositories.IPackageSettingRepository packageSettingsRepository)
        {
            _packageId = packageId;
            _packageSettingsRepository = packageSettingsRepository;
        }

        /// <summary>
        /// Gets the <see cref="System.String"/> with the specified key.
        /// </summary>
        /// <value>
        /// The <see cref="System.String"/>.
        /// </value>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        public string this[string key]
        {
            get { return _packageSettingsRepository.ByKey(_packageId, key); }
        }

        public bool Exists(string key)
        {
            return _packageSettingsRepository.All().Any(setting => setting.PackageId.ToLower() == _packageId.ToLower() && setting.Key.ToLower() == key.ToLower());
        }

        public void Register(string key, string value = "")
        {
            var dataModel = _packageSettingsRepository.Get(_packageId, key) ?? new DataModel.PackageSetting { PackageId = _packageId, Key = key };
            dataModel.Value = value;
            _packageSettingsRepository.Store(dataModel);
        }

        [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.Infrastructure)]
        public override object InitializeLifetimeService()
        {
            return null;
        }
    }
}