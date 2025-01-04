using Domain.Enum;
using Microsoft.EntityFrameworkCore;

namespace Application.Contracts.Persistence.Contexts;

public interface IUnitOfWork<TContext> where TContext : DbContext
{
    DatabaseMode Mode { get; }
    TContext Context { get; }
    void SetDatabaseMode(DatabaseMode mode);
    Task<int> SaveChangesAsync();
}