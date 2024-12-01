using System.Diagnostics.CodeAnalysis;

namespace Domain.Exceptions
{
    [ExcludeFromCodeCoverage]
    public class CartDeletionException : Exception
    {
        public CartDeletionException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
