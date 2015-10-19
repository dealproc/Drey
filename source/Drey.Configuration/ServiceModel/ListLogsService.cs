using Drey.Nut;

using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Drey.Configuration.ServiceModel
{
    class ListLogsService : RemoteInvocationService<DomainModel.Request<DomainModel.Empty>, DomainModel.Empty, DomainModel.Response<IEnumerable<string>>, IEnumerable<string>>
    {
        readonly INutConfiguration _configurationManager;

        public ListLogsService(INutConfiguration configurationManager) : base("BeginListLogFiles", "EndListLogFiles")
        {
            _configurationManager = configurationManager;
        }
        protected override Task<DomainModel.Response<IEnumerable<string>>> ProcessAsync(DomainModel.Request<DomainModel.Empty> request)
        {
            if (!Directory.Exists(_configurationManager.LogsDirectory)) { Directory.CreateDirectory(_configurationManager.LogsDirectory); }

            var files = Directory.EnumerateFiles(_configurationManager.LogsDirectory, "*.*", SearchOption.AllDirectories);

            return Task.FromResult(DomainModel.Response<IEnumerable<string>>.Success(request.Token, files));
        }
    }
}
