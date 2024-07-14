using Application.Model.Product;
using Domain.Entities;

namespace Application.Contracts.Persistence
{
    public interface IProductRepository
    {
        Task<List<GetListProductsDto>> GetListProducts(string IdCategory);
        Task<List<GetListProductsDto>> GetListProducts1(string Title);
    }
}