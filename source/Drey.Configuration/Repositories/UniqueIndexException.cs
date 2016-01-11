using System;

namespace Drey.Configuration.Repositories
{
    /// <summary>
    /// Thrown when a unique index has been violated.
    /// </summary>
    [Serializable]
    class UniqueIndexException : Exception
    {
        public UniqueIndexException() : base() { }
        public UniqueIndexException(string message) : base(message) { }
    }
}
