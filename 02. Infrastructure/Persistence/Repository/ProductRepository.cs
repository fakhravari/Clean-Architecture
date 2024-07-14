using Application.Contracts.Persistence;
using Application.Model.Product;
using Mapster;
using Microsoft.EntityFrameworkCore;
using Persistence.Contexts;

namespace Persistence.Repository
{
    public class ProductRepository : IProductRepository
    {
        private readonly IApplicationDbContextFactory _dbContext;
        public ProductRepository(IApplicationDbContextFactory dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<GetListProductsDto>> GetListProducts(string IdCategory)
        {
            string SQL = $"EXEC SpGeneral.GetListProducts @IdCategory={IdCategory}";

            using (var context = _dbContext.CreateDbContext(false))
            {
                return await context.Database.SqlQueryRaw<GetListProductsDto>(SQL).ToListAsync();
            }
        }

        public async Task<List<GetListProductsDto>> GetListProducts1(string Title)
        {
            using (var context = _dbContext.CreateDbContext(true))
            {
                var producvts = await context.Products.ToListAsync();
                var products = await context.Products
                    .Where(v => EF.Functions.Like(v.Title, $"%{Title}%")).ToListAsync();
                var products1 = await context.Products.Where(v => v.Title.Contains(Title))
                    .ToListAsync();

                var productsDto = products.Adapt<List<GetListProductsDto>>();
                var productsDto1 = products1.Adapt<List<GetListProductsDto>>();

                return productsDto;
            }
        }
    }
}
