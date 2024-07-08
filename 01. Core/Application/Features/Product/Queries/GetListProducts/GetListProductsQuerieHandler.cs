using Application.Contracts.Persistence;
using Application.Model.Product;
using Application.Services.Jwt;
using MediatR;

namespace Application.Features.Product.Queries.GetListProducts
{
    public sealed class GetListProductsQuerieHandler : IRequestHandler<GetListProductsQuerie, List<GetListProductsDto>>
    {
        private readonly IProductRepository productRepository;
        private readonly IJwtService jwt;

        public GetListProductsQuerieHandler(IProductRepository _personelRepository, IJwtService _jwt)
        {
            productRepository = _personelRepository;
            jwt = _jwt;
        }

        public async Task<List<GetListProductsDto>> Handle(GetListProductsQuerie request, CancellationToken cancellationToken)
        {
            var getList = await productRepository.GetListProducts(request.IdCategory);

            return getList;
        }
    }
}