using Application.Model.Product;

namespace Application.Contracts.Persistence.IRepository
{
    public interface IProductRepository
    {
        Task<List<GetListProductsDto>> GetListProducts(string IdCategory);
        Task<List<GetListProductsDto>> GetListProducts1(string Title);


        Task BulkInsertAsync();
        Task InsertAsync();


        Task<object> MultipleResults();


        Task<Guid> WorkManager(string data);
    }
}