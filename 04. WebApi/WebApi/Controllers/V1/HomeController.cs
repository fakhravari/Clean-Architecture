using Application.Model;
using FluentValidation;
using Infrastructure.Config.Jwt;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
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
        public async Task<IActionResult> JwtTokenGenerate(AuthorizeDTO model)
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





    #region ایجاد محصول
    public class CreateProductCommand : IRequest<int>
    {
        [Required(ErrorMessage = "نام محصول الزامی است.")]
        public string Name { get; set; }

        [Range(0.01, double.MaxValue, ErrorMessage = "قیمت محصول باید بیشتر از صفر باشد.")]
        public decimal Price { get; set; }
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

    #endregion
    #region ولیدیشن
    public class CreateProductCommandValidator : AbstractValidator<CreateProductCommand>
    {
        public CreateProductCommandValidator()
        {
            RuleFor(p => p.Name)
                .NotNull()
                .NotEmpty()
                .WithMessage("نام محصول الزامی است.");

            RuleFor(command => command.Price)
                .GreaterThan(0)
                .WithMessage("قیمت محصول باید بیشتر از صفر باشد.");
        }
    }



    #endregion
    #region ریپازیتوری
    public interface IProductRepository
    {
        Task<int> CreateProduct(string Name, decimal Price);
    }
    public class ProductRepository : IProductRepository
    {
        public async Task<int> CreateProduct(string Name, decimal Price)
        {
            return DateTime.Now.Second;
        }
    }
    #endregion
}
