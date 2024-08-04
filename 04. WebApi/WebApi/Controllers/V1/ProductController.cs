using Application.Features.Product.Queries.GetListProducts;
using Application.Features.Product.Queries.GetListProducts2;
using Application.Services.Jwt;
using Application.Services.Serilog;
using Microsoft.AspNetCore.Mvc;
using WebApi.Controllers.Base;

namespace WebApi.Controllers.V1
{
    [ApiVersion("1")]
    public class ProductController : BaseController
    {
        private readonly IJwtService iwJwtService;
        private readonly ISerilogService _logger;
        public ProductController(IJwtService _iwJwtService, ISerilogService logger)
        {
            this.iwJwtService = _iwJwtService;
            _logger = logger;
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
            try
            {
                int.Parse("a");
            }
            catch (Exception ex)
            {
                _logger.LogSystem(ex);
            }

            var result = await Mediator.Send(command);
            return Ok(result);
        }
    }
}
