using Application.Contracts.Persistence;
using Application.Model.Product;
using Domain.Entities;
using Mapster;
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

        public async Task<List<GetListProductsDto>> GetListProducts1(string Title)
        {
            var producvts = await _dbContext.Products.ToListAsync();
            var products = await _dbContext.Products.Where(v => EF.Functions.Like(v.Title, $"%{Title}%")).ToListAsync();
            var products1 = await _dbContext.Products.Where(v => v.Title.Contains(Title)).ToListAsync();

            var productsDto = products.Adapt<List<GetListProductsDto>>();
            var productsDto1 = products1.Adapt<List<GetListProductsDto>>();

            return productsDto;
        }
    }
}
