using Newtonsoft.Json;

namespace Application.Model.Product;
public class GetListProductsDto
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public int IdCategory { get; set; }
    public int Price { get; set; }
    public string CategorieName { get; set; }


    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public List<string> Images { get; set; } = new List<string>();
}