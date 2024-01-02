namespace Domain.Entities;

public partial class Image
{
    public int Id { get; set; }

    public string ImageName { get; set; } = null!;

    public int? IdProduct { get; set; }

    public virtual Product? IdProductNavigation { get; set; }
}
