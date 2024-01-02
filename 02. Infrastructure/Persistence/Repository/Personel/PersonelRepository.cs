using Persistence.Contexts;

namespace Persistence.Repository.Personel
{
    public class PersonelRepository : IPersonelRepository
    {
        private readonly FakhravariDbContext db;

        public PersonelRepository(FakhravariDbContext Context)
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
