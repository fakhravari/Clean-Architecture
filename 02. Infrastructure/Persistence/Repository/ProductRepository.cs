using Application.Contracts.Persistence.Contexts;
using Application.Contracts.Persistence.IRepository;
using Application.Model.Product;
using Application.Services.Serilog;
using Domain.Entities;
using Domain.Enum;
using EFCore.BulkExtensions;
using Mapster;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Persistence.Contexts;
using Persistence.Repository.Generic;
using System.Data;

namespace Persistence.Repository
{
    public class ProductRepository : GenericRepository<Product>, IProductRepository
    {
        private readonly IUnitOfWork<FakhravariDbContext> _unitOfWork;

        public ProductRepository(IUnitOfWork<FakhravariDbContext> iUnitOfWork, ISerilogService logger) : base(iUnitOfWork, logger)
        {
            _unitOfWork = iUnitOfWork;
        }

        public async Task<List<GetListProductsDto>> GetListProducts(string IdCategory)
        {
            _unitOfWork.SetDatabaseMode(DatabaseMode.Read);
            var product = await QueryListRawAsync<GetListProductsDto>($"EXEC SpGeneral.GetListProducts @IdCategory={IdCategory}");
            return product;
        }

        public async Task<List<GetListProductsDto>> GetListProducts1(string Title)
        {
            _unitOfWork.SetDatabaseMode(DatabaseMode.Read);

            Func<IQueryable<Product>, IQueryable<Product>>? query = q => q.Where(p => EF.Functions.Like(p.Title, $"%{Title}%"));
            var products = await QueryListAsync<Product>(query);

            Func<IQueryable<Product>, IQueryable<Product>>? query2 = q => q.Where(p => p.Title.Contains(Title));
            var products1 = await QueryListAsync<Product>(query2);


            var productsDto = products.Adapt<List<GetListProductsDto>>();
            var productsDto1 = products1.Adapt<List<GetListProductsDto>>();


            // await Trans();


            var p2 = await QueryListAsync<Product>(
                query => query
                    .Include(e => e.Images)
                    .Include(e => e.IdCategoryNavigation)
                    .Where(e => e.Title.Contains(""))
                );

            var data = p2.Select(v => new GetListProductsDto()
            {
                CategorieName = "",
                Id = v.Id,
                IdCategory = v.IdCategory,
                Images = v.Images.Any() ? v.Images.Select(v => v.ImageName).ToList() : null,
                Price = v.Price,
                Title = v.Title
            }).ToList();

            return data;
        }


        public async Task BulkInsertAsync()
        {
            _unitOfWork.SetDatabaseMode(DatabaseMode.Write);

            var lst = new List<Product>();
            for (int i = 0; i < 200; i++)
            {
                lst.Add(new Product()
                {
                    Id = Guid.NewGuid(),
                    IdCategory = 1,
                    Price = 100,
                    Title = "test",
                    Images = new List<Image>()
                    {
                        new Image() { Id = Guid.NewGuid(),ImageName = "https://fakhravari.ir/CDN/Site/Image/Mohammad_Hussein_Fakhravari_1.jpg" },
                        new Image() { Id = Guid.NewGuid(),ImageName = "https://fakhravari.ir/CDN/Site/Image/Mohammad_Hussein_Fakhravari_1.jpg" }
                    }
                });
            }

            var bulkConfig = new BulkConfig
            {
                IncludeGraph = true,
                SetOutputIdentity = true
            };

            await BulkInsertAsync(lst, bulkConfig);
        }

        public async Task InsertAsync()
        {
            _unitOfWork.SetDatabaseMode(DatabaseMode.Write);

            var lst = new List<Product>();
            for (int i = 0; i < 200; i++)
            {
                lst.Add(new Product()
                {
                    Id = Guid.NewGuid(),
                    IdCategory = 1,
                    Price = 100,
                    Title = "test",
                    Images = new List<Image>()
                    {
                        new Image() { Id = Guid.NewGuid(),ImageName = "https://fakhravari.ir/CDN/Site/Image/Mohammad_Hussein_Fakhravari_1.jpg" },
                        new Image() { Id = Guid.NewGuid(),ImageName = "https://fakhravari.ir/CDN/Site/Image/Mohammad_Hussein_Fakhravari_1.jpg" }
                    }
                });
            }
            await AddRangeAsync(lst);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<int> Trans()
        {
            try
            {
                int tt = int.Parse("4445kh");
            }
            catch (Exception ex)
            {
                // _logger.LogSystem(ex);
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



        public async Task<object> MultipleResults()
        {
            _unitOfWork.SetDatabaseMode(DatabaseMode.Read);

            var results = await MultipleResultsAsync(
                "exec [SpGeneral].[GetListMultiple] @IdCategory",
                new List<Type> { typeof(MyClass1), typeof(MyClass2), typeof(MyClass3) },
                new List<SqlParameter>
                {
                    new SqlParameter("@IdCategory", SqlDbType.NVarChar) { Value = "0" }
                });

            var categories = results[0].Cast<MyClass1>().ToList();
            var images = results[1].Cast<MyClass2>().ToList();
            var products = results[2].Cast<MyClass3>().ToList();

            // کنترل بسته نشدن کانکشن
            Func<IQueryable<Product>, IQueryable<Product>>? query = q => q.Where(p => EF.Functions.Like(p.Title, $"%{""}%"));
            var products33 = await QueryListAsync<Product>(query);

            return new { categories, images, products };
        }









        public async Task<Guid> WorkManager(string data)
        {
            _unitOfWork.SetDatabaseMode(DatabaseMode.Write);

            var Id = Guid.NewGuid();

            var columnValue = new SqlParameter("columnValue", data);

            string sql = $" INSERT Security.Tokens(Id, Idconnection, IdPersonel, Token, DateTime, Ip, IsActive) SELECT '{Id}',CAST(GETDATE() AS DATE),1,@columnValue,GETDATE(),'--',1; SELECT 'ok'";
            int res = _unitOfWork.Context.Database.ExecuteSqlRaw(sql, columnValue);

            return Id;
        }
    }
}


class MyClass1
{
    public int Id { get; set; }
    public string Title { get; set; }
}
class MyClass2
{
    public Guid Id { get; set; }
    public string ImageName { get; set; }
    public Guid IdProduct { get; set; }
}
class MyClass3
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public int IdCategory { get; set; }
    public int Price { get; set; }
}