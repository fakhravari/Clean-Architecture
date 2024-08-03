using Application.Contracts.Persistence.Contexts;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Persistence.Contexts
{
    public partial class FakhravariDbContext : DbContext, IDbContext
    {
        public FakhravariDbContext()
        {

        }

        public FakhravariDbContext(DbContextOptions<FakhravariDbContext> options) : base(options)
        {

        }

        public virtual DbSet<Category> Categories { get; set; }

        public virtual DbSet<Image> Images { get; set; }

        public virtual DbSet<Personel> Personels { get; set; }

        public virtual DbSet<Product> Products { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // optionsBuilder.UseSqlServer("Server=185.55.224.117;Initial Catalog=technos6_cafe;User ID=technos6_cafe;Password=cn*6s6I52;TrustServerCertificate=True");
        }



        public Task<int> SaveChangesAsync()
        {
            return base.SaveChangesAsync();
        }
        DatabaseFacade IDbContext.Database => base.Database;



        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("technos6_cafe");

            modelBuilder.Entity<Category>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK__Categori__3214EC07B0DB47C9");

                entity.ToTable("Categories", "General");

                entity.Property(e => e.Title).HasMaxLength(200);
            });

            modelBuilder.Entity<Image>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK__Images__3214EC07D2502567");

                entity.ToTable("Images", "General");

                entity.Property(e => e.Id).ValueGeneratedNever();
                entity.Property(e => e.ImageName).HasMaxLength(100);

                entity.HasOne(d => d.IdProductNavigation).WithMany(p => p.Images)
                    .HasForeignKey(d => d.IdProduct)
                    .HasConstraintName("FK__Images__IdProduc__3F466844");
            });

            modelBuilder.Entity<Personel>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK__Personel__3214EC07DF398778");

                entity.ToTable("Personel", "Security");

                entity.Property(e => e.FirstName).HasMaxLength(400);
                entity.Property(e => e.LastName).HasMaxLength(400);
                entity.Property(e => e.NationalCode).HasMaxLength(50);
                entity.Property(e => e.Password).HasMaxLength(100);
                entity.Property(e => e.UserName).HasMaxLength(100);
            });

            modelBuilder.Entity<Product>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK__Products__3214EC0742A9D342");

                entity.ToTable("Products", "General");

                entity.Property(e => e.Title).HasMaxLength(400);

                entity.HasOne(d => d.IdCategoryNavigation).WithMany(p => p.Products)
                    .HasForeignKey(d => d.IdCategory)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Products__IdCate__3C69FB99");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
