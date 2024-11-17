using Domain.Enums;
using Domain.Utils.Extensions;

namespace Domain.Entities.Orders
{
    public partial class Order : EntityBase
    {
        public Order(Guid? clientId, decimal totalAmount)
        {
            ClientId = clientId;
            TotalAmount = totalAmount;
            Status = OrderStatus.None;
            Items = new List<Item>();
        }

        public Guid? ClientId { get; private set; }
        public decimal TotalAmount { get; private set; } = 0;
        public OrderStatus Status { get; private set; }
        public ICollection<Item> Items { get; private set; }
        public override bool IsValid()
        {
            return TotalAmount > 0;
        }

        public void SendToNextStatus()
        {
            Status = Status.NextStatus();
        }

        public void Cancel()
        {
            Status = OrderStatus.Canceled;
        }
    }
}
