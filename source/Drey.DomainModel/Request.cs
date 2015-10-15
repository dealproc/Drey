namespace Drey.DomainModel
{
    /// <summary>
    /// A request that contains specific data related to its concern and its
    /// consuming service
    /// </summary>
    public class Request
    {
        /// <summary>
        /// Gets or sets the token which is then returned on the response object.
        /// </summary>
        /// <value>
        /// The token.
        /// </value>
        public string Token { get; set; }
    }
    /// <summary>
    /// A request that contains specific data related to its concern and its
    /// consuming service
    /// </summary>
    /// <typeparam name="TMessage">The type of the message.</typeparam>
    public class Request<TMessage> : Request
    {
        /// <summary>
        /// Gets or sets the payload of information for this request.
        /// </summary>
        /// <value>
        /// The message.
        /// </value>
        public TMessage Message { get; set; }
    }
}
