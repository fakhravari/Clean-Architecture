using Domain.Enum;
using Microsoft.EntityFrameworkCore;

namespace Application.Contracts.Persistence.Contexts;

public interface IUnitOfWork<TContext> where TContext : DbContext
{
    void SetDatabaseMode(DatabaseMode mode);
    DatabaseMode Mode { get; }
    TContext Context { get; }
    Task<int> SaveChangesAsync();
}

