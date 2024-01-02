namespace PersistenceService.Service.Personel
{
    public interface IPersonelService
    {
        Task<bool> IsValidName(string Name);
        Task<int> Login(string UserName, string Password);
    }
}
