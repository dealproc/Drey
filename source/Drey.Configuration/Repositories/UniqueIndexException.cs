using System;

namespace Drey.Configuration.Repositories
{
    [Serializable]
    class UniqueIndexException : Exception
    {
        public UniqueIndexException() : base() { }
        public UniqueIndexException(string message) : base(message) { }
    }
}
