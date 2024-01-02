using Persistence.Contexts;

namespace PersistenceService.Service.Personel
{
    public class PersonelService : IPersonelService
    {
        private readonly FakhravariDbContext db;

        public PersonelService(FakhravariDbContext Context)
        {
            db = Context;
        }

        public async Task<int> Login(string UserName, string Password)
        {
            return DateTime.Now.Second;
        }

        public async Task<bool> IsValidName(string Name)
        {
            return (Name).Trim().Contains("test2") == false;
        }
    }
}
