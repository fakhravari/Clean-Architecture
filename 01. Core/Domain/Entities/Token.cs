namespace Domain.Entities;

public class Token
{
    public Guid Id { get; set; }

    public string Idconnection { get; set; } = null!;

    public long? IdPersonel { get; set; }

    public string? Token1 { get; set; }

    public DateTime DateTime { get; set; }

    public string Ip { get; set; } = null!;

    public bool IsActive { get; set; }

    public virtual Personel? IdPersonelNavigation { get; set; }
}