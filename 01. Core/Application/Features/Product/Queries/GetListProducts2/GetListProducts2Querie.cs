using Application.Model.Product;
using MediatR;

namespace Application.Features.Product.Queries.GetListProducts2
{
    public sealed class GetListProducts2Querie : IRequest<List<GetListProductsDto>>
    {
        public string Title { get; set; }
    }
}
