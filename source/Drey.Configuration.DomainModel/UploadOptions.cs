namespace Drey.Configuration.DomainModel
{
    public class UploadOptions
    {
        /// <summary>
        /// The maximum chunk size to be posted to the server.
        /// </summary>
        public long MaxChunkSize { get; set; }

        /// <summary>
        /// Gets or sets the chunk endpoint.
        /// </summary>
        public string ChunkEndpoint { get; set; }

        /// <summary>
        /// Gets or sets the assemble endpoint.
        /// </summary>
        public string AssembleEndpoint { get; set; }
    }
}