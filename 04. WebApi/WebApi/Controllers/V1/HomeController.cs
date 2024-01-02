using Application.Services.Jwt;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Persistence.Repository.Personel;
using Shared.Extentions;
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
        public async Task<IActionResult> PublicAction(string model)
        {
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(model);
            return Ok(json);
        }

        [Authorize]
        [HttpPost("[action]")]
        public async Task<IActionResult> PrivateAction(string model)
        {
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(model);
            return Ok(json);
        }

        //[HttpPost("[action]")]
        //public async Task<IActionResult> Login(LoginCommand model)
        //{
        //    var jwt = iwJwtService.JwtTokenGenerate(model);

        //    return Ok(jwt);
        //}



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

    public record CreateProductCommand : IRequest<object>
    {
        public string Name { get; set; }
        public decimal Price { get; set; }

        public CreateProductTags Tags { get; set; }
    }

    public record CreateProductTags
    {
        public List<string> Name { get; set; }
        public int Id { get; set; }
    }


    public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, object>
    {
        private readonly IPersonelRepository _productRepository;

        public CreateProductCommandHandler(IPersonelRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public async Task<object> Handle(CreateProductCommand request, CancellationToken cancellationToken)
        {
            var productId = await _productRepository.CreateProduct(request.Name, request.Price);
            return productId;
        }
    }

    public class CreateProductCommandValidator : AbstractValidator<CreateProductCommand>
    {
        private readonly IPersonelRepository _productRepository;

        public CreateProductCommandValidator(IPersonelRepository productRepository)
        {
            _productRepository = productRepository;

            When(x => x.Name.Length == 2, () =>
            {
                RuleFor(p => p.Name)
                    .Must(b => false)
                    .WithMessage("دو کاراکتر وارد نکنید");
            });

            RuleFor(p => p.Name)
                .NotNull().NotEmpty().WithMessage("نام محصول الزامی است.")
                .Must(p => p.IsValidName()).WithMessage("مقدار تست را وارد نکنید")
                .MustAsync(IsValidName2).WithMessage("مقدار تست2 را وارد نکنید");

            RuleFor(command => command.Price).GreaterThan(0).WithMessage("قیمت محصول باید بیشتر از صفر باشد.");
        }



        private async Task<bool> IsValidName2(string Name, CancellationToken cancellationToken)
        {
            return await _productRepository.IsValidName(Name);
        }
    }

}
