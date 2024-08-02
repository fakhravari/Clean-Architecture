using Application.Contracts.Persistence;
using Application.Model.Product;
using Domain.Entities;
using Mapster;
using Microsoft.EntityFrameworkCore;

namespace Persistence.Repository
{
    public class ProductRepository : IProductRepository
    {
        private readonly IUnitOfWork _unitOfWork;
        public ProductRepository(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<GetListProductsDto>> GetListProducts(string IdCategory)
        {
            _unitOfWork.CreateContext(isReadOnly: true);

            var product = await _unitOfWork.QueryListRawAsync<GetListProductsDto>($"EXEC SpGeneral.GetListProducts @IdCategory={IdCategory}");
            return product;
        }

        public async Task<List<GetListProductsDto>> GetListProducts1(string Title)
        {
            _unitOfWork.CreateContext(isReadOnly: true);

            Func<IQueryable<Product>, IQueryable<Product>> query = q => q.Where(p => EF.Functions.Like(p.Title, $"%{Title}%"));
            var products = await _unitOfWork.QueryListAsync<Product>(query);

            Func<IQueryable<Product>, IQueryable<Product>> query2 = q => q.Where(p => p.Title.Contains(Title));
            var products1 = await _unitOfWork.QueryListAsync<Product>(query2);


            var productsDto = products.Adapt<List<GetListProductsDto>>();
            var productsDto1 = products1.Adapt<List<GetListProductsDto>>();

            return productsDto;
        }
    }
}
