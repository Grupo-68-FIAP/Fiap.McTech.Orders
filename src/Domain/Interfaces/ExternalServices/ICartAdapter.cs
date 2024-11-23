namespace Domain.Interfaces.ExternalServices
{
    public interface ICartAdapter
    {
        Task<bool> DeleteCartByIdAsync(Guid cartId);
    }
}
