using Drey.Nut;
using System;
using System.Collections.Generic;

namespace Drey.Configuration.Services
{
    public class ConnectionStringsService : MarshalByRefObject, IConnectionStrings
    {
        readonly string _packageId;
        readonly Repositories.IConnectionStringRepository _connectionStringsRepository;

        public ConnectionStringsService(string packageId, Repositories.IConnectionStringRepository connectionStringsRepository)
        {
            _packageId = packageId;
            _connectionStringsRepository = connectionStringsRepository;
        }

        public string this[string key]
        {
            get { return _connectionStringsRepository.ByKey(_packageId, key); }
        }

        public void Register(IEnumerable<string> keys)
        {
            throw new NotImplementedException();
        }
    }
}