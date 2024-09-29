using Application.Contracts.Persistence.IRepository;
using FluentValidation;

namespace Application.Features.Product.Queries.GetListProducts2
{
    public sealed class GetListProducts2QuerieValidator : AbstractValidator<GetListProducts2Querie>
    {
        private readonly IPersonelRepository _productRepository;

        public GetListProducts2QuerieValidator(IPersonelRepository productRepository)
        {
            _productRepository = productRepository;

            RuleFor(p => p.Title).NotNull().NotEmpty().WithMessage("مقدار جستجو را وارد کنید");
        }
    }
}