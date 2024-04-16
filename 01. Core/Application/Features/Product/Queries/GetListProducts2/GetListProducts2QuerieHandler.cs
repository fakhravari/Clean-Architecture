using Application.Contracts.Persistence;
using Application.Model.Product;
using Application.Services.Jwt;
using MediatR;

namespace Application.Features.Product.Queries.GetListProducts2
{
    public sealed class GetListProducts2QuerieHandler : IRequestHandler<GetListProducts2Querie, List<GetListProductsDto>>
    {
        private readonly IProductRepository productRepository;
        private readonly IJwtService jwt;

        public GetListProducts2QuerieHandler(IProductRepository _personelRepository, IJwtService _jwt)
        {
            productRepository = _personelRepository;
            jwt = _jwt;
        }

        public async Task<List<GetListProductsDto>> Handle(GetListProducts2Querie request, CancellationToken cancellationToken)
        {
            var getList = await productRepository.GetListProducts1(request.Title);

            return getList;
        }
    }
}