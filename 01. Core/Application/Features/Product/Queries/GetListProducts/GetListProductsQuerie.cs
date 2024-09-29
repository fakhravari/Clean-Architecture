using Application.Model.Product;
using Domain.Common;
using MediatR;

namespace Application.Features.Product.Queries.GetListProducts
{
    public sealed class GetListProductsQuerie : IRequest<BaseResponse<List<GetListProductsDto>>>
    {
        public string IdCategory { get; set; }
    }
}

