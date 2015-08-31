using Drey.Nut;

using System;
using System.Collections.Generic;

namespace Drey.Configuration.Services
{
    public class ConnectionStringsService : MarshalByRefObject, IConnectionStrings
    {
        readonly string _packageId;
        readonly Repositories.IConnectionStringRepository _connectionStringsRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConnectionStringsService"/> class.
        /// </summary>
        /// <param name="packageId">The package identifier.</param>
        /// <param name="connectionStringsRepository">The connection strings repository.</param>
        public ConnectionStringsService(string packageId, Repositories.IConnectionStringRepository connectionStringsRepository)
        {
            _packageId = packageId;
            _connectionStringsRepository = connectionStringsRepository;
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
            get { return _connectionStringsRepository.ByName(_packageId, key); }
        }

        public void Register(IEnumerable<string> keys)
        {
            throw new NotImplementedException();
        }
    }
}