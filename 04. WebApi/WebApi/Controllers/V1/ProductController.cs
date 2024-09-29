using Application.Contracts.Persistence.IRepository;
using Application.Features.Product.Queries.GetListProducts;
using Application.Features.Product.Queries.GetListProducts2;
using Application.Services.JWTAuthetication;
using Application.Services.Serilog;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using WebApi.Controllers.Base;

namespace WebApi.Controllers.V1
{
    [ApiVersion("1")]
    public class ProductController : BaseController
    {
        private readonly IJwtService iwJwtService;
        private readonly ISerilogService _logger;
        private readonly IProductRepository iProductRepository;
        public ProductController(IJwtService _iwJwtService, ISerilogService logger, IProductRepository _iProductRepository)
        {
            this.iwJwtService = _iwJwtService;
            _logger = logger;
            iProductRepository = _iProductRepository;
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> GetListProducts(GetListProductsQuerie command)
        {
            var result = await Mediator.Send(command);
            return Ok(result);
        }


        [HttpPost("[action]")]
        public async Task<IActionResult> GetListProducts2(GetListProducts2Querie command)
        {
            //try
            //{
            //    int.Parse("a");
            //}
            //catch (Exception ex)
            //{
            //    _logger.LogSystem(ex);
            //}

            var result = await Mediator.Send(command);
            return Ok(result);
        }



        [HttpPost("[action]")]
        public async Task<IActionResult> MultipleResults()
        {
            var result = await iProductRepository.MultipleResults();
            return Ok(result);
        }





        [HttpPost("[action]")]
        public async Task<IActionResult> BulkInsert()
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            await iProductRepository.BulkInsertAsync();

            stopwatch.Stop();
            var ts = stopwatch.Elapsed;

            return Ok("Seconds: " + ts.Seconds);
        }
        [HttpPost("[action]")]
        public async Task<IActionResult> Insert()
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            await iProductRepository.InsertAsync();

            stopwatch.Stop();
            var ts = stopwatch.Elapsed;

            return Ok("Seconds: " + ts.Seconds);
        }
    }
}
