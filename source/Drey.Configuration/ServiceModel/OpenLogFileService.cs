using Drey.Nut;
using Drey.Utilities;

using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;

namespace Drey.Configuration.ServiceModel
{
    class OpenLogFileService : RemoteInvocationService<DomainModel.Request<DomainModel.FileDownloadOptions>, DomainModel.FileDownloadOptions, DomainModel.Response<byte[]>, byte[]>
    {
        readonly INutConfiguration _configurationManager;

        public OpenLogFileService(INutConfiguration configurationManager) : base("BeginOpenLogFile", "EndOpenLogFile")
        {
            _configurationManager = configurationManager;
        }
        protected override async Task<DomainModel.Response<byte[]>> ProcessAsync(DomainModel.Request<DomainModel.FileDownloadOptions> request)
        {
            var relativePath = request.Message.RelativeOrAbsolutePath;

            while (relativePath.StartsWith("\\"))
            {
                relativePath = relativePath.Substring(1);
            }

            var logFile = Path.Combine(_configurationManager.LogsDirectory, relativePath).NormalizePathSeparator();

            if (File.Exists(logFile))
            {
                var ms = new MemoryStream();

                using (var zip = new GZipStream(ms, CompressionLevel.Optimal))
                using (var file = File.OpenRead(logFile))
                {
                    await file.CopyToAsync(zip);
                }

                return DomainModel.Response<byte[]>.Success(request.Token, ms.ToArray());
            }
            return DomainModel.Response<byte[]>.Failure(request.Token, "File does not exist.", 1M, default(byte[]));

        }
    }
}