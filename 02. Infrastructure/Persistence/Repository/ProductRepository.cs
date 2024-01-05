using Application.Contracts.Persistence;
using Application.Model.Product;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Persistence.Contexts;

namespace Persistence.Repository
{
    public class ProductRepository : GenericRepository<Product>, IProductRepository
    {
        public ProductRepository(FakhravariDbContext dbContext) : base(dbContext)
        {

        }

        public async Task<List<GetListProductsDto>> GetListProducts(string IdCategory)
        {
            string SQL = $"EXEC SpGeneral.GetListProducts @IdCategory={IdCategory}";

            var products = await _dbContext.Database.SqlQueryRaw<GetListProductsDto>(SQL).ToListAsync();

            return products;

        }
    }
}
