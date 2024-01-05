namespace Application.Model.Product
{
    public class GetListProductsDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public int IdCategory { get; set; }
        public int Price { get; set; }
        public string CategorieName { get; set; }
    }
}
