namespace Infrastructure.Repository.Product
{
    public class ProductRepository : IProductRepository
    {
        public async Task<int> CreateProduct(string Name, decimal Price)
        {
            return DateTime.Now.Second;
        }
    }
}
