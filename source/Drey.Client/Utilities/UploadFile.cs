using Drey.Client.Logging;
using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;

namespace Drey.Client.Utilities
{
    public class UploadFile
    {
        static readonly ILog _log = LogProvider.For<UploadFile>();

        readonly HttpClient _webClient;
        readonly string _chunkEndpoint;
        readonly string _assembleEndpoint;
        readonly string _fileName;
        readonly Options _options;

        byte[] _chunk;
        int _chunkSize = 0;
        int _chunkIndex = 0;
        int _startIndex = 0;
        int _retryCount = 0;
        int _fileSize = 0;

        /// <summary>
        /// Initializes a new instance of the <see cref="UploadFile"/> class.
        /// </summary>
        /// <param name="fileName">The full path and name of the file to be uploaded.</param>
        /// <param name="chunkEndpoint">The endpoint that will process each chunk as it is prepared and sent.</param>
        /// <param name="assembleEndpoint">
        /// The assemble endpoint.
        /// <remarks>This url is utilized with a string.format, and you'll place a {0} wherever you want the filename to be.</remarks>
        /// </param>
        /// <param name="options">The options.</param>
        /// <exception cref="System.ArgumentNullException">
        /// fileName
        /// or
        /// chunkEndpoint
        /// or
        /// assembleEndpoint
        /// </exception>
        /// <exception cref="System.IO.FileNotFoundException">The file does not appear to exist.</exception>
        public UploadFile(string fileName, HttpClient webClient, string chunkEndpoint, string assembleEndpoint, Options options = null)
        {
            _options = options ?? new Options();

            if (string.IsNullOrWhiteSpace(fileName)) { throw new ArgumentNullException("fileName"); }
            if (!File.Exists(fileName)) { throw new FileNotFoundException("The file does not appear to exist."); }

            if (webClient == null) { throw new ArgumentNullException("webClient"); }
            if (string.IsNullOrWhiteSpace(chunkEndpoint)) { throw new ArgumentNullException("chunkEndpoint"); }
            if (string.IsNullOrWhiteSpace(assembleEndpoint)) { throw new ArgumentNullException("assembleEndpoint"); }

            _fileName = fileName;
            _webClient = webClient;
            _chunkEndpoint = chunkEndpoint;
            _assembleEndpoint = assembleEndpoint;
        }

        /// <summary>
        /// Performs the upload process.
        /// </summary>
        public Drey.Configuration.DomainModel.UploadReferenceInfo Execute()
        {
            _chunkSize = 1024 * _options.KBPerChunk;
            _chunk = new byte[_chunkSize];

            var file = new FileInfo(_fileName);
            _fileSize = Convert.ToInt32(file.Length);

            if (PushContent(file))
            {
                return Assemble(file);
            }

            return null;
        }

