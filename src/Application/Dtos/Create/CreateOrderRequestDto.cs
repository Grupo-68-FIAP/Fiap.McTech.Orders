namespace Application.Dtos.Create
{
    public class CreateOrderRequestDto
    {
        public Guid CartId { get; set; }
        public List<CartItemRequestDto> Items { get; set; }
    }

    public class CartItemRequestDto
    {
        public string Name { get; set; }
        public int Quantity { get; set; }
        public decimal Value { get; set; }
        public Guid ProductId { get; set; }
    }
}
