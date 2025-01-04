namespace Domain.Entities;

public class Image
{
    public Guid Id { get; set; }

    public string ImageName { get; set; } = null!;

    public Guid? IdProduct { get; set; }

    public virtual Product? IdProductNavigation { get; set; }
}