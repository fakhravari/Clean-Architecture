using Application.Features.Product.Queries.GetListProducts;
using Application.Services.Jwt;
using Microsoft.AspNetCore.Mvc;
using WebApi.Controllers.Base;

namespace WebApi.Controllers.V1
{
    [ApiVersion("1")]
    public class ProductController : BaseController
    {
        private readonly IJwtService iwJwtService;
        public ProductController(IJwtService _iwJwtService)
        {
            this.iwJwtService = _iwJwtService;
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> GetListProducts(GetListProductsQuerie command)
        {
            var result = await Mediator.Send(command);
            return Ok(result);
        }
    }
}
