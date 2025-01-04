using Application.Contracts.Persistence.IRepository;
using Application.Model.Product;
using Application.Services.JWTAuthetication;
using Domain.Common;
using MediatR;

namespace Application.Features.Product.Queries.GetListProducts2;

public sealed class
    GetListProducts2QuerieHandler : IRequestHandler<GetListProducts2Querie, BaseResponse<List<GetListProductsDto>>>
{
    private readonly IJwtAuthenticatedService _jwtAuthenticated;
    private readonly IProductRepository productRepository;

    public GetListProducts2QuerieHandler(IProductRepository _personelRepository,
        IJwtAuthenticatedService jwtAuthenticated)
    {
        productRepository = _personelRepository;
        _jwtAuthenticated = jwtAuthenticated;
    }

    public async Task<BaseResponse<List<GetListProductsDto>>> Handle(GetListProducts2Querie request,
        CancellationToken cancellationToken)
    {
        var getList = await productRepository.GetListProducts1(request.Title);

        return new BaseResponse<List<GetListProductsDto>>
        {
            Data = getList
        };
    }
}