namespace Application.Model.Personel;

public class LoginDto
{
    public bool IsLogin { get; set; }

    public long Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string NationalCode { get; set; }
    public string Token { get; set; }
    public Guid RefreshToken { get; set; }
}