using Domain.Enums;

namespace Domain.Utils.Extensions
{
    public static class OrderStatusExtensions
    {
        public static OrderStatus NextStatus(this OrderStatus currentStatus)
        {
            return currentStatus switch
            {
                OrderStatus.None => OrderStatus.WaitPayment,
                OrderStatus.WaitPayment => OrderStatus.Received,
                OrderStatus.Received => OrderStatus.InPreparation,
                OrderStatus.InPreparation => OrderStatus.Ready,
                OrderStatus.Ready => OrderStatus.Finished,
                OrderStatus.Finished => throw new InvalidOperationException("The order is already finished."),
                OrderStatus.Canceled => throw new InvalidOperationException("The order is already canceled."),
                _ => OrderStatus.None,
            };
        }
    }
}
