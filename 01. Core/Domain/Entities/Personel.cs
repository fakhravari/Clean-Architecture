namespace Domain.Entities;

public class Personel
{
    public long Id { get; set; }

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string? NationalCode { get; set; }

    public string UserName { get; set; } = null!;

    public string Password { get; set; } = null!;

    public virtual ICollection<PersonnelToChangeRolesActivity> PersonnelToChangeRolesActivities { get; set; } =
        new List<PersonnelToChangeRolesActivity>();

    public virtual ICollection<Token> Tokens { get; set; } = new List<Token>();
}