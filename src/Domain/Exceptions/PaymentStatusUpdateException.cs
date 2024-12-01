using System.Diagnostics.CodeAnalysis;

namespace Domain.Exceptions
{
    [ExcludeFromCodeCoverage]
    public class PaymentStatusUpdateException : Exception
    {
        public PaymentStatusUpdateException(string message)
            : base(message)
        {
        }

        public PaymentStatusUpdateException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
