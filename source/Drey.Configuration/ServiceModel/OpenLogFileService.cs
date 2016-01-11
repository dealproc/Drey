using Drey.Nut;
using Drey.Logging;
using Drey.Utilities;

using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;

namespace Drey.Configuration.ServiceModel
{
    /// <summary>
    /// Provides the capability for the server to retrieve a log file for display.
    /// </summary>
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

            Log.InfoFormat("Read Log File Request received for: '{relativePath}'", relativePath);

            while (relativePath.StartsWith("\\") || relativePath.StartsWith("/"))
            {
                relativePath = relativePath.Substring(1);
            }

            Log.DebugFormat("After removing starting slash(es), relative path looks like '{relativePath}'", relativePath);

            var logFile = Path.Combine(Drey.Utilities.PathUtilities.MapPath(_configurationManager.LogsDirectory), relativePath);
            Log.DebugFormat("Absolute path for log file is: {absoluteLogPath}", logFile);

            if (File.Exists(logFile))
            {
                Log.Info("Log file found.  Compressing and returning to consumer.");
                var ms = new MemoryStream();

                using (var zip = new GZipStream(ms, CompressionLevel.Optimal))
                using (var file = File.OpenRead(logFile))
                {
                    await file.CopyToAsync(zip);
                }

                return DomainModel.Response<byte[]>.Success(request.Token, ms.ToArray());
            }

            Log.Warn("Log file does not exist.");
            return DomainModel.Response<byte[]>.Failure(request.Token, "File does not exist.", 1M, default(byte[]));

        }
    }
}