namespace Application.Dtos
{
    public class CreateOrderRequestDto
    {
        public Guid CartId { get; set; }
        public Guid ClientId { get; set; }
        public decimal AllValue { get; private set; } = 0;
        public List<CartItemRequestDto> Items { get; set; } = new List<CartItemRequestDto>();

        public void CalculateAllValue()
        {
            if (Items == null || !Items.Any())
            {
                AllValue = 0;
                return;
            }

            AllValue = Items.Sum(item => item.Quantity * item.Value);
        }
    }

    public class CartItemRequestDto
    {
        public string? Name { get; set; }
        public int Quantity { get; set; } = 0;
        public decimal Value { get; set; } = 0;
        public Guid ProductId { get; set; }
    }
}
