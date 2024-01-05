using Application.Model.Product;
using Domain.Entities;

namespace Application.Contracts.Persistence
{
    public interface IProductRepository : IGenericRepositoryAsync<Product>
    {
        Task<List<GetListProductsDto>> GetListProducts(string IdCategory);
    }
}