using System;

namespace Drey.DomainModel
{
    public class Response
    {
        /// <summary>
        /// Gets or sets the token that was presented by the broker with the request.
        /// </summary>
        /// <value>
        /// The token.
        /// </value>
        public string Token { get; set; }

        /// <summary>
        /// Gets or sets total time the server recorded to complete the request.  Expressed as milliseconds.
        /// </summary>
        public long Duration { get; set; }

        /// <summary>
        /// The total time the process took to complete on the client.  Expressed as milliseconds.
        /// </summary>
        public long ClientDuration { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the processing of the paired <see cref="Request"/> was successful.
        /// </summary>
        public bool Successful { get; set; }

        /// <summary>
        /// When Successful is false, expresses an error code to the consumer.
        /// </summary>
        public decimal ErrorCode { get; set; }

        /// <summary>
        /// When Successful is false, expresses a human readable error message that the consumer can use to display to the user.
        /// </summary>
        /// <value>
        /// The error message.
        /// </value>
        public string ErrorMessage { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether an exception was encountered and subsequently thrown
        /// that needs to be expressed to the client.
        /// <remarks>
        /// The exceptions that will be documented here are ones for not being able to connect to a sql/ldap server, out of memory, etc.  Exceptions thrown for things like
        /// bad password are not exceptions, and are covered by the <see cref="Successful"/> property.
        /// </remarks>
        /// </summary>
        public bool ExceptionsEncountered { get; set; }

        /// <summary>
        /// Gets or sets what the name of the exception is.
        /// </summary>
        public string ExceptionType { get; set; }

        /// <summary>
        /// Gets or sets the exception's message.
        /// </summary>
        public string ExceptionMessage { get; set; }

        /// <summary>
        /// Gets or sets the exception stack trace.
        /// </summary>
        public string ExceptionStackTrace { get; set; }
    }

    public class Response<TMessage> : Response
    {
        /// <summary>
        /// Gets or sets the contents of the response from the institution client.
        /// </summary>
        /// <value>
        /// The message.
        /// </value>
        public TMessage Message { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Response{TMessage}"/> class.
        /// </summary>
        public Response()
        {
            var type = typeof(TMessage);
            if (type.IsClass && !type.IsArray)
            {
                Message = Activator.CreateInstance<TMessage>();
            }
            else
            {
                Message = default(TMessage);
            }
        }

        /// <summary>
        /// Factory method to build a successful response.
        /// </summary>
        /// <param name="token">The token.</param>
        /// <param name="message">The message.</param>
        /// <returns></returns>
        public static Response<TMessage> Success(string token, TMessage message)
        {
            return new Response<TMessage> { Token = token, Successful = true, ErrorCode = 0, ErrorMessage = string.Empty, Message = message };
        }

        /// <summary>
        /// Factory method to build a failure response.
        /// </summary>
        /// <param name="token">The token.</param>
        /// <param name="errorMessage">The error message.</param>
        /// <param name="code">The code.</param>
        /// <param name="message">The message.</param>
        /// <returns></returns>
        public static Response<TMessage> Failure(string token, string errorMessage, decimal code, TMessage message)
        {
            return new Response<TMessage> { Token = token, ErrorCode = code, ErrorMessage = errorMessage, Successful = false, Message = message };
        }

        /// <summary>
        /// Factory method to build a failure response.
        /// </summary>
        /// <param name="token">The token.</param>
        /// <param name="ex">The ex.</param>
        /// <param name="code">The code.</param>
        /// <returns></returns>
        public static Response<TMessage> Failure(string token, Exception ex, decimal code)
        {
            return new Response<TMessage>
            {
                Token = token,
                ErrorCode = code,
                //TODO: Verify text for this.
                ErrorMessage = "An error occurred while processing your request.  Please try again later.  If this problem persists, please contact your institution.",
                Successful = false,

                ExceptionsEncountered = true,
                ExceptionMessage = ex.Message,
                ExceptionStackTrace = ex.StackTrace,
                ExceptionType = ex.GetType().Name
            };
        }
    }
}
