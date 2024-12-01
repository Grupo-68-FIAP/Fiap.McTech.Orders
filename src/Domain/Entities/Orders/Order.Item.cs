namespace Domain.Entities.Orders
{
    public partial class Order : EntityBase
    {
        public class Item : EntityBase
        {
            public Item(Guid productId, Guid orderId, string productName, decimal price, int quantity)
            {
                ProductId = productId;
                OrderId = orderId;
                ProductName = productName;
                Price = price;
                Quantity = quantity;
            }

            public Guid ProductId { get; private set; }

            public Guid OrderId { get; set; }

            public string ProductName { get; private set; }

            public decimal Price { get; private set; }

            public int Quantity { get; private set; }

            public override bool IsValid()
            {
                return ProductId != Guid.Empty &&
                       !string.IsNullOrWhiteSpace(ProductName) &&
                       Price > 0 &&
                       Quantity > 0;
            }
        }
    }
}
