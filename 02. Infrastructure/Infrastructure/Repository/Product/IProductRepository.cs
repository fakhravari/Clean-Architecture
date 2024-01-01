namespace Infrastructure.Repository.Product
{
    public interface IProductRepository
    {
        Task<int> CreateProduct(string Name, decimal Price);
    }
}
