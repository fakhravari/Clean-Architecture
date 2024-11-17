using Application.Contracts.Persistence.IRepository;
using Application.Model.Product;
using Application.Services.JWTAuthetication;
using Domain.Common;
using MediatR;

namespace Application.Features.Product.Queries.GetListProducts
{
    public sealed class GetListProductsQuerieHandler : IRequestHandler<GetListProductsQuerie, BaseResponse<List<GetListProductsDto>>>
    {
        private readonly IProductRepository productRepository;
        private readonly IJwtAuthenticatedService _jwtAuthenticated;

        public GetListProductsQuerieHandler(IProductRepository _personelRepository, IJwtAuthenticatedService jwtAuthenticated)
        {
            productRepository = _personelRepository;
            _jwtAuthenticated = jwtAuthenticated;
        }

        public async Task<BaseResponse<List<GetListProductsDto>>> Handle(GetListProductsQuerie request, CancellationToken cancellationToken)
        {
            var getList = await productRepository.GetListProducts(request.IdCategory);

            return new BaseResponse<List<GetListProductsDto>>()
            {
                Data = getList
            };
        }
    }
}