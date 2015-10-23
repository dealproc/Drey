using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;

namespace Drey.Server
{
    public class FileActionResult : IHttpActionResult
    {
        readonly Stream _fileStream;
        readonly string _fileName;
        readonly string _mimeType;

        public FileActionResult(Stream fileStream, string fileName, string mimeType)
        {
            _fileStream = fileStream;
            _fileName = fileName;
            _mimeType = mimeType;
        }

        public Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
        {
            var result = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StreamContent(_fileStream),
            };
            result.Headers.Add("Content-Disposition", "attachment; filename=\"" + _fileName + "\"");
            result.Headers.Add("Content-Type", _mimeType);

            return Task.FromResult(result);
        }
    }
}
