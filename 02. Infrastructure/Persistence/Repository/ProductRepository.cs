using Application.Contracts.Persistence;
using Application.Contracts.Persistence.Contexts;
using Application.Model.Product;
using Application.Services.Serilog;
using Domain.Entities;
using Mapster;
using Microsoft.EntityFrameworkCore;

namespace Persistence.Repository
{
    public class ProductRepository : IProductRepository
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ISerilogService _logger;
        public ProductRepository(IUnitOfWork unitOfWork, ISerilogService logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<List<GetListProductsDto>> GetListProducts(string IdCategory)
        {
            _unitOfWork.CreateContext(isReadOnly: true);

            var tttt = _unitOfWork.Context.Database.GetConnectionString();

            var product = await _unitOfWork.QueryListRawAsync<GetListProductsDto>($"EXEC SpGeneral.GetListProducts @IdCategory={IdCategory}");
            return product;
        }

        public async Task<List<GetListProductsDto>> GetListProducts1(string Title)
        {
            _unitOfWork.CreateContext(isReadOnly: false);

            Func<IQueryable<Product>, IQueryable<Product>> query = q => q.Where(p => EF.Functions.Like(p.Title, $"%{Title}%"));
            var products = await _unitOfWork.QueryListAsync<Product>(query);

            Func<IQueryable<Product>, IQueryable<Product>> query2 = q => q.Where(p => p.Title.Contains(Title));
            var products1 = await _unitOfWork.QueryListAsync<Product>(query2);


            var productsDto = products.Adapt<List<GetListProductsDto>>();
            var productsDto1 = products1.Adapt<List<GetListProductsDto>>();


            await Trans();


            return productsDto;
        }


        public async Task<int> Trans()
        {
            _unitOfWork.CreateContext(isReadOnly: true);

            try
            {
                int tt = int.Parse("4445kh");
            }
            catch (Exception ex)
            {
                _logger.LogSystem(ex);
            }



            //try
            //{
            //    await _unitOfWork.BeginTransactionAsync();

            //    var addenty = new Product() { IdCategory = 1, Price = 100, Title = "test" };

            //    await _unitOfWork.AddAsync(addenty);
            //    var res1 = await _unitOfWork.SaveChangesAsync();
            //    var newId = addenty.Id;

            //    await _unitOfWork.CommitTransactionAsync();
            //}
            //catch (Exception ex)
            //{
            //    await _unitOfWork.RollbackTransactionAsync(ex);
            //}

            return 1;
        }
    }
}
