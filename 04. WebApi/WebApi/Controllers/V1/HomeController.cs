using Application.Features.Account.Model;
using Application.Model;
using FluentValidation;
using Infrastructure.Config.Jwt;
using Infrastructure.Repository.Product;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApi.Controllers.Base;

namespace WebApi.Controllers.V1
{
    [ApiVersion("1")]
    public class HomeController : BaseController
    {
        private readonly IJwtService iwJwtService;
        public HomeController(IJwtService _iwJwtService)
        {
            this.iwJwtService = _iwJwtService;
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> PublicAction(PublicActionDTO model)
        {
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(model);
            return Ok(json);
        }

        [Authorize]
        [HttpPost("[action]")]
        public async Task<IActionResult> PrivateAction(PrivateActionDTO model)
        {
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(model);
            return Ok(json);
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> Login(AccountLoginDto model)
        {
            var jwt = iwJwtService.JwtTokenGenerate(model.UserName, model.Password);

            return Ok(jwt);
        }



        /// <summary>
        /// ورژن موبایل
        /// </summary>
        /// <returns></returns>
        [HttpPost("[action]")]
        public async Task<IActionResult> GetVersion()
        {
            return Ok("GetVersion = " + DateTime.Now.ToString());
        }








        [HttpPost("[action]")]
        public async Task<IActionResult> Create(CreateProductCommand command)
        {
            var result = await Mediator.Send(command);
            return Ok(result);
        }
    }



    public class CreateProductCommand : IRequest<int>
    {
        public string Name { get; set; }
        public decimal Price { get; set; }

        public CreateProductTags Tags { get; set; }
    }

    public class CreateProductTags
    {
        public List<string> Name { get; set; }
        public int Id { get; set; }
    }






    public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, int>
    {
        private readonly IProductRepository _productRepository;

        public CreateProductCommandHandler(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public async Task<int> Handle(CreateProductCommand request, CancellationToken cancellationToken)
        {
            var productId = await _productRepository.CreateProduct(request.Name, request.Price);
            return productId;
        }
    }

    public class CreateProductCommandValidator : AbstractValidator<CreateProductCommand>
    {
        public CreateProductCommandValidator()
        {
            RuleFor(p => p.Name).NotNull().NotEmpty().WithMessage("نام محصول الزامی است.");
            RuleFor(command => command.Price).GreaterThan(0).WithMessage("قیمت محصول باید بیشتر از صفر باشد.");
        }
    }
}
