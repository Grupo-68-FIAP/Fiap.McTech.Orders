using System;

namespace Domain.Interfaces.ExternalServices
{
    public interface IPaymentAdapter
    {
        Task<bool> MoveOrderToNextStatus(Guid orderId);
    }
}
