using Domain.Enums;

namespace Application.Dtos.Orders
{
	public class OrderOutputDto
    {
		public Guid Id { get; set; }
		public Guid? ClientId { get; set; }
		public string? ClientName { get; set; }
		public decimal TotalAmount { get; set; }
		public OrderStatus Status { get; set; }
		public List<Item> Items { get; set; } = new();
        public class Item
        {
            public Guid ProductId { get; set; }
            public Guid OrderId { get; set; }
            public string ProductName { get; set; } = string.Empty;
            public decimal Price { get; set; }
            public int Quantity { get; set; }
            public decimal TotalPrice { get; set; }
        }
    }
}
