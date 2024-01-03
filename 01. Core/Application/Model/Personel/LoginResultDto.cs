namespace Application.Model.Personel
{
    public class LoginResultDto
    {
        public bool IsLogin { get; set; }


        public long Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}