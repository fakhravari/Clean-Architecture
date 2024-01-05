using Application.Model.Product;
using MediatR;

namespace Application.Features.Product.Queries.GetListProducts
{
    public sealed class GetListProductsQuerie : IRequest<List<GetListProductsDto>>
    {
        public string IdCategory { get; set; }
    }
}
