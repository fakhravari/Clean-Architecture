using Application.Model.Product;
using Domain.Common;
using MediatR;

namespace Application.Features.Product.Queries.GetListProducts2
{
    public sealed class GetListProducts2Querie : IRequest<BaseResponse<List<GetListProductsDto>>>
    {
        public string Title { get; set; }
    }
}
