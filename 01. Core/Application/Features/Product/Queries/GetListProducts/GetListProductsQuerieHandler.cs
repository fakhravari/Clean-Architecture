using Application.Contracts.Persistence.IRepository;
using Application.Model.Product;
using Application.Services.JWTAuthetication;
using Domain.Common;
using MediatR;

namespace Application.Features.Product.Queries.GetListProducts;

public sealed class
    GetListProductsQuerieHandler : IRequestHandler<GetListProductsQuerie, BaseResponse<List<GetListProductsDto>>>
{
    private readonly IJwtAuthService _jwtAuth;
    private readonly IProductRepository productRepository;

    public GetListProductsQuerieHandler(IProductRepository _personelRepository,
        IJwtAuthService jwtAuth)
    {
        productRepository = _personelRepository;
        _jwtAuth = jwtAuth;
    }

    public async Task<BaseResponse<List<GetListProductsDto>>> Handle(GetListProductsQuerie request,
        CancellationToken cancellationToken)
    {
        var getList = await productRepository.GetListProducts(request.IdCategory);

        return new BaseResponse<List<GetListProductsDto>>
        {
            Data = getList
        };
    }
}