        private bool PushContent(FileInfo file)
        {
            using (var stream = file.OpenRead())
            {
                int bytesRead = ReadChunk(stream);
                do
                {
                    var content = PrepareBody(bytesRead, file.Name);
                    var response = _webClient.PostAsync(_chunkEndpoint, content).Result;

                    try
                    {
                        response.EnsureSuccessStatusCode();

                        _startIndex += bytesRead;
                        _chunkIndex++;
                        _retryCount = 0;

                        ReportProgress("Uploading file...");

                        bytesRead = ReadChunk(stream);
                    }
                    catch (Exception ex)
                    {
                        _retryCount++;
                        _log.DebugException("Attempt {0} to upload chunk {1} failed.", ex, _retryCount, _chunkIndex);

                        if (_retryCount > _options.MaxRetryCountBeforeFailure)
                        {
                            _log.Error("Upload failed.");
                            ReportProgress("Upload could not complete.");
                            return false;
                        }
                        else
                        {
                            System.Threading.Thread.Sleep(TimeSpan.FromSeconds(_options.SecondsToDelayBetweenRetries));
                            continue;
                        }
                    }
                } while (bytesRead != 0 && _startIndex < _fileSize);
            }
            return true;
        }
        private Drey.Configuration.DomainModel.UploadReferenceInfo Assemble(FileInfo file)
        {
            _retryCount = 0;
            bool successful = false;
            do
            {
                var response = _webClient.PostAsync(string.Format(_assembleEndpoint, file.Name), new StringContent("{}")).Result;
                try
                {
                    response.EnsureSuccessStatusCode();
                    successful = true;
                    return response.Content.ReadAsAsync<Drey.Configuration.DomainModel.UploadReferenceInfo>().Result;
                }
                catch (Exception ex)
                {
                    _retryCount++;
                    _log.DebugException("Attempt {0} to assemble file failed.", ex, _retryCount);
                    if (_retryCount > _options.MaxRetryCountBeforeFailure)
                    {
                        System.Threading.Thread.Sleep(TimeSpan.FromSeconds(_options.SecondsToDelayBetweenRetries));
                    }
                    else
                    {
                        _log.Error("Assembling file failed.");
                        ReportProgress("File assembly did not occur.");
                        return null;
                    }
                }
            } while (!successful);

            return null;
        }
        private void ReportProgress(string message)
        {
            if (_options.ReportsProgress && _options.ProgressReporter != null)
            {
                var pct = Math.Floor((Convert.ToDecimal(_startIndex) / Convert.ToDecimal(_fileSize)) * 100);
                _options.ProgressReporter.Report(new Progress(Convert.ToInt32(pct), message));
            }
        }
        private int ReadChunk(Stream stream)
        {
            var maxBytesToRead = Convert.ToInt32(Math.Min(_chunkSize, (_fileSize - _startIndex)));
            return stream.Read(_chunk, 0, maxBytesToRead);
        }
        private MultipartFormDataContent PrepareBody(int bytesRead, string fileName)
        {
            var result = new MultipartFormDataContent();

            result.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment") { FileName = string.Format("\"{0}\"", fileName) };
            result.Headers.Add("Content-Index", _chunkIndex.ToString());
            result.Headers.Add("Content-Range", string.Format("bytes {0}-{1}/{2}", _startIndex, (_startIndex + bytesRead) - 1, _fileSize));


            var content = new ByteArrayContent(_chunk, 0, bytesRead);
            content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment") { FileName = string.Format("\"{0}\"", fileName) };
            content.Headers.Add("Content-Index", _chunkIndex.ToString());
            content.Headers.Add("Content-Range", string.Format("bytes {0}-{1}/{2}", _startIndex, (_startIndex + bytesRead) - 1, _fileSize));

            result.Add(content);

            return result;
        }

        public class Progress
        {
            /// <summary>
            /// Gets what the percentage of the file has been uploaded to the server.
            /// </summary>
            public int PercentUploaded { get; private set; }

            /// <summary>
            /// Gets the status text.
            /// </summary>
            public string StatusText { get; private set; }

            /// <summary>
            /// Initializes a new instance of the <see cref="Progress"/> class.
            /// </summary>
            /// <param name="percentUploaded">The percent uploaded.</param>
            /// <param name="statusText">The status text.</param>
            public Progress(int percentUploaded, string statusText)
            {
                PercentUploaded = percentUploaded;
                StatusText = statusText;
            }
        }

        public class Options
        {
            /// <summary>
            /// Gets or sets the kb per chunk.
            /// </summary>
            public int KBPerChunk { get; set; }

            /// <summary>
            /// Gets or sets a value indicating whether [reports progress].
            /// </summary>
            public bool ReportsProgress { get; set; }

            /// <summary>
            /// Gets or sets the progress reporter.
            /// </summary>
            public IProgress<Progress> ProgressReporter { get; set; }

            /// <summary>
            /// Gets or sets the maximum retry count before failure.
            /// </summary>
            public int MaxRetryCountBeforeFailure { get; set; }

            /// <summary>
            /// Gets or sets the seconds to delay between retries.
            /// </summary>
            public int SecondsToDelayBetweenRetries { get; set; }

            /// <summary>
            /// Initializes a new instance of the <see cref="UploadOptions"/> class.
            /// </summary>
            public Options()
            {
                KBPerChunk = 800;
                ReportsProgress = true;
                MaxRetryCountBeforeFailure = 3;
                SecondsToDelayBetweenRetries = 3;
            }
        }
    }
}