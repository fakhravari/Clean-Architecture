namespace Domain.Entities;

public class PersonnelToChangeRolesActivity
{
    public Guid Id { get; set; }

    public long IdPersonel { get; set; }

    public bool IsActive { get; set; }

    public virtual Personel IdPersonelNavigation { get; set; } = null!;
}