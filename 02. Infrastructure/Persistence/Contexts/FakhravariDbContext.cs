using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Persistence.Contexts.Configurations;

namespace Persistence.Contexts;

public partial class FakhravariDbContext : DbContext
{
    public FakhravariDbContext()
    {

    }

    public FakhravariDbContext(DbContextOptions<FakhravariDbContext> options) : base(options)
    {

    }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<Image> Images { get; set; }

    public virtual DbSet<Log> Logs { get; set; }

    public virtual DbSet<Personel> Personels { get; set; }

    public virtual DbSet<PersonnelToChangeRolesActivity> PersonnelToChangeRolesActivities { get; set; }

    public virtual DbSet<Product> Products { get; set; }

    public virtual DbSet<Token> Tokens { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ConfigureModels();

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
