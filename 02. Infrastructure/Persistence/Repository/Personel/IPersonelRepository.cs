namespace Persistence.Repository.Personel
{
    public interface IPersonelRepository
    {
        Task<bool> IsValidName(string Name);
        Task<int> Login(string UserName, string Password);
    }
}
