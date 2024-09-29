using Application.Contracts.Persistence.IRepository;
using FluentValidation;

namespace Application.Features.Product.Queries.GetListProducts
{
    public sealed class GetListProductsQuerieValidator : AbstractValidator<GetListProductsQuerie>
    {
        private readonly IPersonelRepository _productRepository;

        public GetListProductsQuerieValidator(IPersonelRepository productRepository)
        {
            _productRepository = productRepository;

            RuleFor(p => p.IdCategory).NotNull().NotEmpty().WithMessage("دیفالت مقدار 0 است");
        }
    }
}