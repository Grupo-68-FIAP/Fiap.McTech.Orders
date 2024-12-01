using System;

namespace Domain.Interfaces.ExternalServices
{
    public interface IPaymentAdapter
    {
        Task<bool> MoveOrderToNextStatus(Guid orderId);

        Task GeneratePayment(Guid orderId, PaymentRequest model);
    }

    public class PaymentRequest
    {
        public Guid OrderId { get; set; }
        public decimal TotalAmount { get; set; }
        public ClientRequest Client { get; set; } = new ClientRequest();

        public class ClientRequest
        {
            public Guid Id { get; set; }
            public string Name { get; set; }
            public string Email { get; set; }
            public string Cpf { get; set; }
        }
    }
}
