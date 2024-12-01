using System.Diagnostics.CodeAnalysis;

namespace Domain.Exceptions
{
    [ExcludeFromCodeCoverage]
    public class PaymentStatusUpdateException : ApplicationException
    {
        public PaymentStatusUpdateException(string message) : base(message) { }
    }
}
