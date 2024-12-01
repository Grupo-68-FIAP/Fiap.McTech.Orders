using System.Diagnostics.CodeAnalysis;

namespace Domain.Exceptions
{
    [ExcludeFromCodeCoverage]
    public class InvalidCartIdException : ArgumentException
    {
        public InvalidCartIdException(string message) : base(message) { }
    }
}
